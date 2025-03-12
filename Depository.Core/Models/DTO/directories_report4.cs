using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.Models.DTO
{
    public class directories_report4
    {
        public List<depositors> depositors { get; set; }
        public List<instructions> instructions { get; set; }
        public List<partners> partners { get; set; }
        public List<securities> securities { get; set; }
        public List<security_types>security_types { get; set; }
        public List<issuers> issuers { get; set; }
        public List<service_types> service_types { get; set; }
        public List<service_groups> service_groups { get; set; }
        public List<tariffs_cd> tariffs_cds { get; set; }
        public List<tariffs_registrars> tariffs_registrars { get; set; }
        public List<coefficient_depositors> coefficient_depositors { get; set; }
        public List<security_balance> security_balances { get; set; }

        public directories_report4()
        {
            depositors = new List<depositors>();
            instructions = new List<instructions>();
            partners = new List<partners>();
            securities = new List<securities>();
            security_types = new List<security_types>();
            issuers = new List<issuers>();
            service_types = new List<service_types>();
            service_groups = new List<service_groups>();
            tariffs_registrars = new List<tariffs_registrars>();
            coefficient_depositors = new List<coefficient_depositors>();
            security_balances = new List<security_balance>();

        }
    }
}
