using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.Models.DTO
{
    public class directories_report2
    {
        public depositors depositor { get; set; }
        public  partners partner { get; set; }
        public List<securities> securities { get; set; }
        public List<tariffs_cd> cd_tariffs { get; set; }
        public List<service_types> service_types { get; set; }
        public List<service_groups> service_groups { get; set; }
        public List<coefficient_depositors> coefficient_depositors { get; set; }

        public List<security_types> security_types { get; set; }
        public List<issuers> issuers { get; set; }
        public List<tariffs_registrars> tariff_registrars { get; set; }
        public List<partners> partners { get; set; }
        public List<currencies> currencies { get; set; }
        public directories_report2()
        {
            securities = new List<securities>();
            cd_tariffs = new List<tariffs_cd>();
            service_types = new List<service_types>();
            service_groups = new List<service_groups>();
            coefficient_depositors = new List<coefficient_depositors>();
            security_types = new List<security_types>();
            issuers = new List<issuers>();
            tariff_registrars = new List<tariffs_registrars>();
            currencies = new List<currencies>();
            partners = new List<partners>();
        }
    }
}
