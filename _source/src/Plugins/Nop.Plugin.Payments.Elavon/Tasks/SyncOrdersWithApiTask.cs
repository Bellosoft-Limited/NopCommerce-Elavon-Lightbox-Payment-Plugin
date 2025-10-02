// <copyright file="SyncOrdersWithApiTask.cs" company="Bellosoft Limited">
// Copyright (c) Bellosoft Limited. All rights reserved.
// </copyright>

using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Plugin.Payments.Elavon.Business;
using Nop.Plugin.Payments.Elavon.Business.Services.Api;
using Nop.Plugin.Payments.Elavon.Models.DTO;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.ScheduleTasks;

namespace Nop.Plugin.Payments.Elavon.Tasks;

/// <summary>
/// Represents a task to sync order payments with api transactions
/// </summary>
public class SyncOrdersWithApiTask : IScheduleTask
{
    private readonly IScheduleTaskService _scheduleTaskService;
    private readonly IOrderService _orderService;
    private readonly IApiIntegrationService _apiIntegrationService;
    private readonly ILocalizationService _localizationService;
    private readonly IPaymentService _paymentService;
    private readonly IOrderProcessingService _orderProcessingService;
    private readonly ILogger _logger;

    public SyncOrdersWithApiTask(
        IScheduleTaskService scheduleTaskService,
        IOrderService orderService,
        IApiIntegrationService apiIntegrationService,
        ILocalizationService localizationService,
        IPaymentService paymentService,
        IOrderProcessingService orderProcessingService,
        ILogger logger)
    {
        _scheduleTaskService = scheduleTaskService;
        _orderService = orderService;
        _apiIntegrationService = apiIntegrationService;
        _localizationService = localizationService;
        _paymentService = paymentService;
        _orderProcessingService = orderProcessingService;
        _logger = logger;
    }

    protected static string TaskType => "Nop.Plugin.Payments.Elavon.Tasks.SyncOrdersWithApiTask";

    public async Task ExecuteAsync()
    {
        try
        {
            await _logger.InformationAsync("SyncOrdersWithApiTask: Starting sync");

            var orders = await _orderService.SearchOrdersAsync(
                paymentMethodSystemName: Globals.SystemName,
                psIds: new List<int> { (int)PaymentStatus.Authorized }
            );

            var updatedCount = 0;
            var failedCount = 0;

            foreach (var order in orders)
            {
                try
                {
                    var updated = await ProcessOrderAsync(order);
                    if (updated)
                    {
                        updatedCount++;
                    }
                }
                catch (Exception ex)
                {
                    failedCount++;
                    await _logger.ErrorAsync($"SyncOrdersWithApiTask: Error processing order {order.Id}", ex);
                }
            }

            await _logger.InformationAsync($"SyncOrdersWithApiTask: Completed. Processed {orders.Count} orders. Updated: {updatedCount}, Failed: {failedCount}");
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync("SyncOrdersWithApiTask: Fatal error during sync", ex);
        }
    }

    private async Task<bool> ProcessOrderAsync(Order order)
    {
        var transactionIdKey = await _localizationService.GetResourceAsync("Plugins.Payments.Elavon.Order.TransactionId");
        if (!order.CustomValuesXml.Contains(transactionIdKey))
        {
            await _logger.WarningAsync($"SyncOrdersWithApiTask: Order {order.Id} does not have transaction ID");
            return false;
        }

        var customValues = _paymentService.DeserializeCustomValues(order);
        if (!customValues.TryGetValue(transactionIdKey, out var transactionIdValue))
        {
            await _logger.WarningAsync($"SyncOrdersWithApiTask: Order {order.Id} transaction ID not found in custom values");
            return false;
        }

        var transactionId = transactionIdValue?.ToString();
        if (string.IsNullOrEmpty(transactionId))
        {
            await _logger.WarningAsync($"SyncOrdersWithApiTask: Order {order.Id} has empty transaction ID");
            return false;
        }

        var transaction = await _apiIntegrationService.GetTransactionByIdAsync(transactionId);
        if (transaction == null)
        {
            await _logger.WarningAsync($"SyncOrdersWithApiTask: Could not retrieve transaction {transactionId} for order {order.Id}");
            return false;
        }

        var stateChanged = await UpdateOrderBasedOnTransactionStateAsync(order, transaction);
        return stateChanged;
    }

    private async Task<bool> UpdateOrderBasedOnTransactionStateAsync(Order order, TransactionResponse transaction)
    {
        var originalStatus = order.PaymentStatus;
        var newStatus = originalStatus;

        switch (transaction.State)
        {
            case TransactionState.SETTLED:
                newStatus = PaymentStatus.Paid;
                break;
        }

        if (newStatus != originalStatus)
        {
            if (newStatus == PaymentStatus.Paid)
            {
                await _orderProcessingService.MarkOrderAsPaidAsync(order);
            }

            await _logger.InformationAsync($"SyncOrdersWithApiTask: Updated order {order.Id} from {originalStatus} to {newStatus}");

            return true;
        }

        return false;
    }

    public async Task InstallTaskAsync()
    {
        var scheduleTask = await GetScheduleTaskAsync();
        if (scheduleTask.Id > 0)
        {
            await _scheduleTaskService.UpdateTaskAsync(scheduleTask);
        }
        else
        {
            await _scheduleTaskService.InsertTaskAsync(scheduleTask);
        }
    }

    public async Task UninstallTaskAsync()
    {
        var scheduleTask = await GetScheduleTaskAsync();
        if (scheduleTask.Id > 0)
        {
            await _scheduleTaskService.DeleteTaskAsync(scheduleTask);
        }
    }

    private async Task<ScheduleTask> GetScheduleTaskAsync()
    {
        var task = await _scheduleTaskService.GetTaskByTypeAsync(TaskType) ?? new ScheduleTask
        {
            Type = TaskType,
            Name = "Sync Orders with Elavon API",
            Enabled = true,
            StopOnError = false,
            Seconds = 1800 // Run every 30 minutes
        };

        return task;
    }
}