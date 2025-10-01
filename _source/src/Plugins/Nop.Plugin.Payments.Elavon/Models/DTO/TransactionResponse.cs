// <copyright file="TransactionResponse.cs" company="Bellosoft Limited">
// Copyright (c) Bellosoft Limited. All rights reserved.
// </copyright>

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nop.Plugin.Payments.Elavon.Models.DTO;

/// <summary>
/// Response from Elavon Transaction API
/// </summary>
public class TransactionResponse
{
    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("isAuthorized")] public bool IsAuthorized { get; set; }

    [JsonProperty("state")]
    [JsonConverter(typeof(StringEnumConverter))]
    public TransactionState State { get; set; }
}

public enum TransactionState
{
    [EnumMember(Value = "declined")]
    DECLINED,

    [EnumMember(Value = "authorized")]
    AUTHORIZED,

    [EnumMember(Value = "captured")]
    CAPTURED,

    [EnumMember(Value = "voided")]
    VOIDED,

    [EnumMember(Value = "settled")]
    SETTLED,

    [EnumMember(Value = "expired")]
    EXPIRED,

    [EnumMember(Value = "settlementDelayed")]
    SETTLEMENTDELAYED,

    [EnumMember(Value = "rejected")]
    REJECTED,

    [EnumMember(Value = "heldForReview")]
    HELDFORREVIEW,

    [EnumMember(Value = "unknown")]
    UNKNOWN,

    [EnumMember(Value = "authorizationPending")]
    AUTHORIZATIONPENDING
}