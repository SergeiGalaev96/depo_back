using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.Models.DTO
{
    public class CurrencyItem
    {
        public string account_number { get; set; }

        public string currency_code { get; set; }

        public double quantity { get; set; }

       // public double quantity_stop { get; set; }
    }
}
