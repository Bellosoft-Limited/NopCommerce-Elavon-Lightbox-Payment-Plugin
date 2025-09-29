// <copyright file="ElavonPaymentSettings.cs" company="Bellosoft Limited">
// Copyright (c) Bellosoft Limited. All rights reserved.
// </copyright>

using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.Elavon;

/// <summary>
/// Represents settings of the Elavon payment plugin
/// </summary>
public class ElavonPaymentSettings : ISettings
{
    /// <summary>
    /// Gets or sets the hpp URL
    /// </summary>
    public string HppUrl { get; set; }

    /// <summary>
    /// Gets or sets the api URL
    /// </summary>
    public string ApiUrl { get; set; }

    /// <summary>
    /// Gets or sets the merchant alias
    /// </summary>
    public string MerchantAlias { get; set; }

    /// <summary>
    /// Gets or sets the public API key
    /// </summary>
    public string PublicKey { get; set; }

    /// <summary>
    /// Gets or sets the secret API key
    /// </summary>
    public string SecretKey { get; set; }
}
