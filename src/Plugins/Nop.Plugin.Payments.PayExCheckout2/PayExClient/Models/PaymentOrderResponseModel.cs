using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.PayExCheckout2.PayExClient.Models
{

    public class PaymentOrderResponseModel
    {
        public PaymentOrderResponse_PaymentOrder PaymentOrder { get; set; }
        public List<PaymentOrderResponse_Operation> Operations { get; set; }

        public class PaymentOrderResponse_PaymentOrder
        {
            public string Id { get; set; }
            public DateTime Created { get; set; }
            public DateTime Updated { get; set; }
            public string Operation { get; set; }
            public string State { get; set; }
            public string Currency { get; set; }
            public int Amount { get; set; }
            public int VatAmount { get; set; }
            public int RemainingCaptureAmount { get; set; }
            public int RemainingCancellationAmount { get; set; }
            public int RemainingReversalAmount { get; set; }
            public string Description { get; set; }
            //public string InitiatingSystemUserAgent { get; set; }
            //public string UserAgent { get; set; }
            public string Language { get; set; }
            //public PaymentOrderResponse_Urls Urls { get; set; }
            public PaymentOrderResponse_PayeeInfo PayeeInfo { get; set; }
            public PaymentOrderResponse_Payer Payer { get; set; }
            public PaymentOrderResponse_Payments Payments { get; set; }
            public MetadataCommonModel Metadata { get; set; }

            public class PaymentOrderResponse_Urls
            {
                public string Id { get; set; }
            }
            public class PaymentOrderResponse_PayeeInfo
            {
                public string Id { get; set; }
                public string PayeeId { get; set; }
                public string PayeeReference { get; set; }
                public string PayeeName { get; set; }
            }
            public class PaymentOrderResponse_Payer
            {
                public string Id { get; set; }
                public string Email { get; set; }
                public string Msisdn { get; set; }
                public PaymentOrderResponse_ShippingAddress ShippingAddress { get; set; }

                public class PaymentOrderResponse_ShippingAddress
                {
                    public string Addressee { get; set; }
                    public string CoAddress { get; set; }
                    public string StreetAddress { get; set; }
                    public string ZipCode { get; set; }
                    public string City { get; set; }
                    public string CountryCode { get; set; }
                }
            }
            public class PaymentOrderResponse_Payments
            {
                public string Id { get; set; }
                public IList<PaymentOrderResponse_PaymentList> PaymentList { get; set; }

                public class PaymentOrderResponse_PaymentList
                {
                    public string Id { get; set; }
                    public string Instrument { get; set; }
                    public DateTime Created { get; set; }
                }
            }
        }

        public class PaymentOrderResponse_Operation
        {
            public string Method { get; set; }
            public string Href { get; set; }
            public string Rel { get; set; }
            public string ContentType { get; set; }
        }

        public PaymentOrderResponse_Operation GetPaymentOrderCaptureOperation()
        {
            return Operations.Find(x => x.Rel.Equals("create-paymentorder-capture", StringComparison.OrdinalIgnoreCase));
        }

        public PaymentOrderResponse_Operation GetPaymentOrderCancelOperation()  //void operation
        {
            return Operations.Find(x => x.Rel.Equals("create-paymentorder-cancel", StringComparison.OrdinalIgnoreCase));
        }

        //public bool IsAuthorized()
        //{
        //    return CanCapture() && CanCancel();
        //}

        //public bool IsCaptured()
        //{

        //}

        //public bool IsCanceled()
        //{
        //    return !CanCapture() && !CanCancel() && !CanRefund();
        //}

        public bool CanCapture()
        {
            return PaymentOrder.RemainingCaptureAmount > 0;
        }

        public bool CanCancel()
        {
            return PaymentOrder.RemainingCancellationAmount > 0;
        }

        private bool CanRefund()
        {
            return PaymentOrder.RemainingReversalAmount > 0;
        }
    }





}
