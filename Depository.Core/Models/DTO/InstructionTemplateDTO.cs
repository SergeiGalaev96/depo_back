using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models.DTO
{
    public class InstructionTemplateDTO
    {
        public string number { get; set; }
        public DateTime? executed_date { get; set; }
        public DateTime? created_at { get; set; }
        public string depositor_name { get; set; }
        public string depositor_name2 { get; set; }
        public string accountTo { get; set; }
        public string accountFrom { get; set; }
        public string issuer_name { get; set; }
        public string base_type_name { get; set; }
        public string base_code { get; set; }
        public double? base_count { get; set; }
        public string base_count_in_words { get; set; }
        public string basis { get; set; }
        public string owner_name { get; set; }
        public string owner_address { get; set; }
        public string owner_document { get; set; }
        public int instruction_type { get; set; }
        public string bank_name { get; set; }
        public string bank_bik { get; set; }
        public string bank_address { get; set; }
        public string currency_name { get; set; }
        public string payment_recipient { get; set; }
        public string payment_bank { get; set; }
        public string payment_account { get; set; }
        public string payment_bik { get; set; }
        public string payment_purpose { get; set; }
        
        //public string depositor2_name { get; set; }
        public string filled_user_name { get; set; }
        public string executed_user_name { get; set; }
        public string canceled_user_name { get; set; }
        public string signed_user_name { get; set; }
        public bool? canceled { get; set; }
        public bool? executed { get; set; }
        public bool? onExecution { get; set; }
        public string onExecutionUser { get; set; }
        public string cancelationReason { get; set; }
        public string signed_cd_full_name { get; set; }
        public string signed_cd_inn { get; set; }
        public string signed_depositor_full_name { get; set; }
        public string signed_depositor_inn { get; set; }

    }
}
