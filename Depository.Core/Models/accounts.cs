using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class accounts:Entity
    {
        public string accnumber { get; set; } = "";
        public int partner { get; set; }
        public int? account_type { get; set; }
        public string notes { get; set; }
        public DateTime? dateopen { get; set; }
        public DateTime? dateclosed { get; set; }
        public bool? isactive { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
        public int? acc_member { get; set; }
        public bool? isclosed { get; set; }
        public int? acc_manager { get; set; }
        public bool? istrading_account { get; set; }

        [JsonIgnore]
        public virtual partners partners { get; set; }

    }
}
