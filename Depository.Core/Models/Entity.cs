using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Depository.Core.Models
{
    public class Entity
    {
        [Key]
        public int id { get; set; }
    }
}
