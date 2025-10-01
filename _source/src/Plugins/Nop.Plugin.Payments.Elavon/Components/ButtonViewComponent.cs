// <copyright file="ButtonViewComponent.cs" company="Bellosoft Limited">
// Copyright (c) Bellosoft Limited. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.Elavon.Business.Services;
using Nop.Plugin.Payments.Elavon.Models;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.Elavon.Components;

/// <summary>
/// Represents the view component to display Elavon button in the public store
/// </summary>
public class ButtonViewComponent : NopViewComponent
{
    private readonly ElavonPaymentSettings _elavonPaymentSettings;
    private readonly PaymentElavonServiceManager _paymentElavonServiceManager;

    public ButtonViewComponent(
        ElavonPaymentSettings elavonPaymentSettings,
        PaymentElavonServiceManager paymentElavonServiceManager)
    {
        _elavonPaymentSettings = elavonPaymentSettings;
        _paymentElavonServiceManager = paymentElavonServiceManager;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var (active, _) = await _paymentElavonServiceManager.IsActiveAsync(_elavonPaymentSettings);
        if (!active)
        {
            return Content(string.Empty);
        }

        var model = new PaymentInfoModel
        {
            LightboxUrl = _elavonPaymentSettings.HppUrl
        };

        return View("~/Plugins/Payments.Elavon/Views/Public/_Button.cshtml", model);
    }
}