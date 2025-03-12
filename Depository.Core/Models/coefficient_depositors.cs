using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class coefficient_depositors:Entity
    {
        public int depositor { get; set; }
        public double coefficient { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
        public int? partner { get; set; }
    }
}
