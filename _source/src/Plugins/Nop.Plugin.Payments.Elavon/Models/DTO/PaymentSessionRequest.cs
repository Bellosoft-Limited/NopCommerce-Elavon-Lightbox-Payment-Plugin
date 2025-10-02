// <copyright file="PaymentSessionRequest.cs" company="Bellosoft Limited">
// Copyright (c) Bellosoft Limited. All rights reserved.
// </copyright>

using Newtonsoft.Json;

namespace Nop.Plugin.Payments.Elavon.Models.DTO;

/// <summary>
/// Request to the Elavon Payment Session API
/// </summary>
public class PaymentSessionRequest
{
    public PaymentSessionRequest(string elavonOrderHref, string originUrl)
    {
        Order = elavonOrderHref;
        OriginUrl = originUrl;
    }

    [JsonProperty("order")] public string Order { get; set; }

    [JsonProperty("hppType")] public string HppType = "lightbox";

    [JsonProperty("originUrl")] public string OriginUrl { get; set; }

    [JsonProperty("doCreateTransaction")] public bool DoCreateTransaction = true;
}