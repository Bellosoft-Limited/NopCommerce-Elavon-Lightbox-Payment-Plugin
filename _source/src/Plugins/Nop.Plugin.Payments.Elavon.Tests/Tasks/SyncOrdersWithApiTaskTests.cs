using Moq;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.Elavon.Business;
using Nop.Plugin.Payments.Elavon.Business.Services.Api;
using Nop.Plugin.Payments.Elavon.Models.DTO;
using Nop.Plugin.Payments.Elavon.Tasks;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.ScheduleTasks;
using NUnit.Framework;

namespace Nop.Plugin.Payments.Elavon.Tests.Tasks;

[TestFixture]
public class SyncOrdersWithApiTaskTests
{
    private const string TRANSACTION_ID_KEY = "Transaction ID";

    private Order _order;
    private Dictionary<string, object> _customValues;
    private Mock<IApiIntegrationService> _apiIntegrationService;
    private Mock<IOrderProcessingService> _orderProcessingService;
    private SyncOrdersWithApiTask _task;

    [SetUp]
    public void SetUp()
    {
        //an Authorized Elavon order for 200.00 whose transaction Elavon has settled
        _order = new Order { Id = 1, OrderTotal = 200m, CustomValuesXml = $"<DictionarySerializer><item><key>{TRANSACTION_ID_KEY}</key><value>transaction-1</value></item></DictionarySerializer>" };
        _customValues = new Dictionary<string, object> { [TRANSACTION_ID_KEY] = "transaction-1" };

        var orderService = new Mock<IOrderService>();
        orderService
            .Setup(x => x.SearchOrdersAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<List<int>>(), It.IsAny<List<int>>(),
                It.IsAny<List<int>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
            .ReturnsAsync(() => new PagedList<Order>([_order], 0, int.MaxValue));

        var localizationService = new Mock<ILocalizationService>();
        localizationService.Setup(x => x.GetResourceAsync("Plugins.Payments.Elavon.Order.TransactionId")).ReturnsAsync(TRANSACTION_ID_KEY);

        var paymentService = new Mock<IPaymentService>();
        paymentService.Setup(x => x.DeserializeCustomValues(_order)).Returns(() => _customValues);

        _apiIntegrationService = new Mock<IApiIntegrationService>();
        _apiIntegrationService
            .Setup(x => x.GetTransactionByIdAsync("transaction-1"))
            .ReturnsAsync(new TransactionResponse { Id = "transaction-1", State = TransactionState.SETTLED });

        _orderProcessingService = new Mock<IOrderProcessingService>();

        _task = new SyncOrdersWithApiTask(
            new Mock<IScheduleTaskService>().Object,
            orderService.Object,
            _apiIntegrationService.Object,
            localizationService.Object,
            paymentService.Object,
            _orderProcessingService.Object,
            new Mock<ILogger>().Object);
    }

    //a deposit order is marked paid like any other, so the "order paid" event sends the deposit to Logic Cloud;
    //orders placed before deposits have no amount charged
    [TestCase("50.00")]
    [TestCase("200.00")]
    [TestCase(null)]
    public async Task ExecuteAsync_ShouldMarkOrderPaid_WhenSettled(string amountCharged)
    {
        if (amountCharged != null)
        {
            _customValues[Globals.AmountChargedCustomValue] = amountCharged;
        }

        await _task.ExecuteAsync();

        _orderProcessingService.Verify(x => x.MarkOrderAsPaidAsync(_order), Times.Once);
    }
}
