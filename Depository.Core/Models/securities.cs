using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class securities : Entity
    {
        public string code { get; set; }
        public int issuer { get; set; }
        public int security_type { get; set; }
        public int? exchange { get; set; }
        public int? registrar { get; set; }
        public double? nominal { get; set; }
        public int? currency { get; set; }
        public int? quantity { get; set; }
        public DateTime? reg_date { get; set; }
        public DateTime? end_date { get; set; }

        public double? dividends { get; set; }
        public string isin { get; set; }

        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
        public bool? is_gov_security { get; set; }
        public bool? gov_part { get; set; }
        public bool? blocked { get; set; }
        public DateTime? block_date { get; set; }
        public bool? send_to_trades { get; set; }
        public double payment_percent { get; set; }
        public double trading_ratio { get; set; }
        public int? location { get; set; }
        [JsonIgnore]
        public virtual security_types security_types { get; set; }
        [JsonIgnore]
        public virtual issuers issuers { get; set; }
        [JsonIgnore]
        public virtual List<instructions> instructions { get; set; }


        public securities()
        {
            security_types = new security_types();
            issuers = new issuers();
        }
    }
}
