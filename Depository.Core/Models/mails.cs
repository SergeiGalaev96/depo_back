using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class mails:Entity
    {
        public string? subject { get; set; }
        public string? body { get; set; }
        public int? status { get; set; }
        public Guid? files_directory { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
        public int? sender { get; set; }
        public int? type { get; set; }
        public string es_full_name { get; set; }
        public string es_inn { get; set; }
        public DateTime? es_date { get; set; }

        [JsonIgnore]
        public virtual List<mail_distributions> mail_distributions { get; set; }
    }
}
