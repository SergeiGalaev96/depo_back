using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.Models
{
    public class limits:Entity
    {
        public int service_type { get; set; }
        public double min { get; set; }
        public double max { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }

        public bool? deleted { get; set; }

        public int? min_payment { get; set; }
        public int? min_month { get; set; }
    }
}
