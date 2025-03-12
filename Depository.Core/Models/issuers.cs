using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class issuers:Entity
    {
        public string registration { get; set; }
       // public int partner { get; set; }
        public int? registrar { get; set; }
       // public string notes { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
        public double? capital { get; set; }
        public string name { get; set; }
        public bool? is_resident { get; set; }
        public string gov_registration { get; set; }
        public int? currency { get; set; }
        public int? city { get; set; }
        public string address { get; set; }
        public string post_address { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
        public string email { get;set; }
        public int? bank { get; set; }
        public string account { get; set; }
        public string inn { get; set; }
        public string tax_registration { get; set; }
        public bool? is_not_resident { get; set; }
        public int? country { get; set; }
        public bool? blocked { get; set; }
        public DateTime? block_date { get; set; }
        public bool is_active { get; set; }

        [JsonIgnore]
        public virtual registrars registrars { get; set; }
        [JsonIgnore]
        public virtual List<instructions> instructions { get; set; }

        [JsonIgnore]
        public virtual List<securities> securities { get; set; }

        public issuers()
        {
            registrars = new registrars();
        }
    }
}
