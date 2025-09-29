// <copyright file="IApiIntegrationService.cs" company="Bellosoft Limited">
// Copyright (c) Bellosoft Limited. All rights reserved.
// </copyright>

using Nop.Plugin.Payments.Elavon.Models.DTO;

namespace Nop.Plugin.Payments.Elavon.Business.Services.Api;

public interface IApiIntegrationService
{
    Task<PaymentSessionResponse> CreatePaymentSessionAsync();

    Task<OrderResponse> GetElavonOrderBySessionIdAsync(string sessionId);

    Task<bool> VerifyTransactionApprovalAsync(string sessionId);
}
