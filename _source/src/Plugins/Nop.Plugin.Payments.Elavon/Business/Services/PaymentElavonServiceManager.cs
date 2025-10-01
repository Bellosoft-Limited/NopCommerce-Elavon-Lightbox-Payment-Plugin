// <copyright file="PaymentElavonServiceManager.cs" company="Bellosoft Limited">
// Copyright (c) Bellosoft Limited. All rights reserved.
// </copyright>

using Nop.Core;
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

    public PaymentElavonServiceManager(
        IWorkContext workContext,
        IStoreContext storeContext,
        IPaymentPluginManager paymentPluginManager)
    {
        _workContext = workContext;
        _storeContext = storeContext;
        _paymentPluginManager = paymentPluginManager;
    }

    public static bool IsConfigured(ElavonPaymentSettings settings)
    {
        return !string.IsNullOrEmpty(settings.HppUrl) && 
            !string.IsNullOrEmpty(settings.ApiUrl) &&
            !string.IsNullOrEmpty(settings.MerchantAlias) &&
            !string.IsNullOrEmpty(settings.PublicKey) &&
            !string.IsNullOrEmpty(settings.SecretKey);
    }

    /// <summary>
    /// Check whether the plugin is configured, connected and active
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the check result; plugin instance
    /// </returns>
    public async Task<(bool Active, IPaymentMethod paymentMethod)> IsActiveAsync(ElavonPaymentSettings settings)
    {
        if (!IsConfigured(settings))
        {
            return (false, null);
        }

        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        var plugin = await _paymentPluginManager.LoadPluginBySystemNameAsync(Globals.SystemName, customer, store.Id);
        if (!_paymentPluginManager.IsPluginActive(plugin))
        {
            return (false, plugin);
        }

        return (true, plugin);
    }
}