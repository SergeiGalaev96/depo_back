using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class partners:Entity
    {
        public string name { get; set; }
        public int? partner_type { get; set; }
        public int? legal_status { get; set; }
        public string bank_account { get; set; }
        public string address { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
        public string gov_reg { get; set; }
        public double? capital { get; set; }
        public int? currency { get; set; }
        public int? city { get; set; }
        public int? region { get; set; }
        public int? country { get; set; }
        public string post_address { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
        public string email { get; set; }
        public int? bank { get; set; }
        public string inn { get; set; }
        public string rni { get; set; }

        public string cliring_account { get; set; }
        public int? cliring_bank { get; set; }
        public string storage_account { get; set; }
        public int? storage_bank { get; set; }
        public string security_reg { get; set; }
        public bool? is_resident { get; set; }
        public string corr_depository_account { get; set; }
        public string main_depository_account { get; set; }
        public bool? authorized { get; set; }
        public bool is_active { get; set; }
        public double? sales_tax_amount { get; set; }

       // public bool vat { get; set; }
        public bool? sales_tax { get; set; }
        public bool? vat { get; set; }
        public double? vat_amount { get; set; }

        [JsonIgnore]
        public virtual List<accounts> accounts { get; set; }

        public virtual List<depositors> depositors { get; set; }
        public virtual List<depositories> depositories { get; set; }
        public virtual List<corr_depositories> corr_depositories { get; set; }
        public virtual List<registrars> registrars { get; set; }
        public virtual List<partner_contacts> partner_contacts { get; set; }
    }
}
