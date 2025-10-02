// <copyright file="PaymentSessionResponse.cs" company="Bellosoft Limited">
// Copyright (c) Bellosoft Limited. All rights reserved.
// </copyright>

using Newtonsoft.Json;

namespace Nop.Plugin.Payments.Elavon.Models.DTO;

/// <summary>
/// Response from Elavon Payment Session API
/// </summary>
public class PaymentSessionResponse
{
    [JsonProperty("id")] public string SessionId { get; set; }

    [JsonProperty("transaction")] public string TransactionHref { get; set; }
}