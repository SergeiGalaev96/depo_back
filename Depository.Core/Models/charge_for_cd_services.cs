using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class charge_for_cd_services:Entity
    {
        public DateTime date { get; set; }
        public int service_type { get; set; }
        public int depositor { get; set; }
        public double main_quantity { get; set; }
        public double transit_quantity { get; set; }
        public int currency { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
    }
}
