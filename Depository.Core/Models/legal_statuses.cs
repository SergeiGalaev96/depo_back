using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class legal_statuses:Entity
    {
        public string uid { get; set; }
        public string name { get; set; }
        public int? partner { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
    }
}
