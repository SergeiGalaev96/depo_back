using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class metadata:Entity
    {
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int type { get; set; }
        public  Guid defid { get; set; }
        public string data { get; set; }
    }
}
