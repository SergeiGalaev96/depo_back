using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class history:Entity
    {
        public int event_id{ get;set; }
        public int object_id { get; set; }
        public string object_str { get; set; }
        public string object_name { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
        public int user_id { get; set; }
    }
}
