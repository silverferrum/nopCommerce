using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.PayExCheckout2.PayExClient.Models
{
    public class CheckinResponseModel
    {
        public string Token { get; set; }
        public List<CheckinResponse_Operation> Operations { get; set; }

        public class CheckinResponse_Operation
        {
            public string Rel { get; set; }
            public string Method { get; set; }
            public string ContentType { get; set; }
            public string Href { get; set; }
        }

        public CheckinResponse_Operation GetViewConsumerIdentificationOperation()
        {
            return Operations.Find(x => x.Rel == "view-consumer-identification");
        }
    }
}
