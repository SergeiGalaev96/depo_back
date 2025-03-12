using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.Models.DTO
{
    public class report_security_model
    {
        public int depositor_id { get; set; }
        public string depositor_name { get; set; }
        public int service_type_id { get; set; }
        public string service_type_name { get; set; }
        public int service_group_id { get; set; }
        public string service_group_name { get; set; }
        public string security_name { get; set; }
        public double service_main { get; set; }
        public double service_transit { get; set; }
        public double service_total { get; set; }

    }
}
