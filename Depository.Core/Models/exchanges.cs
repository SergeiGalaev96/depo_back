using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class exchanges:Entity
    {
        public string name { get; set; }
        public int partner { get; set; }
        public int currencies { get; set; }
        public string notes { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
    }
}
