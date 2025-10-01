// <copyright file="CustomOrderProcessingService.cs" company="Bellosoft Limited">
// Copyright (c) Bellosoft Limited. All rights reserved.
// </copyright>

using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Events;
using Nop.Plugin.Payments.Elavon.Business.Services.Api;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;

namespace Nop.Plugin.Payments.Elavon.Business.Services;

internal class CustomOrderProcessingService : OrderProcessingService
{
    private readonly IApiIntegrationService _apiIntegrationService;

    #region Constructor

    public CustomOrderProcessingService(
        CurrencySettings currencySettings,
        IAddressService addressService,
        IAffiliateService affiliateService,
        ICheckoutAttributeFormatter checkoutAttributeFormatter,
        ICountryService countryService,
        ICurrencyService currencyService,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService,
        ICustomNumberFormatter customNumberFormatter,
        IDiscountService discountService,
        IEncryptionService encryptionService,
        IEventPublisher eventPublisher,
        IGenericAttributeService genericAttributeService,
        IGiftCardService giftCardService,
        ILanguageService languageService,
        ILocalizationService localizationService,
        ILogger logger,
        IOrderService orderService,
        IOrderTotalCalculationService orderTotalCalculationService,
        IPaymentPluginManager paymentPluginManager,
        IPaymentService paymentService,
        IPdfService pdfService,
        IPriceCalculationService priceCalculationService,
        IPriceFormatter priceFormatter,
        IProductAttributeFormatter productAttributeFormatter,
        IProductAttributeParser productAttributeParser,
        IProductService productService,
        IReturnRequestService returnRequestService,
        IRewardPointService rewardPointService,
        IShipmentService shipmentService,
        IShippingService shippingService,
        IShoppingCartService shoppingCartService,
        IStateProvinceService stateProvinceService,
        IStaticCacheManager staticCacheManager,
        IStoreMappingService storeMappingService,
        IStoreService storeService,
        ITaxService taxService,
        IVendorService vendorService,
        IWebHelper webHelper,
        IWorkContext workContext,
        IWorkflowMessageService workflowMessageService,
        LocalizationSettings localizationSettings,
        OrderSettings orderSettings,
        PaymentSettings paymentSettings,
        RewardPointsSettings rewardPointsSettings,
        ShippingSettings shippingSettings,
        TaxSettings taxSettings,
        IApiIntegrationService apiIntegrationService)
        : base(currencySettings,
            addressService,
            affiliateService,
            checkoutAttributeFormatter,
            countryService,
            currencyService,
            customerActivityService,
            customerService,
            customNumberFormatter,
            discountService,
            encryptionService,
            eventPublisher,
            genericAttributeService,
            giftCardService,
            languageService,
            localizationService,
            logger,
            orderService,
            orderTotalCalculationService,
            paymentPluginManager,
            paymentService,
            pdfService,
            priceCalculationService,
            priceFormatter,
            productAttributeFormatter,
            productAttributeParser,
            productService,
            returnRequestService,
            rewardPointService,
            shipmentService,
            shippingService,
            shoppingCartService,
            stateProvinceService,
            staticCacheManager,
            storeMappingService,
            storeService,
            taxService,
            vendorService,
            webHelper,
            workContext,
            workflowMessageService,
            localizationSettings,
            orderSettings,
            paymentSettings,
            rewardPointsSettings,
            shippingSettings,
            taxSettings)
    {
        _apiIntegrationService = apiIntegrationService;
    }

    #endregion Constructor

    protected override async Task<Order> SaveOrderDetailsAsync(ProcessPaymentRequest processPaymentRequest, ProcessPaymentResult processPaymentResult, PlaceOrderContainer details)
    {
        var sessionId = processPaymentRequest?.CustomValues?[Globals.PaymentSessionId] as string ?? string.Empty;
        var transaction = await _apiIntegrationService.GetTransactionBySessionIdAsync(sessionId);

        if (transaction != null)
        {
            var transactionIdKey = await _localizationService.GetResourceAsync("Plugins.Payments.Elavon.Order.TransactionId");
            processPaymentRequest.CustomValues[transactionIdKey] = transaction.Id;
            processPaymentRequest.CustomValues.Remove(Globals.PaymentSessionId);
        }

        return await base.SaveOrderDetailsAsync(processPaymentRequest, processPaymentResult, details);
    }
}
