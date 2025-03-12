using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.Models.DTO
{
    public class trade_history_output:Entity
    {
        public string status_code { get; set; }
        public string status_message { get; set; }
    }
}
