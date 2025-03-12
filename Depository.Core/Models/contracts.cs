using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class contracts:Entity
    {
        public Guid files_directory { get; set; }
        public int partner { get; set; }
        public DateTime? open_date { get; set; }
        public DateTime? close_date { get; set; }
        public DateTime? prolongation_date { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
        public string notes { get; set; }
        public string additional_agreements { get; set; }
         

    }
}
