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

    /// <summary>
    /// Gets the amount to charge for an order of this total: the amount saved for the current customer (a deposit) when it
    /// is part of the total, otherwise the total
    /// </summary>
    Task<decimal> GetAmountToChargeAsync(decimal orderTotal);
}
