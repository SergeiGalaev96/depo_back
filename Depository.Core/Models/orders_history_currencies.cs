using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class orders_history_currencies:Entity
    {
        public int depositor_id_from_trades { get; set; }
        public double quantity { get; set; }
        public string account_number { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
        public bool? processed { get; set; }
        public DateTime order_date { get; set; }
        public string currency_code { get; set; }
    }
}
