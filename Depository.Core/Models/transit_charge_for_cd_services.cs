using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public  class transit_charge_for_cd_services:Entity
    {
        public DateTime date { get; set; }
        public int service_type { get; set; }
        public int? registrar { get; set; }
        public int? corr_depository { get; set; }
        public int recipient_type { get; set; }
        public double quantity { get; set; }
        public int currency { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
    }
}
