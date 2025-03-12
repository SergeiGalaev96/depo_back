using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.Models.DTO
{
    public class orders_history_currencies_output : Entity
    {
        public string account { get; set; }
        public double quantity { get; set; }
        public string currency { get; set; }
        public string status_message { get; set; }
    }
}
