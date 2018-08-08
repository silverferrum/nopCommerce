using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Payments.PayExCheckout2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.PayExCheckout2.Mappings
{
    public class SettingsConfigMapping : AutoMapper.Profile, IOrderedMapperProfile
    {
        public SettingsConfigMapping()
        {
            CreateMap<PayExCheckoutSettings, ConfigurationModel>();

            CreateMap<ConfigurationModel, PayExCheckoutSettings>();
        }

        public int Order => 0;
    }
}
