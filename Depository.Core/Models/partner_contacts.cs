using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class partner_contacts : Entity
    {
        public string uid { get; set; }
        public int partner { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string middlename { get; set; }
        public string address { get; set; }
        public string mail { get; set; }
        public string phones { get; set; }
        public string notes { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
        public virtual partners partners{get;set;}
    }
}
