using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.Models.DTO
{
    public class charge_output_model
    {
         public int payer_id { get; set; }
         public int service_type_id { get; set; }
         public string payer_name { get; set; }
         public string service_type_name { get; set; }
         public double main_dep { get; set; }
        public double transit_dep { get; set; }
    }
}
