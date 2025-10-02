// <copyright file="PaymentElavonPublicController.cs" company="Bellosoft Limited">
// Copyright (c) Bellosoft Limited. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using Nop.Core.Http.Extensions;
using Nop.Plugin.Payments.Elavon.Business;
using Nop.Plugin.Payments.Elavon.Business.Services.Api;
using Nop.Plugin.Payments.Elavon.Factories;
using Nop.Services.Payments;
using Nop.Web.Controllers;

namespace Nop.Plugin.Payments.Elavon.Controllers;

[AutoValidateAntiforgeryToken]
public class PaymentElavonPublicController : BasePublicController
{
    #region Fields

    private readonly IApiIntegrationService _apiIntegrationService;
    private readonly PaymentElavonModelFactory _modelFactory;

    #endregion

    #region Ctor

    public PaymentElavonPublicController(
        IApiIntegrationService apiIntegrationService,
        PaymentElavonModelFactory modelFactory)
    {
        _apiIntegrationService = apiIntegrationService;
        _modelFactory = modelFactory;
    }

    #endregion

    #region Methods

    public async Task<IActionResult> PluginPaymentInfoAsync()
    {
        var model = await _modelFactory.PrepareCheckoutPaymentInfoModelAsync();

        return View("~/Plugins/Payments.Elavon/Views/Public/PluginPaymentInfo.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePaymentSessionAsync()
    {
        try
        {
            var paymentSession = await _apiIntegrationService.CreatePaymentSessionAsync();

            var processPaymentRequest = new ProcessPaymentRequest();
            processPaymentRequest.CustomValues[Globals.PaymentSessionId] = paymentSession.SessionId;

            await HttpContext.Session.SetAsync(Globals.PaymentRequestSessionKey, processPaymentRequest);

            return Ok(paymentSession.SessionId);
        } 
        catch (Exception)
        {
            return Problem();
        }
    }

    #endregion
}