using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class depositors:Entity
    {
        public int partner { get; set; }
        public int? depositorstatus { get; set; }
        public string legastatus { get; set; }
        public string registration { get; set; }
        public string addressdata { get; set; }
        public string tradingsystem { get; set; }
        public string notes { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public bool? deleted { get; set; }
        public bool? maintaining_bank_account { get; set; }
        public bool payment_for_mba { get; set; }

        public string npf { get; set; }
        public int? clr_service { get; set; }


        [JsonIgnore]
        public virtual partners partners { get; set; }
        public virtual List<instructions> instructions { get; set; }

        public depositors()
        {
            partners = new partners();
        }
    }
}
