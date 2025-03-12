using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class questinaries_gik : Entity
    {
        public string acc_number { get; set; }
        public DateTime? open_date { get; set; }

        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }

        public string official_name { get; set; }
        public string short_name { get; set; }
        public string legal_form { get; set; }
        public string gov_reg { get; set; }
        public string gov_reg_number { get; set; }
        public DateTime? reg_date { get; set; }
        public string reg_authority_name { get; set; }
        public DateTime? initial_reg_date { get; set; }
        public string okpo_code { get; set; }
        public string pin { get; set; }
        public string post_address { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
        public string email { get; set; }
        public string contact_person { get; set; }
        public int depositor_type { get; set; }
        public bool participant_on_securities_market { get; set; }
        public string license_number { get; set; }
        public DateTime? license_reg_date { get; set; }
        public string license_reg_authority { get; set; }
        public string? recipient { get; set; }
        public bool? resident { get; set; }
        public string recipient_pin { get; set; }
        public string bank_account { get; set; }
        public string bank_name { get; set; }
        public string bank_city { get; set; }
        public string corr_account { get; set; }
        public string bik { get; set; }
        public string order_letter { get; set; }
        public string order_fax { get; set; }
        public string order_person { get; set; }
        public string additional_information { get; set; }
        public string officials_position { get; set; }
        public string officials_full_name { get; set; }
        public string authorized_position { get; set; }
        public string authorized_full_name { get; set; }
        public string location { get; set; }
    }
}
