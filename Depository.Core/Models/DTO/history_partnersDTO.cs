using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models.DTO
{
    public class history_partnersDTO
    {
        public int change_type { get; set; }
        public string name { get; set; }
        public string partner_type_name { get; set; }
        public string legal_status_name { get; set; }
        public string bank_account { get; set; }
        public string address { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
        public string gov_reg { get; set; }
        public double? capital { get; set; }
        public string currency_name { get; set; }
        public string city_name { get; set; }
        public string region_name { get; set; }
        public string country_name { get; set; }
        public string post_address { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
        public string email { get; set; }
        public string bank_name { get; set; }
        public string inn { get; set; }
        public string rni { get; set; }

        public string cliring_account { get; set; }
        public string cliring_bank_name { get; set; }
        public string storage_account { get; set; }
        public string storage_bank_name { get; set; }
        public string security_reg { get; set; }
    }
}
