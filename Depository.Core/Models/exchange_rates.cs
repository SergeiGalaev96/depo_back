using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class exchange_rates:Entity
    {
        public int currency { get; set; }
        public DateTime date { get; set; }
        public double rate { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
    }
}
