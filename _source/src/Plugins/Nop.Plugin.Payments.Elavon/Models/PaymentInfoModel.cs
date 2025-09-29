// <copyright file="PaymentInfoModel.cs" company="Bellosoft Limited">
// Copyright (c) Bellosoft Limited. All rights reserved.
// </copyright>

using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.Elavon.Models;

/// <summary>
/// Represents a lightbox model
/// </summary>
public record PaymentInfoModel : BaseNopModel
{
    /// <summary>
    /// Gets or sets the lightbox URL
    /// </summary>
    public string LightboxUrl { get; set; }
}