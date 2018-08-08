using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Plugins;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Tax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.PayExCheckout2
{
    public class PayExCheckoutPlugin : BasePlugin, IPaymentMethod
    {
        /// <summary>
        /// returns "Payments.PayExCheckout2"
        /// </summary>
        public static readonly string PaymentSystemName = "Payments.PayExCheckout2";
        /// <summary>
        /// returns "paymentOrderId"
        /// </summary>
        public static readonly string PaymentOrderIdCustomValueKey = "paymentOrderId";

        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IOrderService _orderService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IPaymentService _paymentService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly PayExCheckoutSettings _paymentSettings;

        #endregion        

        #region Ctor

        public PayExCheckoutPlugin(ILocalizationService localizationService,
            IOrderService orderService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IPaymentService paymentService,
            ISettingService settingService,
            IWebHelper webHelper,
            PayExCheckoutSettings paymentSettings)
        {
            this._orderService = orderService;
            this._localizationService = localizationService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._paymentService = paymentService;
            this._settingService = settingService;
            this._webHelper = webHelper;
            this._paymentSettings = paymentSettings;
        }
        #endregion

        #region Base Plugin Overridings

        public override void Install()
        {
            var defaultSettings = new PayExCheckoutSettings()
            {
                ApiEndPoint = PayExCheckoutSettings.ApiEndPoints[0],
                Language = PayExCheckoutSettings.Languages[0],
                Urls_callbackUrl = $"{_webHelper.GetStoreLocation()}payex/callback/",
                CreditCard_SettingsEnabled = false                
            };

            _settingService.SaveSetting(defaultSettings);

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.PaymentMethodDescription", "PayEx Checkout v2 is a complete reimagination of the checkout experience, integrating seamlessly into the merchant website through highly customizable and flexible components.");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.ApiEndPoint", "Api End Point");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.ApiEndPoint.Hint", "Specify api end point (test or production one)");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.Language", "UI Language");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.Language.Hint", "UI Language");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.MerchantToken", "Merchant Token");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.MerchantToken.Hint", "Enter yours Merchant Token from PayEx system");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.PayeeId", "Payee Id");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.PayeeId.Hint", "Enter yours Payee Id from PayEx system");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.PayeeName", "Payee Name");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.PayeeName.Hint", "The name of the payee, usually the name of the merchant.");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.Fee_Invoice", "Invoice Fee");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.Fee_Invoice.Hint", "The fee amount in the lowest monetary unit to apply if the consumer chooses to pay with invoice.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.Fee_CampaignInvoice", "Campaign Invoice Fee");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.Fee_CampaignInvoice.Hint", "The fee amount in the lowest monetary to apply if the consumer chooses to pay with campaign invoice.");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.Urls_completeUrl", "Payment Order Complete Url");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.Urls_completeUrl.Hint", "The URL to redirect the payer to once the payment is completed.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.Urls_cancelUrl", "Payment Order Cancel Url");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.Urls_cancelUrl.Hint", "The URL to redirect the payer to if the payment is canceled.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.Urls_callbackUrl", "Payment Order Callback Url");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.Urls_callbackUrl.Hint", "The URL to the API endpoint receiving POST requests on transaction activity related to the payment order.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.Urls_termsOfServiceUrl", "Terms Of Service Url (https only)");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.Urls_termsOfServiceUrl.Hint", "The URL to the terms of service document the payer must accept in order to complete the payment.");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_SettingsEnabled", "Credit Card Settings Enabled");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_SettingsEnabled.Hint", "");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_no3DSecure", "Do not require 3D-Secure");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_no3DSecure.Hint", "true if the payment should not require a 3D-secure authentication; otherwise false.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_mailOrderTelephoneOrder", "Mail Order Telephone Order (MOTO)");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_mailOrderTelephoneOrder.Hint", "true if _localizationService is a MOTO payment; otherwise false. ");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_rejectCardNot3DSecureEnrolled", "Reject Card Not 3D-Secure Enrolled");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_rejectCardNot3DSecureEnrolled.Hint", "true if cards not enrolled with 3D-secure should be declined; otherwise false.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_rejectDebitCards", "Reject Debit Cards");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_rejectDebitCards.Hint", "true if debit cards should be declined; otherwise  false");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_rejectCreditCards", "Reject Credit Cards");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_rejectCreditCards.Hint", "true if credit cards should be declined; otherwise  false.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_rejectConsumerCards", "Reject Consumer Cards");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_rejectConsumerCards.Hint", "true if consumer cards should be declined; otherwise false");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_rejectCorporateCards", "Reject Corporate Cards");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_rejectCorporateCards.Hint", "true if corporate cards should be declined; otherwise false");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_rejectAuthenticationStatusU", "Reject Authentication Status U");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_rejectAuthenticationStatusU.Hint", "true if response code U from issuer bank should be declined; otherwise false. Response code U means that it couldn't be determined if card was 3DSecure enrolled or not. ");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_rejectAuthenticationStatusA", "Reject Authentication Status A");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_rejectAuthenticationStatusA.Hint", "true if response code A from issuer bank should be declined; otherwise false. Response code A means that the 3DSecure service is unavailable and therefore a 3DSecure verification is not made.");


            base.Install();
        }

        public override void Uninstall()
        {
            _settingService.DeleteSetting<PayExCheckoutSettings>();

            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.PaymentMethodDescription");

            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.ApiEndPoint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.ApiEndPoint.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.Language");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.Language.Hint");

            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.MerchantToken");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.MerchantToken.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.PayeeId");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.PayeeId.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.PayeeName");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.PayeeName.Hint");

            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.Fee_Invoice");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.Fee_Invoice.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.Fee_CampaignInvoice");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.Fee_CampaignInvoice.Hint");

            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.Urls_completeUrl");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.Urls_completeUrl.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.Urls_cancelUrl");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.Urls_cancelUrl.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.Urls_callbackUrl");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.Urls_callbackUrl.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.Urls_termsOfServiceUrl");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.Urls_termsOfServiceUrl.Hint");

            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_SettingsEnabled");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_SettingsEnabled.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_no3DSecure");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_no3DSecure.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_mailOrderTelephoneOrder");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_mailOrderTelephoneOrder.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_rejectCardNot3DSecureEnrolled");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_rejectCardNot3DSecureEnrolled.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_rejectDebitCards");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_rejectDebitCards.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_rejectCreditCards");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_rejectCreditCards.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_rejectConsumerCards");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_rejectConsumerCards.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_rejectCorporateCards");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_rejectCorporateCards.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_rejectAuthenticationStatusU");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_rejectAuthenticationStatusU.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_rejectAuthenticationStatusA");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.PayExCheckout2.Fields.CreditCard_rejectAuthenticationStatusA.Hint");

            base.Uninstall();
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/PayExCheckout/Configure";
        }

        #endregion

        public bool SupportCapture => true;
        public bool SupportVoid => true;
        public bool SupportPartiallyRefund => false;
        public bool SupportRefund => false;        

        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;

        public PaymentMethodType PaymentMethodType => PaymentMethodType.Standard;

        public bool SkipPaymentInfo => false;

        public string PaymentMethodDescription => "PayEx Checkout v2";

        #region Not supported stuff
        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            throw new NotImplementedException();
        }       

        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            throw new NotImplementedException();
        }

        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            throw new NotImplementedException();
        }

        #endregion
        
        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart) => 0M;
        public bool HidePaymentMethod(IList<ShoppingCartItem> cart) => false;
        public bool CanRePostProcessPayment(Order order)
        {
            return true;
        }

        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            CapturePaymentResult result = new CapturePaymentResult();

            var order = capturePaymentRequest.Order;
            
            var customValues = _paymentService.DeserializeCustomValues(order);

            if (!customValues.ContainsKey(PaymentOrderIdCustomValueKey))
            {
                result.Errors.Add("There is no PayEx paymentOrderId assigned to this order..");
                result.NewPaymentStatus = Core.Domain.Payments.PaymentStatus.Authorized;
                return result;
            }

            string paymentOrderId = (string)customValues[PaymentOrderIdCustomValueKey];

            bool isSuccess = false;

            using (var client = new PayExClient.PayExClient(_paymentSettings.ApiEndPoint, _paymentSettings.MerchantToken))
            {
                var taxes = _orderService.ParseTaxRates(order, order.TaxRates).First();

                isSuccess = client.CapturePaymentOrderAsync(paymentOrderId, 
                                                            (int)(order.OrderTotal * 100),
                                                            (int)(order.OrderTax * 100), 
                                                            (int)(taxes.Key * 100), 
                                                            $"Capture order {order.Id}", 
                                                            $"CAPTURE_ORDERID_{order.Id}")
                                                            .Result;
            }

            if (!isSuccess)
            {
                result.Errors.Add("Payment capturing error");                
            }

            result.NewPaymentStatus = Core.Domain.Payments.PaymentStatus.Authorized;

            return result;
        }

        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            VoidPaymentResult result = new VoidPaymentResult();

            var order = voidPaymentRequest.Order;

            var customValues = _paymentService.DeserializeCustomValues(order);

            if (!customValues.ContainsKey(PaymentOrderIdCustomValueKey))
            {
                result.Errors.Add("There is no PayEx paymentOrderId assigned to this order..");
                result.NewPaymentStatus = Core.Domain.Payments.PaymentStatus.Authorized;
                return result;
            }

            string paymentOrderId = (string)customValues[PaymentOrderIdCustomValueKey];

            bool isSuccess = false;

            using (var client = new PayExClient.PayExClient(_paymentSettings.ApiEndPoint, _paymentSettings.MerchantToken))
            {
                isSuccess = client.CancelPaymentOrderAsync(paymentOrderId, $"Void order {order.Id}", $"VOID_ORDERID_{order.Id}").Result;
            }

            if (!isSuccess)
            {
                result.Errors.Add("Payment canceling error");
            }

            result.NewPaymentStatus = Core.Domain.Payments.PaymentStatus.Authorized;

            return result;
        }

        public ProcessPaymentRequest GetPaymentInfo(IFormCollection form)
        {
            return new ProcessPaymentRequest();
        }

        public string GetPublicViewComponentName()
        {
            return null;
        }

        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            //nothing to do.. maybe
        }

        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            return new ProcessPaymentResult()
            {
                AllowStoringCreditCardNumber = false,                 
                NewPaymentStatus = Core.Domain.Payments.PaymentStatus.Authorized
            };
        }

        
        public IList<string> ValidatePaymentForm(IFormCollection form)
        {
            return new List<string>();
        }


    }
}
