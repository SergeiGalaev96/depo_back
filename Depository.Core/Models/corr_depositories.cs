using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class corr_depositories:Entity
    {
        public string name { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int partner { get; set; }
        public bool? deleted { get; set; }

        [JsonIgnore]
        public virtual partners partners { get; set; }
    }
}
