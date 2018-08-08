using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.PayExCheckout2.PayExClient.Models
{
    public class CheckinRequestModel
    {
        public CheckinRequestModel()
        {
            Operation = "initiate-consumer-session";
        }
        public string Operation { get; set; }
        public string Msisdn { get; set; }
        public string Email { get; set; }
        public string ConsumerCountryCode { get; set; }
        public CheckinRequest_NationalIdentifier NationalIdentifier { get; set; }

        public class CheckinRequest_NationalIdentifier
        {
            public string SocialSecurityNumber { get; set; }
            public string CountryCode { get; set; }
        }

    }
}
