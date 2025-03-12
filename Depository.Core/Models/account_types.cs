using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class account_types:Entity
    {
        public int code { get; set; }
        public string name { get; set; }
        public string astatus { get; set; }
        public string amember { get; set; }
        public string abase { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int? account { get; set; }
        public bool? deleted { get; set; }
        public bool show_in_positions { get; set; }


    }
}
