using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.Models
{
    public class schedule_tasks : Entity
    {
       // public int sector_id { get; set; }
        public int task_type_id { get; set; }
        //public int? location_id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool? deleted { get; set; }
        public int hh { get; set; }
        public int mm { get; set; }
    }
}
