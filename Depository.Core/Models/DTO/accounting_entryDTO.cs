using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models.DTO
{
    public class accounting_entryDTO:Entity
    {
        public string instruction_type { get; set; }
        public string operation { get; set; }
        public string accounting_position { get; set; }
        public string instruction_status { get; set; }
    }
}
