// <copyright file="ConfigurationModel.cs" company="Bellosoft Limited">
// Copyright (c) Bellosoft Limited. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.Elavon.Models.Admin;

public record ConfigurationModel : BaseNopModel
{
    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Plugins.Payments.Elavon.Fields.HppUrl")]
    [Required]
    public string HppUrl { get; set; }
    public bool HppUrl_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payments.Elavon.Fields.ApiUrl")]
    [Required]
    public string ApiUrl { get; set; }
    public bool ApiUrl_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payments.Elavon.Fields.MerchantAlias")]
    [Required]
    public string MerchantAlias { get; set; }
    public bool MerchantAlias_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payments.Elavon.Fields.PublicKey")]
    [Required]
    public string PublicKey { get; set; }
    public bool PublicKey_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payments.Elavon.Fields.SecretKey")]
    [Required]
    public string SecretKey { get; set; }
    public bool SecretKey_OverrideForStore { get; set; }
}