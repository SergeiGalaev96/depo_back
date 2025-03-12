using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class localities:Entity
    {
        public string code { get; set; }
        public string name { get; set; }
        public string parent { get; set; }
        public int istown { get; set; }
        public int level { get; set; }
        public int isaimc { get; set; }
        public int isdistc { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
    }
}
