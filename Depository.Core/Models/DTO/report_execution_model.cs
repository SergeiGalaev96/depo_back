using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.Models.DTO
{

    public class report_execution_model
    {
        public string status { get; set; }
        public string requestId { get; set; }
        public string reportURI { get; set; }
        public Export[] exports { get; set; }
    }

    public class Export
    {
        public string status { get; set; }
        public string id { get; set; }
    }
}
