using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class instructions:Entity
    {

        public int? type { get; set; }
        public int? security { get; set; }
        public int? depositor { get; set; }

        public double? quantity { get; set; }
        public string basis { get; set; }
        public DateTime? executedDate { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        
        public bool? deleted { get; set; }
        public int? accFrom { get; set; }
        public int? accTo { get; set; }
        public bool? filled { get; set; }
        public string filledUser { get; set; }
        public bool? onExecution { get; set; }
        public string onExecutionUser { get; set; }
        public bool? executed { get; set; }
        public string executedUser { get; set; }
        public bool? canceled { get; set; }
        public string canceledUser { get; set; }
        public string cancelationReason { get; set; }
        
        public string ownerName { get; set; }
        public int? corrDepository { get; set; }
        public int? tradingSystem { get; set; }
        public int? issuer { get; set; }
        public string ownerAddress { get; set; }
        public string ownerDocument { get; set; }
        public bool? opened { get; set; }
        public int? currency { get; set; }

        public bool? signed { get; set; }
        public DateTime? signed_at { get; set; }
        public string inn { get; set; }
        public int? depositor2 { get; set; }
        public bool? urgent { get; set; }
        public DateTime? onExecutionDate { get; set; }
        public DateTime? canceledDate { get; set; }
        public string signed_user_full_name { get; set; }
        public string signed_depositor_inn { get; set; }
        public DateTime? signed_at_depositor { get; set; }
        public string signed_depositor_full_name { get; set; }
        public string signed_cd_inn { get; set; }
        public DateTime? signed_at_cd { get; set; }
        public string signed_cd_full_name { get; set; }
        public string payment_recipient { get; set; }
        public string payment_bank { get; set; }
        public string payment_account { get; set; }
        public string payment_bik { get; set; }
        public string payment_purpose { get; set; }
        public DateTime? filled_at { get; set; }
        public string additional_information { get; set; }
        public int? created_user_partner { get; set; }

        [JsonIgnore]
        public virtual issuers issuers { get; set; }
        [JsonIgnore]
        public virtual depositors depositors { get; set; }
        [JsonIgnore]
        public virtual securities securities { get; set; }
        [JsonIgnore]
        public virtual currencies currencies { get; set; }
        [JsonIgnore]
        public virtual instruction_types instruction_types { get; set; }

        //  public virtual List<accounting_entry> accounting_entries { get; set; }

        //public instructions()
        //{
        //    issuers = new issuers();
        //    depositors = new depositors();
        //    securities = new securities();
        //}
    }
}
