using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models.DTO
{
    public class model_export_to_exchangeDTO
    {
        public int depositor_id_from_trades { get; set; }
        public string security_code { get; set; }
        public double quantity { get; set; }
        public string account_number { get; set; }
    }
}
