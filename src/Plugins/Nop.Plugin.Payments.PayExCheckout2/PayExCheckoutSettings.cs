using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.PayExCheckout2
{
    public class PayExCheckoutSettings : ISettings
    {
        internal static readonly IList<string> ApiEndPoints = new List<string>() { "https://api.stage.payex.com/", "https://api.payex.com/" };
        internal static readonly IList<string> Languages = new List<string>() { "en-US", "sv-SE", "nb-NO" };

        public string ApiEndPoint { get; set; }
        public string Language { get; set; }

        public string MerchantToken { get; set; }
        public string PayeeId { get; set; }
        public string PayeeName { get; set; }        

        public decimal Fee_Invoice { get; set; }
        public decimal Fee_CampaignInvoice { get; set; }

        public string Urls_completeUrl { get; set; }
        public string Urls_cancelUrl { get; set; }
        public string Urls_callbackUrl { get; set; }
        public string Urls_termsOfServiceUrl { get; set; }

        public bool CreditCard_SettingsEnabled { get; set; }
        public bool CreditCard_no3DSecure { get; set; }
        public bool CreditCard_mailOrderTelephoneOrder { get; set; }
        public bool CreditCard_rejectCardNot3DSecureEnrolled { get; set; }
        public bool CreditCard_rejectDebitCards { get; set; }
        public bool CreditCard_rejectCreditCards { get; set; }
        public bool CreditCard_rejectConsumerCards { get; set; }
        public bool CreditCard_rejectCorporateCards { get; set; }
        public bool CreditCard_rejectAuthenticationStatusU { get; set; }
        public bool CreditCard_rejectAuthenticationStatusA { get; set; }

    }
}
