using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Payments.PayExCheckout2.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }


        [NopResourceDisplayName("Plugins.Payments.PayExCheckout2.Fields.ApiEndPoint")]
        public SelectList ApiEndPointValues { get; set; }
        [Required]
        public string ApiEndPoint { get; set; }
        public bool ApiEndPoint_OverrideForStore { get; set; }


        [NopResourceDisplayName("Plugins.Payments.PayExCheckout2.Fields.Language")]
        public SelectList LanguageValues { get; set; }
        [Required]
        public string Language { get; set; }
        public bool Language_OverrideForStore { get; set; }

        #region MerchantToken & Payee
        [NopResourceDisplayName("Plugins.Payments.PayExCheckout2.Fields.MerchantToken")]
        [Required]
        public string MerchantToken { get; set; }
        public bool MerchantToken_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayExCheckout2.Fields.PayeeId")]
        [Required]
        public string PayeeId { get; set; }
        public bool PayeeId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayExCheckout2.Fields.PayeeName")]
        [Required]
        public string PayeeName { get; set; }
        public bool PayeeName_OverrideForStore { get; set; }
        #endregion

        #region Fees
        [NopResourceDisplayName("Plugins.Payments.PayExCheckout2.Fields.Fee_Invoice")]
        public decimal Fee_Invoice { get; set; }
        public bool Fee_Invoice_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayExCheckout2.Fields.Fee_CampaignInvoice")]
        public decimal Fee_CampaignInvoice { get; set; }
        public bool Fee_CampaignInvoice_OverrideForStore { get; set; }
        #endregion

        #region Urls
        [NopResourceDisplayName("Plugins.Payments.PayExCheckout2.Fields.Urls_completeUrl")]
        public string Urls_completeUrl { get; set; }
        public bool Urls_completeUrl_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayExCheckout2.Fields.Urls_cancelUrl")]
        [Required]
        public string Urls_cancelUrl { get; set; }
        public bool Urls_cancelUrl_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayExCheckout2.Fields.Urls_callbackUrl")]
        [AutoMapper.IgnoreMap]
        public string Urls_callbackUrl { get; set; }
        public bool Urls_callbackUrl_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayExCheckout2.Fields.Urls_termsOfServiceUrl")]
        [Required]
        public string Urls_termsOfServiceUrl { get; set; }
        public bool Urls_termsOfServiceUrl_OverrideForStore { get; set; }
        #endregion

        #region Credit Card restrictinos
        
        [NopResourceDisplayName("Plugins.Payments.PayExCheckout2.Fields.CreditCard_SettingsEnabled")]
        public bool CreditCard_SettingsEnabled { get; set; }
        public bool CreditCard_SettingsEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayExCheckout2.Fields.CreditCard_no3DSecure")]
        public bool CreditCard_no3DSecure { get; set; }
        public bool CreditCard_no3DSecure_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayExCheckout2.Fields.CreditCard_mailOrderTelephoneOrder")]
        public bool CreditCard_mailOrderTelephoneOrder { get; set; }
        public bool CreditCard_mailOrderTelephoneOrder_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayExCheckout2.Fields.CreditCard_rejectCardNot3DSecureEnrolled")]
        public bool CreditCard_rejectCardNot3DSecureEnrolled { get; set; }
        public bool CreditCard_rejectCardNot3DSecureEnrolled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayExCheckout2.Fields.CreditCard_rejectDebitCards")]
        public bool CreditCard_rejectDebitCards { get; set; }
        public bool CreditCard_rejectDebitCards_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayExCheckout2.Fields.CreditCard_rejectCreditCards")]
        public bool CreditCard_rejectCreditCards { get; set; }
        public bool CreditCard_rejectCreditCards_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayExCheckout2.Fields.CreditCard_rejectConsumerCards")]
        public bool CreditCard_rejectConsumerCards { get; set; }
        public bool CreditCard_rejectConsumerCards_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayExCheckout2.Fields.CreditCard_rejectCorporateCards")]
        public bool CreditCard_rejectCorporateCards { get; set; }
        public bool CreditCard_rejectCorporateCards_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayExCheckout2.Fields.CreditCard_rejectAuthenticationStatusU")]
        public bool CreditCard_rejectAuthenticationStatusU { get; set; }
        public bool CreditCard_rejectAuthenticationStatusU_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayExCheckout2.Fields.CreditCard_rejectAuthenticationStatusA")]
        public bool CreditCard_rejectAuthenticationStatusA { get; set; }
        public bool CreditCard_rejectAuthenticationStatusA_OverrideForStore { get; set; }
        #endregion
    }
}
