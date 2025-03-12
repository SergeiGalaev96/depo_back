using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class oper_days:Entity
    {
        public DateTime date { get; set; }
        public DateTime open { get; set; }
        public DateTime close { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
    }
}
