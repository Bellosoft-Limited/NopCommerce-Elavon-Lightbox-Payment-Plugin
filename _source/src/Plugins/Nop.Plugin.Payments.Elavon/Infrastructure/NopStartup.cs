// <copyright file="NopStartup.cs" company="Bellosoft Limited">
// Copyright (c) Bellosoft Limited. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Payments.Elavon.Business.Services;
using Nop.Plugin.Payments.Elavon.Business.Services.Api;
using Nop.Plugin.Payments.Elavon.Factories;
using Nop.Plugin.Payments.Elavon.Tasks;
using Nop.Services.Orders;

namespace Nop.Plugin.Payments.Elavon.Infrastructure;

/// <summary>
/// Represents the object for the configuring services on application startup
/// </summary>
public class NopStartup : INopStartup
{
    /// <summary>
    /// Add and configure any of the middleware
    /// </summary>
    /// <param name="services">Collection of service descriptors</param>
    /// <param name="configuration">Configuration of the application</param>
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IApiIntegrationService, ApiIntegrationService>();
        services.AddScoped<IOrderProcessingService, CustomOrderProcessingService>();
        services.AddScoped<PaymentElavonModelFactory>();
        services.AddScoped<PaymentElavonServiceManager>();
        services.AddScoped<SyncOrdersWithApiTask>();
    }

    /// <summary>
    /// Configure the using of added middleware
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public void Configure(IApplicationBuilder application)
    {
    }

    /// <summary>
    /// Gets order of this startup configuration implementation
    /// </summary>
    public int Order => 9999;
}