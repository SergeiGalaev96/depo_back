using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.Models
{
    public class action
    {
        public int kind { get; set; }
        public int hh { get; set; }
        public int mm { get; set; }
        public Guid user_guid { get; set; }
    }
}
