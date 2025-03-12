using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.Models
{
    public class ths_tasks:Entity
    {
        public string object_str { get; set; }
        public DateTime date { get; set; }
        public bool is_done { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
        public string job_id { get; set; }
    }
}
