using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.PayExCheckout2.PayExClient.Models
{
    public class CheckoutRequestModel
    {
        public CheckoutRequestModel(
            Guid orderGuid,
            int amount, 
            int vatAmount, 
            string currency, 
            string description, 
            string userAgent, 
            string language, 
            
            string payeeId, 
            string payeeReference, 
            
            string consumerProfileRef)
        {
            PaymentOrder = new CheckoutRequest_PaymentOrder
            {
                Operation = "Purchase",
                Currency = currency,
                Amount = amount,
                VatAmount = vatAmount,
                Description = description,
                UserAgent = userAgent,
                Language = language,

                Urls = new CheckoutRequest_PaymentOrder.PaymentOrder_Urls(),
                PayeeInfo = new CheckoutRequest_PaymentOrder.PaymentOrder_PayeeInfo(payeeId, payeeReference),
                Payer = new CheckoutRequest_PaymentOrder.PaymentOrder_Payer(consumerProfileRef),
                Metadata = new MetadataCommonModel { OrderGuid = orderGuid.ToString() }
            };
        }

        public CheckoutRequest_PaymentOrder PaymentOrder { get; set; }

        public class CheckoutRequest_PaymentOrder
        {
            public string Operation { get; set; }
            public string Currency { get; set; }
            public int Amount { get; set; }
            public int VatAmount { get; set; }
            public string Description { get; set; }
            public string UserAgent { get; set; }
            public string Language { get; set; }
            public PaymentOrder_Urls Urls { get; set; }
            public PaymentOrder_PayeeInfo PayeeInfo { get; set; }
            public PaymentOrder_Payer Payer { get; set; }
            public MetadataCommonModel Metadata { get; set; }
            public IList<PaymentOrder_Item> Items { get; set; }

            public class PaymentOrder_Urls
            {
                public IList<string> HostUrls { get; set; }
                public string CompleteUrl { get; set; }
                public string CancelUrl { get; set; }
                public string CallbackUrl { get; set; }
                public string TermsOfServiceUrl { get; set; }
                public string LogoUrl { get; set; }
            }

            public class PaymentOrder_PayeeInfo
            {
                public PaymentOrder_PayeeInfo(string payeeId, string payeeReference)
                {
                    PayeeId = payeeId;
                    PayeeReference = payeeReference;
                }

                public string PayeeId { get; private set; }
                public string PayeeReference { get; private set; }
                public string PayeeName { get; set; }
                public string ProductCategory { get; set; }
            }

            public class PaymentOrder_Payer
            {
                public PaymentOrder_Payer(string consumerProfileRef)
                {
                    ConsumerProfileRef = consumerProfileRef;
                }

                public string ConsumerProfileRef { get; private set; }
            }

            public class PaymentOrder_Item
            {
                public Item_CreditCard CreditCard { get; set; }
                public Item_Invoice Invoice { get; set; }
                public Item_CampaignInvoice CampaignInvoice { get; set; }
                public Item_Swish Swish { get; set; }

                public class Item_CreditCard
                {
                    public bool No3DSecure { get; set; }
                    public bool No3DSecureForStoredCard { get; set; }
                    public bool RejectCardNot3DSecureEnrolled { get; set; }
                    public bool RejectCreditCards { get; set; }
                    public bool RejectDebitCards { get; set; }
                    public bool RejectConsumerCards { get; set; }
                    public bool RejectCorporateCards { get; set; }
                    public bool RejectAuthenticationStatusA { get; set; }
                    public bool RejectAuthenticationStatusU { get; set; }
                }

                public class Item_Invoice
                {
                    public int FeeAmount { get; set; }
                }

                public class Item_CampaignInvoice
                {
                    public string CampaignCode { get; set; }
                    public int FeeAmount { get; set; }
                }

                public class Item_Swish
                {
                    public bool EnableEcomOnly { get; set; }
                }
            }
        }
    }






}
