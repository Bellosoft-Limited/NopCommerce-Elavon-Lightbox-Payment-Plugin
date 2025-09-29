// <copyright file="TransactionResponse.cs" company="Bellosoft Limited">
// Copyright (c) Bellosoft Limited. All rights reserved.
// </copyright>

using Newtonsoft.Json;

namespace Nop.Plugin.Payments.Elavon.Models.DTO;

/// <summary>
/// Response from Elavon Transaction API
/// </summary>
public class TransactionResponse
{
    [JsonProperty("isAuthorized")] public bool IsAuthorized { get; set; }
}