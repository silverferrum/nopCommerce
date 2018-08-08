using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.PayExCheckout2.Models
{
    public class CallbackModel
    {
        public Callback_PaymentOrder PaymentOrder { get; set; }
        public Callback_Payment Payment { get; set; }
        public Callback_Transaction Transaction { get; set; }

        public class Callback_PaymentOrder
        {
            public string id { get; set; }
            public string instrument { get; set; }
        }

        public class Callback_Payment
        {
            public string id { get; set; }
            public long number { get; set; }
        }

        public class Callback_Transaction
        {
            public string id { get; set; }
            public long number { get; set; }
        }
    }

}
