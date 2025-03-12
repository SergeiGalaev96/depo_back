using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.Models
{
    public class excerts:Entity
    {
        public Guid? files_directory { get; set; }
        public string depo_acc_number { get; set; }
        public string depo_acc_owner { get; set; }
        public int? depositor_from { get; set; }
        public int? acc_from { get; set; }
        public int? acc_from_type { get; set; }

        public int? security { get; set; }

        public double? quantity { get; set; }

        public int? depositor_to { get; set; }
        public int? acc_to { get; set; }
        public int? acc_to_type { get; set; }
        public string registration_number { get; set; }
        public int? currency { get; set; }
        public double price_by_nominal { get; set; }
        public string basis { get; set; }
        public int? filled_user_partner { get; set; }
        public int? status { get; set; }

        public int? corr_depository { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public bool? deleted { get; set; }
    }
}
