using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Depository.Core.Models
{
    public class banks:Entity
    {
       
        public string bik { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string mfo { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
    }
}
