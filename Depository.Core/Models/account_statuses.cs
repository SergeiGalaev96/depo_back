using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class account_statuses:Entity
    {
        public string name { get; set; }
        public string letter { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
    }
}
