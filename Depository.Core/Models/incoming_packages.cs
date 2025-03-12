using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.Models
{
    public class incoming_packages:Entity
    {

        public int sector_id { get; set; }
        public string trd_str { get; set; }
        public string ord_str { get; set; }
        public string ordds_str { get; set; }
        public string ord_transfer_result { get; set; }
        public string trd_transfer_result { get; set; }
        public string ordds_transfer_result { get; set; }
        public bool processed { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
    }
}
