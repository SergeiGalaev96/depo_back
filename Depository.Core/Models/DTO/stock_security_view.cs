using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.Models.DTO
{
    public class stock_security_view:Entity
    {
        public int account_id { get; set; }
        public int security { get; set; }
        public double quantity { get; set; }
        public double quantity_stop { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
        public double? quantity_pledge { get; set; }
        public int account_type { get; set; }
        public bool show_in_positions { get; set; }
        public string account { get; set; }
        public int partner { get; set; }
    }
}
