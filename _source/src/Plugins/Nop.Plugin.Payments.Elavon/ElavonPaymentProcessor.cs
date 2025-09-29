// <copyright file="ElavonPaymentProcessor.cs" company="Bellosoft Limited">
// Copyright (c) Bellosoft Limited. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.Elavon.Business;
using Nop.Plugin.Payments.Elavon.Business.Services.Api;
using Nop.Plugin.Payments.Elavon.Components;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Payments.Elavon;

/// <summary>
/// Elavon payment processor
/// </summary>
public class ElavonPaymentProcessor : BasePlugin, IPaymentMethod, IWidgetPlugin
{
    #region Fields

    private readonly ISettingService _settingService;
    private readonly IWebHelper _webHelper;
    private readonly ILocalizationService _localizationService;
    private readonly IApiIntegrationService _apiIntegrationService;

    #endregion

    #region Ctor

    public ElavonPaymentProcessor(
        ISettingService settingService,
        IWebHelper webHelper,
        ILocalizationService localizationService,
        IApiIntegrationService apiIntegrationService)
    {
        _settingService = settingService;
        _webHelper = webHelper;
        _localizationService = localizationService;
        _apiIntegrationService = apiIntegrationService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Process a payment
    /// </summary>
    /// <param name="processPaymentRequest">Payment info required for an order processing</param>
    /// <returns>Process payment result</returns>
    public async Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
    {
        var result = new ProcessPaymentResult();

        if (!processPaymentRequest.CustomValues.TryGetValue(Globals.PaymentSessionId, out var sessionIdValue))
        {
            result.AddError(await _localizationService.GetResourceAsync("Plugins.Payments.Elavon.PaymentFailed"));
        }

        if (!string.IsNullOrEmpty(sessionIdValue.ToString()))
        {
            var transaction = await _apiIntegrationService.GetTransactionBySessionIdAsync(sessionIdValue.ToString());
            if (!transaction.IsAuthorized)
            {
                result.AddError(await _localizationService.GetResourceAsync("Plugins.Payments.Elavon.PaymentFailed"));
            }
        }

        return result;
    }

    /// <summary>
    /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
    /// </summary>
    /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
    public async Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
    {
        await Task.CompletedTask;
    }

    /// <summary>
    /// Returns a value indicating whether payment method should be hidden during checkout
    /// </summary>
    /// <param name="cart">Shopping cart</param>
    /// <returns>true - hide; false - display.</returns>
    public Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
    {
        return Task.FromResult(false);
    }

    /// <summary>
    /// Gets additional handling fee
    /// </summary>
    /// <param name="cart">Shopping cart</param>
    /// <returns>Additional handling fee</returns>
    public Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
    {
        return Task.FromResult(decimal.Zero);
    }

    /// <summary>
    /// Captures payment
    /// </summary>
    /// <param name="capturePaymentRequest">Capture payment request</param>
    /// <returns>Capture payment result</returns>
    public Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
    {
        return Task.FromResult(new CapturePaymentResult { Errors = new[] { "Capture method not supported" } });
    }

    /// <summary>
    /// Refunds a payment
    /// </summary>
    /// <param name="refundPaymentRequest">Request</param>
    /// <returns>Result</returns>
    public Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
    {
        return Task.FromResult(new RefundPaymentResult { Errors = new[] { "Refund method not supported" } });
    }

    /// <summary>
    /// Voids a payment
    /// </summary>
    /// <param name="voidPaymentRequest">Request</param>
    /// <returns>Result</returns>
    public Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
    {
        return Task.FromResult(new VoidPaymentResult { Errors = new[] { "Void method not supported" } });
    }

    /// <summary>
    /// Process recurring payment
    /// </summary>
    /// <param name="processPaymentRequest">Payment info required for an order processing</param>
    /// <returns>Process payment result</returns>
    public Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest)
    {
        return Task.FromResult(new ProcessPaymentResult { Errors = new[] { "Recurring payment not supported" } });
    }

    /// <summary>
    /// Cancels a recurring payment
    /// </summary>
    /// <param name="cancelPaymentRequest">Request</param>
    /// <returns>Result</returns>
    public Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest)
    {
        return Task.FromResult(new CancelRecurringPaymentResult { Errors = new[] { "Recurring payment not supported" } });
    }

    /// <summary>
    /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>Result</returns>
    public Task<bool> CanRePostProcessPaymentAsync(Order order)
    {
        return Task.FromResult(false);
    }

    /// <summary>
    /// Validate payment form
    /// </summary>
    /// <param name="form">The parsed form values</param>
    /// <returns>List of validating errors</returns>
    public Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form)
    {
        return Task.FromResult<IList<string>>(new List<string>());
    }

    /// <summary>
    /// Get payment information
    /// </summary>
    /// <param name="form">The parsed form values</param>
    /// <returns>Payment info holder</returns>
    public Task<ProcessPaymentRequest> GetPaymentInfoAsync(IFormCollection form)
    {
        return Task.FromResult(new ProcessPaymentRequest());
    }

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
        return $"{_webHelper.GetStoreLocation()}Admin/PaymentElavon/Configure";
    }

    /// <summary>
    /// Gets a name of a view component for displaying plugin in public store ("payment info" checkout step)
    /// </summary>
    /// <returns>View component name</returns>
    public Type GetPublicViewComponent()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets widget zones where this widget should be rendered
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the widget zones
    /// </returns>
    public Task<IList<string>> GetWidgetZonesAsync()
    {
        return Task.FromResult<IList<string>>(new List<string>
        {
            PublicWidgetZones.OpCheckoutConfirmBottom,
            PublicWidgetZones.CheckoutConfirmBottom
        });
    }

    /// <summary>
    /// Gets a type of a view component for displaying widget
    /// </summary>
    /// <param name="widgetZone">Name of the widget zone</param>
    /// <returns>View component type</returns>
    public Type GetWidgetViewComponent(string widgetZone)
    {
        return typeof(PaymentElavonViewComponent);
    }

    /// <summary>
    /// Install plugin
    /// </summary>
    public override async Task InstallAsync()
    {
        //settings
        var settings = new ElavonPaymentSettings
        {
            HppUrl = "https://uat.hpp.converge.eu.elavonaws.com",
            ApiUrl = "https://uat.api.converge.eu.elavonaws.com",
            MerchantAlias = "",
            PublicKey = "",
            SecretKey = ""
        };
        await _settingService.SaveSettingAsync(settings);

        //locales
        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.Payments.Elavon.Fields.HppUrl"] = "Hosted Payment Page (Hpp) URL",
            ["Plugins.Payments.Elavon.Fields.HppUrl.Hint"] = "Enter the Elavon Hosted Payment Page (Hpp) url.",
            ["Plugins.Payments.Elavon.Fields.ApiUrl"] = "API URL",
            ["Plugins.Payments.Elavon.Fields.ApiUrl.Hint"] = "Enter the Elavon API url.",
            ["Plugins.Payments.Elavon.Fields.MerchantAlias"] = "Merchant Alias",
            ["Plugins.Payments.Elavon.Fields.MerchantAlias.Hint"] = "Enter your Elavon merchant alias.",
            ["Plugins.Payments.Elavon.Fields.PublicKey"] = "Public API Key",
            ["Plugins.Payments.Elavon.Fields.PublicKey.Hint"] = "Enter your Elavon public API key.",
            ["Plugins.Payments.Elavon.Fields.SecretKey"] = "Secret API Key",
            ["Plugins.Payments.Elavon.Fields.SecretKey.Hint"] = "Enter your Elavon secret API key.",
            ["Plugins.Payments.Elavon.PaymentFailed"] = "Payment was not authorized.",
            ["Plugins.Payments.Elavon.PaymentMethodDescription"] = "Pay securely with Elavon",
            ["Plugins.Payments.Elavon.Order.TransactionId"] = "Elavon transaction ID"
        });

        await base.InstallAsync();
    }

    /// <summary>
    /// Uninstall plugin
    /// </summary>
    public override async Task UninstallAsync()
    {
        //settings
        await _settingService.DeleteSettingAsync<ElavonPaymentSettings>();

        //locales
        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Payments.Elavon");

        await base.UninstallAsync();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether capture is supported
    /// </summary>
    public bool SupportCapture => true;

    /// <summary>
    /// Gets a value indicating whether partial refund is supported
    /// </summary>
    public bool SupportPartiallyRefund => false;

    /// <summary>
    /// Gets a value indicating whether refund is supported
    /// </summary>
    public bool SupportRefund => false;

    /// <summary>
    /// Gets a value indicating whether void is supported
    /// </summary>
    public bool SupportVoid => false;

    /// <summary>
    /// Gets a recurring payment type of payment method
    /// </summary>
    public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;

    /// <summary>
    /// Gets a payment method type
    /// </summary>
    public PaymentMethodType PaymentMethodType => PaymentMethodType.Standard;

    /// <summary>
    /// Gets a value indicating whether we should display a payment information page for this plugin
    /// </summary>
    public bool SkipPaymentInfo => true;

    /// <summary>
    /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
    /// </summary>
    public bool HideInWidgetList => false;

    /// <summary>
    /// Gets a payment method description that will be displayed on checkout pages in the public store
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task<string> GetPaymentMethodDescriptionAsync()
    {
        return await _localizationService.GetResourceAsync("Plugins.Payments.Elavon.PaymentMethodDescription");
    }

    #endregion
}