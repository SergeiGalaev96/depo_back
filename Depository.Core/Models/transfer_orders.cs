using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public  class transfer_orders:Entity
    {
        public int? instruction_id { get; set; }
        public string registered_person { get; set; }
        public string address { get; set; }
        public string account_number { get; set; }
        public string document_type { get; set; }
        public string document_series { get; set; }
        public string document_number { get; set; }
        public DateTime? document_issue_date { get; set; }
        public string document_issue_place { get; set; }
        public string document_issue_authority { get; set; }
        public string person_registered_authority { get; set; }
        public string registration_number_date { get; set; }

        public int? quantity_int { get; set; }
        public string quantity_text { get; set; }
        public string encumbrances_on_securities { get; set; }
        public string basis { get; set; }
        public string transfer_of_rights_to_securities { get; set; }
        public int? registrar { get; set; }
        public int? security { get; set; }
        public int? issuer { get; set; }
        public string registrar_authorized_person_name { get; set; }
        public string registration_number_date_by_journal { get; set; }
        public bool? signed_by_depositor { get; set; }
        public DateTime? signed_at_depositor { get; set; }
        public string signed_depositor_inn { get; set; }
        public Guid? files_directory { get; set; }
        public Guid? user_id { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public bool? deleted { get; set; }
        public int? status { get; set; }
        public string signed_depositor_full_name { get; set; }
        public DateTime? signed_at_registrar { get; set; }
        public bool? signed_by_registrar { get; set; }
        public string signed_registrar_inn { get; set; }
        public string signed_registrar_full_name { get; set; }
        public bool? legal_entity { get; set; }
        public bool? individual { get; set; }
        public int? security_type { get; set; }
        public string authority_document_name { get; set; }
    }
}
