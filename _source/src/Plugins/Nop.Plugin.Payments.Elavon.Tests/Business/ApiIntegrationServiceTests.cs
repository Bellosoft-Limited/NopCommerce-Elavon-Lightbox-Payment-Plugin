using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Stores;
using Nop.Plugin.Payments.Elavon.Business;
using Nop.Plugin.Payments.Elavon.Business.Services.Api;
using Nop.Plugin.Payments.Elavon.Tests.Helpers;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Orders;
using NUnit.Framework;

namespace Nop.Plugin.Payments.Elavon.Tests.Business;

[TestFixture]
public class ApiIntegrationServiceTests
{
    private const int STORE_ID = 1;
    private const decimal CART_TOTAL = 200m;

    private readonly Customer _customer = new() { Id = 1, Email = "customer@example.com", CurrencyId = 1 };

    private Mock<IGenericAttributeService> _genericAttributeService;
    private FakeElavonHttpMessageHandler _elavon;
    private ApiIntegrationService _apiIntegrationService;

    [SetUp]
    public void SetUp()
    {
        var workContext = new Mock<IWorkContext>();
        workContext.Setup(x => x.GetCurrentCustomerAsync()).ReturnsAsync(_customer);

        var storeContext = new Mock<IStoreContext>();
        storeContext.Setup(x => x.GetCurrentStoreAsync()).ReturnsAsync(new Store { Id = STORE_ID });

        var webHelper = new Mock<IWebHelper>();
        webHelper.Setup(x => x.GetStoreLocation(It.IsAny<bool?>())).Returns("https://store.test/");

        var currencyService = new Mock<ICurrencyService>();
        currencyService.Setup(x => x.GetCurrencyByIdAsync(It.IsAny<int>())).ReturnsAsync(new Currency { Id = 1, CurrencyCode = "GBP" });

        IList<ShoppingCartItem> cart = [new ShoppingCartItem { ProductId = 1 }];
        var shoppingCartService = new Mock<IShoppingCartService>();
        shoppingCartService
            .Setup(x => x.GetShoppingCartAsync(It.IsAny<Customer>(), It.IsAny<ShoppingCartType?>(), It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
            .ReturnsAsync(cart);

        var orderTotalCalculationService = new Mock<IOrderTotalCalculationService>();
        orderTotalCalculationService
            .Setup(x => x.GetShoppingCartTotalAsync(cart, It.IsAny<bool?>(), It.IsAny<bool>()))
            .ReturnsAsync((CART_TOTAL, 0m, new List<Discount>(), new List<AppliedGiftCard>(), 0, 0m));

        _genericAttributeService = new Mock<IGenericAttributeService>();
        SetupAmountToCharge(null);

        _elavon = new FakeElavonHttpMessageHandler();

        _apiIntegrationService = new ApiIntegrationService(
            webHelper.Object,
            workContext.Object,
            storeContext.Object,
            currencyService.Object,
            shoppingCartService.Object,
            orderTotalCalculationService.Object,
            _genericAttributeService.Object,
            new CurrencySettings { PrimaryStoreCurrencyId = 1 },
            new ElavonPaymentSettings { ApiUrl = "https://elavon.test/", MerchantAlias = "alias", SecretKey = "secret" },
            new HttpClient(_elavon),
            new Mock<ILogger<ApiIntegrationService>>().Object);
    }

    //the amount the Moran's plugin saved for the customer before the payment session; null means nothing saved
    private void SetupAmountToCharge(decimal? amount)
    {
        _genericAttributeService
            .Setup(x => x.GetAttributeAsync(_customer, Globals.AmountToChargeAttribute, STORE_ID, It.IsAny<decimal?>()))
            .ReturnsAsync(amount);
    }

    [Test]
    public async Task GetAmountToChargeAsync_ShouldReturnDeposit_WhenSaved()
    {
        SetupAmountToCharge(50m);

        (await _apiIntegrationService.GetAmountToChargeAsync(200m)).Should().Be(50m);
    }

    [Test]
    public async Task GetAmountToChargeAsync_ShouldReturnTotal_WhenNothingSaved()
    {
        (await _apiIntegrationService.GetAmountToChargeAsync(200m)).Should().Be(200m);
    }

    //never more than the order total, and never nothing
    [TestCase(0)]
    [TestCase(200)]
    [TestCase(250)]
    public async Task GetAmountToChargeAsync_ShouldReturnTotal_WhenSavedAmountIsNotBelowTotal(int savedAmount)
    {
        SetupAmountToCharge(savedAmount);

        (await _apiIntegrationService.GetAmountToChargeAsync(200m)).Should().Be(200m);
    }

    [Test]
    public async Task CreatePaymentSessionAsync_ShouldCreateElavonOrderForAmountToCharge()
    {
        SetupAmountToCharge(50m);

        await _apiIntegrationService.CreatePaymentSessionAsync();

        JObject.Parse(_elavon.RequestBodies["/orders"])["total"]!["amount"]!.Value<decimal>().Should().Be(50m);
    }
}
