using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.Models
{
    public class service_type_dept
    {
        public int service_type_id { get; set; }
        public string service_type_name { get; set; }
        public double without_vat { get; set; }
        public double vat { get; set; }
        public double total { get; set; }
        public double transit { get; set; }
        public double amount_total { get; set; }
        
    }
}
