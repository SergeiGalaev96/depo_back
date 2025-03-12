using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.Models
{
    public class outgoing_packages:Entity
    {
        public string pos_str { get; set; }
        public string posds_str { get; set; }
        public string pos_result { get; set; }
        public string posds_result { get; set; }
        public int sector_id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }

    }
}
