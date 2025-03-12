using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.Models
{
    public class security_balance:Entity
    {
        public int account { get; set; }
        public int security { get; set; }
        public DateTime date_begin { get; set; }
        public double quantity { get; set; }
        public double quantity_stop { get; set; }

        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
    }
}
