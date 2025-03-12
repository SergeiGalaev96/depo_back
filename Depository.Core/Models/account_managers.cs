using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class account_managers:Entity
    {
        public int partner { get; set; }
        public string power_of_attorney { get; set; }
        public string list_of_restrictions { get; set; }

        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
    }
}
