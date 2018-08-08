using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.PayExCheckout2.PayExClient.Models
{
    public class CheckoutResponseModel
    {
        public CheckoutResponse_PaymentOrder PaymentOrder { get; set; }
        public List<CheckoutResponse_Operation> Operations { get; set; }

        public class CheckoutResponse_PaymentOrder
        {
            public string Id { get; set; }           
        }

        public class CheckoutResponse_Operation
        {
            public string Rel { get; set; }
            public string Method { get; set; }
            public string ContentType { get; set; }
            public string Href { get; set; }
        }

        public CheckoutResponse_Operation GetViewPaymentOrderOperation()
        {
            return Operations.Find(x => x.Rel == "view-paymentorder");
        }
    }






}
