using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.Models
{
    public class new_security_application_types:Entity
    {
        public string name { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public bool? deleted { get; set; }
    }
}
