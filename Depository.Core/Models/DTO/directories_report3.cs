using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.Models.DTO
{
    public class directories_report3
    {
        public service_types service_type { get; set; }
        public service_groups service_group { get; set; }
        public List<instructions> instructions { get; set; }
        public List<tariffs_cd> cd_tariffs { get; set; }
        public List<coefficient_depositors> coefficient_depositors { get; set; }
        public List<depositors> depositors { get; set; }
        public List<service_types> service_types { get; set; }
        public List<service_groups> service_groups { get; set; }
        public List<tariffs_registrars> tariff_registrars { get; set; }
        public List<partners> partners { get; set; }
        public List<securities> securities { get; set; }
        public directories_report3()
        {
            instructions = new List<instructions>();
            cd_tariffs = new List<tariffs_cd>();
            coefficient_depositors = new List<coefficient_depositors>();
            depositors = new List<depositors>();
            service_types = new List<service_types>();
            service_groups = new List<service_groups>();
            tariff_registrars = new List<tariffs_registrars>();
            partners = new List<partners>();
            securities = new List<securities>();
        }
    }
}
