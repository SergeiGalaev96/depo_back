using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class users:Entity
    {
        public Guid user_id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string attributes { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
    }
}
