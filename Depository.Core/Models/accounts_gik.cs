using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class accounts_gik : Entity
    {
        public string acc_number { get; set; }
        public int? depositor { get; set; }
        public int? acc_type { get; set; }
        public DateTime? open_date { get; set; }
        public bool? is_active { get; set; }
        public bool? is_closed { get; set; }
        public DateTime? close_date { get; set; }

        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
        public int? acc_manager { get; set; }

    }

   
}
