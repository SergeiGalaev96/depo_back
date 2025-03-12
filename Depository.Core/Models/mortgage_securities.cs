using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class mortgage_securities:Entity
    {
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
    }
}
