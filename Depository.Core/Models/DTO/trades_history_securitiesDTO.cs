using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.Models.DTO
{
    public class trades_history_securitiesDTO:Entity
    {
        public string security_code { get; set; }
        public int seller_id { get; set; }
        public string seller_account { get; set; }
        public int buyer_id { get; set; }
        public string buyer_account { get; set; }
        public double quantity { get; set; }
        public double amount { get; set; }
        public bool cliring { get; set; }
        
    }
}
