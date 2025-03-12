using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class depositories:Entity
    {
        public string name { get; set; }
        public int partner { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public bool? deleted { get; set; }
        public string gov_reg { get; set; }
        public string security_reg { get; set; }
        public int? country { get; set; }
        public int? city { get; set; }
        public string address { get; set; }
        public string post_address { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
        public string email { get; set; }
        public int? bank { get; set; }
        public string bank_account { get; set; }
        public int? cliring_bank { get; set; }
        public string cliring_account { get; set; }
        public int? storage_bank { get; set; }
        public int? currency_account_bank { get; set; }
        public int? currency_account_bank_city { get; set; }
        public string currency_account_bank_bic { get; set; }
        public string currency_account_bank_address { get; set; }
        public string сorr_account_of_demir_bank { get; set; }
        public int? intermediary_bank { get; set; }
        public int? intermediary_bank_city { get; set; }
        public string intermediary_bank_bic { get; set; }
        public string intermediary_bank_account { get; set; }
        public string settlement_account { get; set; }


        [JsonIgnore]
        public virtual partners partners { get; set; }

        public depositories()
        {
            partners = new partners();
        }


    }
}
