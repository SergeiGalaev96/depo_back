using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.Models
{
    public class depositor_dept
    {
        public string? depositor_name { get; set; }
        public double? dept_before { get; set; }
        public List<service_type_dept> service_type_depts { get; set; }
        public double? without_vat_total { get; set; }
        public double? vat { get; set; }
        public double? total { get; set; }
        public double? transit_total { get; set; }
        public double? amount_total_all { get; set; }
        public double? dept_after { get; set; }
        public double? payments_between_period { get; set; }

        public depositor_dept()
        {
            service_type_depts = new List<service_type_dept>();
        }

    }
}
