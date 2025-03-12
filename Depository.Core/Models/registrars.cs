using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class registrars:Entity
    {
        public string name { get; set; }
        public int partner { get; set; }
        public string notes { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
        public int? status { get; set; }
        public int? storage { get; set; }
        public virtual partners partners { get; set; }
        public virtual List<issuers> issuers { get; set; }

        public registrars()
        {
            partners = new partners();
        }
    }
}
