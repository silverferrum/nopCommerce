using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.PayExCheckout2.Models
{
    public class CheckinModel
    {
        public string ScriptHref { get; set; }
        public Customer CurrentUser { get; set; }
        public string Language { get; set; }
    }
}
