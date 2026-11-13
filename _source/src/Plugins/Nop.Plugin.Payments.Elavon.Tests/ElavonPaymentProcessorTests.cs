using FluentAssertions;
using Moq;
using Nop.Core;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.Elavon.Business;
using Nop.Plugin.Payments.Elavon.Business.Services.Api;
using Nop.Plugin.Payments.Elavon.Models.DTO;
using Nop.Plugin.Payments.Elavon.Tasks;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.ScheduleTasks;
using NUnit.Framework;

namespace Nop.Plugin.Payments.Elavon.Tests;

[TestFixture]
public class ElavonPaymentProcessorTests
{
    private Mock<IApiIntegrationService> _apiIntegrationService;
    private ElavonPaymentProcessor _processor;

    [SetUp]
    public void SetUp()
    {
        _apiIntegrationService = new Mock<IApiIntegrationService>();
        _apiIntegrationService
            .Setup(x => x.GetTransactionBySessionIdAsync("session-1"))
            .ReturnsAsync(new TransactionResponse { Id = "transaction-1", IsAuthorized = true });

        var localizationService = new Mock<ILocalizationService>();
        localizationService.Setup(x => x.GetResourceAsync("Plugins.Payments.Elavon.Order.TransactionId")).ReturnsAsync("Elavon transaction ID");

        var syncOrdersWithApiTask = new SyncOrdersWithApiTask(
            new Mock<IScheduleTaskService>().Object,
            new Mock<IOrderService>().Object,
            _apiIntegrationService.Object,
            localizationService.Object,
            new Mock<IPaymentService>().Object,
            new Mock<IOrderProcessingService>().Object,
            new Mock<ILogger>().Object);

        _processor = new ElavonPaymentProcessor(
            new Mock<ISettingService>().Object,
            new Mock<IWebHelper>().Object,
            localizationService.Object,
            _apiIntegrationService.Object,
            new ElavonPaymentSettings(),
            syncOrdersWithApiTask);
    }

    //the culture-independent text is checked on this machine's pt-BR culture too, which writes 50,00
    [TestCase(50, "50.00")]
    [TestCase(200, "200.00")]
    public async Task ProcessPaymentAsync_ShouldRecordAmountCharged(int amountToCharge, string expectedAmountCharged)
    {
        _apiIntegrationService.Setup(x => x.GetAmountToChargeAsync(200m)).ReturnsAsync(amountToCharge);
        var processPaymentRequest = new ProcessPaymentRequest { OrderTotal = 200m };
        processPaymentRequest.CustomValues[Globals.PaymentSessionId] = "session-1";

        var result = await _processor.ProcessPaymentAsync(processPaymentRequest);

        result.NewPaymentStatus.Should().Be(PaymentStatus.Authorized);
        processPaymentRequest.CustomValues[Globals.AmountChargedCustomValue].Should().Be(expectedAmountCharged);
    }

    //SyncOrdersWithApiTask finds the payment by its transaction ID; the session is only needed until the payment is checked
    [Test]
    public async Task ProcessPaymentAsync_ShouldSaveTransactionIdInPlaceOfSession()
    {
        var processPaymentRequest = new ProcessPaymentRequest { OrderTotal = 200m };
        processPaymentRequest.CustomValues[Globals.PaymentSessionId] = "session-1";

        await _processor.ProcessPaymentAsync(processPaymentRequest);

        processPaymentRequest.CustomValues.Should().Contain("Elavon transaction ID", "transaction-1");
        processPaymentRequest.CustomValues.Should().NotContainKey(Globals.PaymentSessionId);
    }
}
