using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models.DTO
{
    public class model_currency_export_to_ExchangeDTO
    {
        public int depositor_id_from_trades { get; set; }
        public double quantity { get; set; }
        public string currency_code { get; set; }
        public string account_number { get; set; }
        
        
    }
}
