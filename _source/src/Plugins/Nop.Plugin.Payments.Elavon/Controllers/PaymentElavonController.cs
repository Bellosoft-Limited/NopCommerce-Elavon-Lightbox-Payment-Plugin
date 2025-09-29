// <copyright file="PaymentElavonController.cs" company="Bellosoft Limited">
// Copyright (c) Bellosoft Limited. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.Elavon.Models.Admin;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.Elavon.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
public class PaymentElavonController : BasePaymentController
{
    #region Fields

    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    private readonly IPermissionService _permissionService;
    private readonly ISettingService _settingService;
    private readonly IStoreContext _storeContext;

    #endregion

    #region Ctor

    public PaymentElavonController(ILocalizationService localizationService,
        INotificationService notificationService,
        IPermissionService permissionService,
        ISettingService settingService,
        IStoreContext storeContext)
    {
        _localizationService = localizationService;
        _notificationService = notificationService;
        _permissionService = permissionService;
        _settingService = settingService;
        _storeContext = storeContext;
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> ConfigureAsync()
    {
        //load settings for a chosen store scope
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var elavonPaymentSettings = await _settingService.LoadSettingAsync<ElavonPaymentSettings>(storeScope);

        var model = new ConfigurationModel
        {
            HppUrl = elavonPaymentSettings.HppUrl,
            ApiUrl = elavonPaymentSettings.ApiUrl,
            MerchantAlias = elavonPaymentSettings.MerchantAlias,
            PublicKey = elavonPaymentSettings.PublicKey,
            SecretKey = elavonPaymentSettings.SecretKey,
            ActiveStoreScopeConfiguration = storeScope
        };

        if (storeScope > 0)
        {
            model.HppUrl_OverrideForStore = await _settingService.SettingExistsAsync(elavonPaymentSettings, x => x.HppUrl, storeScope);
            model.ApiUrl_OverrideForStore = await _settingService.SettingExistsAsync(elavonPaymentSettings, x => x.ApiUrl, storeScope);
            model.MerchantAlias_OverrideForStore = await _settingService.SettingExistsAsync(elavonPaymentSettings, x => x.MerchantAlias, storeScope);
            model.PublicKey_OverrideForStore = await _settingService.SettingExistsAsync(elavonPaymentSettings, x => x.PublicKey, storeScope);
            model.SecretKey_OverrideForStore = await _settingService.SettingExistsAsync(elavonPaymentSettings, x => x.SecretKey, storeScope);
        }

        return View("~/Plugins/Payments.Elavon/Views/Configure.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> ConfigureAsync(ConfigurationModel model)
    {
        if (!ModelState.IsValid)
            return await ConfigureAsync();

        //load settings for a chosen store scope
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var elavonPaymentSettings = await _settingService.LoadSettingAsync<ElavonPaymentSettings>(storeScope);

        //save settings
        elavonPaymentSettings.HppUrl = model.HppUrl;
        elavonPaymentSettings.ApiUrl = model.ApiUrl;
        elavonPaymentSettings.MerchantAlias = model.MerchantAlias;
        elavonPaymentSettings.PublicKey = model.PublicKey;
        elavonPaymentSettings.SecretKey = model.SecretKey;

        /* We do not clear cache after each setting update.
         * This behavior can increase performance because cached settings will not be cleared 
         * and loaded from database after each update */
        await _settingService.SaveSettingOverridablePerStoreAsync(elavonPaymentSettings, x => x.HppUrl, model.HppUrl_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(elavonPaymentSettings, x => x.ApiUrl, model.ApiUrl_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(elavonPaymentSettings, x => x.MerchantAlias, model.MerchantAlias_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(elavonPaymentSettings, x => x.PublicKey, model.PublicKey_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(elavonPaymentSettings, x => x.SecretKey, model.SecretKey_OverrideForStore, storeScope, false);

        //now clear settings cache
        await _settingService.ClearCacheAsync();

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await ConfigureAsync();
    }

    #endregion
}