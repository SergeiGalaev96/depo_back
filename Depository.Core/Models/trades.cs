using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class trades:Entity
    {
        public int depositor { get; set; }
        public int trading_systems { get; set; }
        public int depositor_code { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
    }
}
