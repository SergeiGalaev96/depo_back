using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models.DTO
{
    public class stock_currencyDTO
    {
        public int id { get; set; }
        public string account { get; set; }
        public string currency { get; set; }
        public double quantity { get; set; }
        public double quantity_stop { get; set; }
        public int partner { get; set; }
        public int account_id { get; set; }
        public double? quantity_pledge { get; set; }
    }
}
