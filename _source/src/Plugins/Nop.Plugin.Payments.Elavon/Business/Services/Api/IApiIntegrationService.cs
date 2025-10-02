// <copyright file="IApiIntegrationService.cs" company="Bellosoft Limited">
// Copyright (c) Bellosoft Limited. All rights reserved.
// </copyright>

using Nop.Plugin.Payments.Elavon.Models.DTO;

namespace Nop.Plugin.Payments.Elavon.Business.Services.Api;

public interface IApiIntegrationService
{
    Task<PaymentSessionResponse> CreatePaymentSessionAsync();

    Task<TransactionResponse> GetTransactionByIdAsync(string transactionId);

    Task<TransactionResponse> GetTransactionBySessionIdAsync(string sessionId);
}
