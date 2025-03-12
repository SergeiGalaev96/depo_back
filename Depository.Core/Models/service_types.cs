using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class service_types:Entity
    {
        public string name { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
        public int service_group { get; set; }
        public int? instruction_type { get; set; }
        public int payer { get; set; }
        public bool? vat { get; set; }
        public bool sales_tax { get; set; }
        public int? index { get; set; }
        public double? limit_min { get; set; }
        public double? limit_max { get; set; }

        public bool is_urgent { get; set; }
        public int?  taxtype_vat_id { get; set; }
        public int? taxtype_salestax_id { get; set; }

        public string code { get; set; }
    }
}
