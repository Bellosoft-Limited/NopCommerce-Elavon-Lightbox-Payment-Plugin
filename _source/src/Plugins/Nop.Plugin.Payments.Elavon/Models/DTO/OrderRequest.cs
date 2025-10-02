// <copyright file="OrderRequest.cs" company="Bellosoft Limited">
// Copyright (c) Bellosoft Limited. All rights reserved.
// </copyright>

using Newtonsoft.Json;

namespace Nop.Plugin.Payments.Elavon.Models.DTO;

/// <summary>
/// Request to the Elavon Order API
/// </summary>
public record OrderRequest
{
    [JsonProperty("total")] public OrderTotal Total { get; set; }
}

public record OrderTotal
{
    [JsonProperty("amount")] public decimal Amount { get; set; }

    [JsonProperty("currencyCode")] public string CurrencyCode { get; set; }
}