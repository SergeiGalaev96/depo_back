using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class trades_history_currencies:Entity
    {
        public int trade_id { get; set; }
        public int seller_id { get; set; }
        public string seller_account { get; set; }
        public int buyer_id { get; set; }
        public string buyer_account { get; set; }
        public double amount { get; set; }
        public int cliring { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
    }
}
