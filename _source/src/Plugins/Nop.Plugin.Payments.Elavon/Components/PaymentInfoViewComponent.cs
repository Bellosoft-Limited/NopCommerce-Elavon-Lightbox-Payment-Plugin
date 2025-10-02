// <copyright file="PaymentInfoViewComponent.cs" company="Bellosoft Limited">
// Copyright (c) Bellosoft Limited. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.Elavon.Components;

public class PaymentInfoViewComponent : NopViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View("~/Plugins/Payments.Elavon/Views/Public/PaymentInfo.cshtml");
    }
}
