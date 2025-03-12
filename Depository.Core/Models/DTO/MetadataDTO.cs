using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models.DTO
{
    public class MetadataDTO
    {
        public int type { get; set; }
        public Guid defid { get; set; }
        public string data { get; set; }
    }
}
