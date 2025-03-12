using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.Models
{
    public class gov_securities_payments:Entity
    {
        
        public int security { get; set; }
        public DateTime date_of_payment { get; set; }
        public double percent { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool deleted { get; set; }
    }
}
