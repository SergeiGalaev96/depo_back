using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models.DTO
{
    public class stock_securityDTO
    {
        public int id { get; set; }
        public string account { get; set; }
        public int? security { get; set; }
        public double quantity { get; set; }
        public double quantity_stop { get; set; }
        public int partner { get; set; }
        public int account_id { get; set; }
        public double? quantity_pledge { get; set; }
       
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }

    }
}
