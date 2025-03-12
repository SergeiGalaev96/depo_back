using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.Models
{
    public class new_security_applications:Entity
    {
        public int security_type { get; set; }
        public string code { get; set; }
        public int application_type { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public bool? deleted { get; set; }

        public bool? is_gov_security { get; set; }
        public string depositor { get; set; }
        public int? securities_quantity_int { get; set; }
        public string securities_quantity_text { get; set; }
        public DateTime? securities_placement_begin_date { get; set; }
        public DateTime? securities_placement_end_date { get; set; }
        public DateTime? authorized_state_decision_issue_date { get; set; }
        public string issuer_address { get; set; }
        public int? security_issue_form { get; set; }
        public double? nominal { get; set; }
        public string keeper_register_securities_owners { get; set; }
        public string date_number_issuer_gov_registration { get; set; }
        public string depositor_authorized_person { get; set; }
        public Guid? files_directory { get; set; }
        public string issuer { get; set; }
        public string es_inn { get; set; }
        public string es_full_name { get; set; }
        public DateTime? es_date { get; set; }
        public int? status { get; set; }
        public string security_type_text { get; set; }
        public string isin { get; set; }
        public int? currency { get; set; }
    }
}
