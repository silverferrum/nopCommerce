using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Payments.PayExCheckout2.Models;
using Nop.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.PayExCheckout2.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class PayExCheckoutController : BasePaymentController
    {
        #region Fields
        
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public PayExCheckoutController(ILocalizationService localizationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._settingService = settingService;
            this._storeContext = storeContext;
            this._workContext = workContext;
        }

        #endregion

        #region Methods

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var paymentSettings = _settingService.LoadSetting<PayExCheckoutSettings>(storeScope);                       

            var model = AutoMapperConfiguration.Mapper.Map<PayExCheckoutSettings, ConfigurationModel>(paymentSettings);

            model.Urls_callbackUrl = paymentSettings.Urls_callbackUrl;
            model.ApiEndPointValues = new SelectList(PayExCheckoutSettings.ApiEndPoints, paymentSettings.ApiEndPoint);
            model.LanguageValues = new SelectList(PayExCheckoutSettings.Languages, paymentSettings.Language);

            if (storeScope > 0)
            {
                model.ApiEndPoint_OverrideForStore = _settingService.SettingExists(paymentSettings, x => x.ApiEndPoint, storeScope);
                model.Language_OverrideForStore = _settingService.SettingExists(paymentSettings, x => x.Language, storeScope);
                model.MerchantToken_OverrideForStore = _settingService.SettingExists(paymentSettings, x => x.MerchantToken, storeScope);
                model.PayeeId_OverrideForStore = _settingService.SettingExists(paymentSettings, x => x.PayeeId, storeScope);
            }

            return View("~/Plugins/Payments.PayExCheckout2/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var paymentSettings = _settingService.LoadSetting<PayExCheckoutSettings>(storeScope);

            //save settings
            model.Urls_callbackUrl = paymentSettings.Urls_callbackUrl;
            paymentSettings = AutoMapperConfiguration.Mapper.Map(model, paymentSettings);
            //paymentSettings.ApiEndPoint = model.ApiEndPoint;
            //paymentSettings.Language = model.Language;
            //paymentSettings.MerchantToken = model.MerchantToken;
            //paymentSettings.PayeeId = model.PayeeId;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */

            _settingService.SaveSetting(paymentSettings, storeScope);

            //_settingService.SaveSettingOverridablePerStore(paymentSettings, x => x.ApiEndPoint, model.ApiEndPoint_OverrideForStore, storeScope, false);
            //_settingService.SaveSettingOverridablePerStore(paymentSettings, x => x.Language, model.Language_OverrideForStore, storeScope, false);
            //_settingService.SaveSettingOverridablePerStore(paymentSettings, x => x.MerchantToken, model.MerchantToken_OverrideForStore, storeScope, false);
            //_settingService.SaveSettingOverridablePerStore(paymentSettings, x => x.PayeeId, model.PayeeId_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }


        #endregion
    }
}