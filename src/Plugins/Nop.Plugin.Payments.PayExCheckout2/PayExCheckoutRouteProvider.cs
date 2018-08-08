using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.PayExCheckout2
{
    public class PayExCheckoutRouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routeBuilder">Route builder</param>
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute("PayExCheckout2.Redirect",
                "onepagecheckout/", new { controller = "PublicPayExCheckout", action = "Redirect" });

            routeBuilder.MapRoute("PayExCheckout2.Checkin",
                "payex/checkin/", new { controller = "PublicPayExCheckout", action = "Checkin" });

            routeBuilder.MapRoute("PayExCheckout2.Checkout",
                "payex/checkout/{consumerProfileRef?}", new { controller = "PublicPayExCheckout", action = "Checkout" });

            routeBuilder.MapRoute("PayExCheckout2.Callback",
                "payex/callback/", new { controller = "PublicPayExCheckout", action = "Callback" });

            routeBuilder.MapRoute("PayExCheckout2.OnPaymentCompleted",
                "payex/onPaymentCompleted/", new { controller = "PublicPayExCheckout", action = "OnPaymentCompleted" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority
        {
            get { return 1; }
        }
    }
}
