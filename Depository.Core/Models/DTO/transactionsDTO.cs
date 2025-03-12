using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models.DTO
{
    public class transactionsDTO
    {
        public int id { get; set; }
        public int instruction { get; set; }
        public DateTime date { get; set; }
        public string account { get; set; }
        public string trans_type { get; set; }
        public string stop { get; set; }
    }
}
