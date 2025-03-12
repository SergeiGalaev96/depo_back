using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.Models
{
    public class mail_types:Entity
    {
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public bool? deleted { get; set; }
        public string name { get; set; }

    }
}
