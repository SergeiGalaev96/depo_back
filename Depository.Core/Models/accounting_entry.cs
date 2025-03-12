using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class accounting_entry:Entity
    {
        public int instruction_type { get; set; }
        public string operation { get; set; }
        public bool account_status { get; set; }
        public int? account_type { get; set; }
        public int status_old { get; set; }
        public int status_new { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
        public int? from_to { get; set; }
        public int? stop { get; set; }



        //[JsonIgnore]
        //public virtual account_types account_types { get; set; }
    }
}
