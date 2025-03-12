using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class stock_security : Entity
    {
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
        public int account { get; set; }
        public int security { get; set; }
        public double quantity { get; set; }
        public double quantity_stop { get; set; }
        public double? quantity_pledge { get; set; }
    }
}
