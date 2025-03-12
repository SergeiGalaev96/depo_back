using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.Models.DTO
{
    public class charge_input_model
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int[] DepositorId { get; set; }
        public int[] ServiceTypeId { get; set; }
    }
}
