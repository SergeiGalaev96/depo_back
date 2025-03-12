using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class instruction_types_user_permissions:Entity
    {
        public int instruction_id { get; set; } 
        public string name_in_keycloak { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
    }
}
