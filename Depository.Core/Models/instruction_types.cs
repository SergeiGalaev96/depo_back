using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class instruction_types : Entity
    {
        public string code { get; set; }
        public string name { get; set; }
        public int? it_base{get;set;}
        public int? account_from { get; set; }
        public int? account_to { get; set; }
        public int? owner { get; set; }
        public int? depositor { get; set; }
        public int? trade { get; set; }
        public int? corr { get; set; }
        public int? web { get; set; }
        public int? clr { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public bool? deleted { get; set; }
        public int? report { get; set; }
        public Guid? create_form { get; set; }
        public Guid? edit_form { get; set; }
        public int? blocking { get; set; }
        public bool? transfer_order { get; set; }
        public int? sector { get; set; }
        public bool? instruction_registrar { get; set; }
        public int? instruction_registrar_report { get; set; }

        public virtual List<instructions> instructions { get; set; }
    }
}
