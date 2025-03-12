using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.Models
{
    public class vats:Entity
    {
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool deleted { get; set; }

        public int payment_type { get; set; }
        public double percent { get; set; }
        public string name { get; set; }
        public double value { get; set; }

    }
}
