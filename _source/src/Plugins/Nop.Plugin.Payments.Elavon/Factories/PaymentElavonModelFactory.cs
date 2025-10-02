// <copyright file="PaymentElavonModelFactory.cs" company="Bellosoft Limited">
// Copyright (c) Bellosoft Limited. All rights reserved.
// </copyright>

using Nop.Core;
using Nop.Plugin.Payments.Elavon.Business;
using Nop.Services.Payments;
using Nop.Web.Factories;
using Nop.Web.Models.Checkout;

namespace Nop.Plugin.Payments.Elavon.Factories;

/// <summary>
/// Represents the plugin model factory
/// </summary>
public class PaymentElavonModelFactory
{
    #region Fields

    private readonly ICheckoutModelFactory _checkoutModelFactory;
    private readonly IWorkContext _workContext;
    private readonly IStoreContext _storeContext;
    private readonly IPaymentPluginManager _paymentPluginManager;

    #endregion

    #region Ctor

    public PaymentElavonModelFactory(
        ICheckoutModelFactory checkoutModelFactory,
        IWorkContext workContext,
        IStoreContext storeContext,
        IPaymentPluginManager paymentPluginManager)
    {
        _checkoutModelFactory = checkoutModelFactory;
        _workContext = workContext;
        _storeContext = storeContext;
        _paymentPluginManager = paymentPluginManager;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare the checkout payment info model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the checkout payment info model
    /// </returns>
    public async Task<CheckoutPaymentInfoModel> PrepareCheckoutPaymentInfoModelAsync()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        var plugin = await _paymentPluginManager.LoadPluginBySystemNameAsync(Globals.SystemName, customer, store.Id);

        if (!_paymentPluginManager.IsPluginActive(plugin) || plugin is null)
            return new();

        return await _checkoutModelFactory.PreparePaymentInfoModelAsync(plugin);
    }

    #endregion
}