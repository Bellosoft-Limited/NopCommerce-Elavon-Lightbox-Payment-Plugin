// <copyright file="PaymentElavonServiceManager.cs" company="Bellosoft Limited">
// Copyright (c) Bellosoft Limited. All rights reserved.
// </copyright>

using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Common;
using Nop.Services.Payments;

namespace Nop.Plugin.Payments.Elavon.Business.Services;

/// <summary>
/// Represents the plugin service manager
/// </summary>
public class PaymentElavonServiceManager
{
    private readonly IWorkContext _workContext;
    private readonly IStoreContext _storeContext;
    private readonly IPaymentPluginManager _paymentPluginManager;
    private readonly IGenericAttributeService _genericAttributeService;

    public PaymentElavonServiceManager(
        IWorkContext workContext,
        IStoreContext storeContext,
        IPaymentPluginManager paymentPluginManager,
        IGenericAttributeService genericAttributeService)
    {
        _workContext = workContext;
        _storeContext = storeContext;
        _paymentPluginManager = paymentPluginManager;
        _genericAttributeService = genericAttributeService;
    }

    /// <summary>
    /// Check whether the plugin is configured
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <returns>Result</returns>
    public static bool IsConfigured(ElavonPaymentSettings settings)
    {
        return !string.IsNullOrEmpty(settings.HppUrl) && 
            !string.IsNullOrEmpty(settings.ApiUrl) &&
            !string.IsNullOrEmpty(settings.MerchantAlias) &&
            !string.IsNullOrEmpty(settings.PublicKey) &&
            !string.IsNullOrEmpty(settings.SecretKey);
    }

    /// <summary>
    /// Check whether the plugin is configured and active
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the check result; plugin instance
    /// </returns>
    public async Task<bool> IsActiveAsync(ElavonPaymentSettings settings)
    {
        if (!IsConfigured(settings))
        {
            return false;
        }

        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        var plugin = await _paymentPluginManager.LoadPluginBySystemNameAsync(Globals.SystemName, customer, store.Id);

        return _paymentPluginManager.IsPluginActive(plugin);
    }

    /// <summary>
    /// Check whether the payment method is selected
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <returns>Result</returns>
    public async Task<bool> IsSelectedAsync()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();

        var selectedPaymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SelectedPaymentMethodAttribute, store.Id);

        return !string.IsNullOrEmpty(selectedPaymentMethodSystemName)
            && selectedPaymentMethodSystemName.Equals(Globals.SystemName, StringComparison.InvariantCultureIgnoreCase);
    }
}