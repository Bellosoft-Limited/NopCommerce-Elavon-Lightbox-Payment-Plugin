// <copyright file="PaymentElavonPublicController.cs" company="Bellosoft Limited">
// Copyright (c) Bellosoft Limited. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using Nop.Core.Http.Extensions;
using Nop.Plugin.Payments.Elavon.Business;
using Nop.Plugin.Payments.Elavon.Business.Services.Api;
using Nop.Services.Payments;
using Nop.Web.Controllers;

namespace Nop.Plugin.Payments.Elavon.Controllers;

[AutoValidateAntiforgeryToken]
public class PaymentElavonPublicController : BasePublicController
{
    #region Fields

    private readonly IApiIntegrationService _apiIntegrationService;

    #endregion

    #region Ctor

    public PaymentElavonPublicController(
        IApiIntegrationService apiIntegrationService)
    {
        _apiIntegrationService = apiIntegrationService;
    }

    #endregion

    #region Methods

    [HttpPost]
    public async Task<IActionResult> CreateOrder()
    {
        var paymentSession = await _apiIntegrationService.CreatePaymentSessionAsync();

        var orderPaymentInfo = await HttpContext.Session.GetAsync<ProcessPaymentRequest>(Globals.PaymentRequestSessionKey);
        orderPaymentInfo.CustomValues[Globals.PaymentSessionId] = paymentSession.SessionId;

        await HttpContext.Session.SetAsync("OrderPaymentInfo", orderPaymentInfo);

        return Ok(paymentSession.SessionId);
    }

    #endregion
}