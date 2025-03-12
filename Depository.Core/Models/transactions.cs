using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class transactions:Entity
    {
        public int instruction { get; set; }
        public DateTime date { get; set; }
        public int? account { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
        public string trans_type { get; set; }
        public string stop { get; set; }
    }
}
