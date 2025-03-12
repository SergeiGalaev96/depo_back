using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class directory:Entity
    {
        public string name { get; set; }
        public string description { get; set; }
        public string controller { get; set; }
        public Guid? grid_form { get; set; }
        public Guid? edit_form { get; set; }
        public Guid? creating_form { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public bool? deleted { get; set; }
        public Guid? search_form { get; set; }
    }
}
