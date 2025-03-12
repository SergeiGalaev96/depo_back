using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class currencies:Entity
    {
        public string code { get; set; }
        public string name { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string first { get; set; }
        public string last { get; set; }
        public string status { get; set; }
        public bool? deleted { get; set; }
        public virtual List<instructions> instructions { get; set; }
    }
}
