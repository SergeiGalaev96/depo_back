using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.Models
{
    public class instruction_types_gik:Entity
    {
        public string name { get; set; }

        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool deleted { get; set; }
        public Guid? create_form { get; set; }
        public Guid? edit_form { get; set; }
    }
}
