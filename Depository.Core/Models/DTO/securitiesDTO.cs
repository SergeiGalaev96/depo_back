using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models.DTO
{
    public class securitiesDTO
    {
        public int id { get; set; }
        public string code { get; set; }
        public int issuer { get; set; }
        public string issuer_name { get; set; }
        public int security_type { get; set; }
        public int? registrar { get; set; }
        public string registrar_name { get; set; }
        public double? nominal { get; set; }
        public int? currency { get; set; }
        public int? quantity { get; set; }
        public DateTime? reg_date { get; set; }
        public DateTime? end_date { get; set; }

        public double? dividends { get; set; }
        public string isin { get; set; }
        public string security_type_name { get; set; }
        public int? exchange { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
        public bool? is_gov_security { get; set; }
        public bool? gov_part { get; set; }
        public bool? blocked { get; set; }
        public DateTime? block_date { get; set; }
        public bool? send_to_trades { get; set; }
        public int? location { get; set; }
    }
}
