using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProshoreHouseBroker.Application.DTOs
{
    public class CreateCommissionRuleDto
    {
        public decimal MinAmount { get; set; }

        public decimal? MaxAmount { get; set; }

        public decimal Percentage { get; set; }

        public decimal AdminSharePercentage { get; set; }
    }
    public class UpdateCommissionRuleDto
    {
        public decimal MinAmount { get; set; }

        public decimal? MaxAmount { get; set; }

        public decimal Percentage { get; set; }

        public decimal AdminSharePercentage { get; set; }

        public bool IsActive { get; set; }
    }
}
