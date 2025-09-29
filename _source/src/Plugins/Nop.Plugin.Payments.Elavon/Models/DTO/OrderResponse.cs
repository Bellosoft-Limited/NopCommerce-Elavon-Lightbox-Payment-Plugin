// <copyright file="OrderResponse.cs" company="Bellosoft Limited">
// Copyright (c) Bellosoft Limited. All rights reserved.
// </copyright>

using Newtonsoft.Json;

namespace Nop.Plugin.Payments.Elavon.Models.DTO;

/// <summary>
/// Response from Elavon Order API
/// </summary>
public class OrderResponse
{
    [JsonProperty("href")] public string Href { get; set; }

    [JsonProperty("id")] public string OrderId { get; set; }
}