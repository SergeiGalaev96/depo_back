using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.Models
{
    public class instruction_registrar_reports:Entity
    {
        public string name { get; set; }
        public string report_name { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool deleted { get; set; }
    }
}
