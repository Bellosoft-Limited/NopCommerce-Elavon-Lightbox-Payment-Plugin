// <copyright file="PaymentElavonViewComponent.cs" company="Bellosoft Limited">
// Copyright (c) Bellosoft Limited. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.Elavon.Models;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.Elavon.Components;

public class PaymentElavonViewComponent : NopViewComponent
{
    private readonly ElavonPaymentSettings _elavonPaymentSettings;

    public PaymentElavonViewComponent(
        ElavonPaymentSettings elavonPaymentSettings)
    {
        _elavonPaymentSettings = elavonPaymentSettings;
    }

    public IViewComponentResult Invoke()
    {
        var model = new PaymentInfoModel
        {
            LightboxUrl = _elavonPaymentSettings.HppUrl
        };

        return View("~/Plugins/Payments.Elavon/Views/PaymentInfo.cshtml", model);
    }
}
