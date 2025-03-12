using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class instruction_account_relations:Entity
    {
        public int instruction_type_id { get; set; }
        public string account_short_number { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public bool? deleted { get; set; }
    }
}
