using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models.DTO
{
    public class depositorsDTO:Entity
    {
        public int partner { get; set; }
        public string name { get; set; }

        public string address { get; set; }
        public string bank_account { get; set; }
        public string gov_reg { get; set; }
        public double? capital { get; set; }
        public string currency { get; set; }
        public int? city { get; set; }
        public int? region { get; set; }
        public int? country { get; set; }
        public string post_address { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
        public string email { get; set; }
        public string bank { get; set; }
        public string inn { get; set; }
        public string rni { get; set; }
        public string cliring_account { get; set; }
        public string cliring_bank { get; set; }
        public string storage_account { get; set; }
        public string storage_bank { get; set; }
        public string security_reg { get; set; }
        public bool vat { get; set; }
        public bool sales_tax { get; set; }

        public string npf { get; set; }
        public int? clr_service { get; set; }
    }
}
