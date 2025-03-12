using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class mail_distributions:Entity
    {
        public int sender { get; set; }
        public int recipient { get; set; }
        public int recipient_status { get; set; }
        public int mail_id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }

       // [JsonIgnore]
        public virtual mails mails { get; set; }
    }
}
