using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models.DTO
{
    public class instructionDTO
    {
        public string number { get; set; }
        public DateTime? executed_date { get; set; }
        public DateTime? created_at { get; set; }
        public string depositor_name { get; set; }
        public int? depositor_id { get; set; }

        public string accountTo { get; set; }
        public int? accountTo_id { get; set; }
        public string accountFrom { get; set; }
        public int? accountFrom_id { get; set; }
        public string issuer_name { get; set; }
        public int? issuer_id { get; set; }
        public string base_type_name { get; set; }
        public string base_code { get; set; }
        public double? base_count { get; set; }
        public string type_name { get; set; }
        public int type_id { get; set; }
    }
}
