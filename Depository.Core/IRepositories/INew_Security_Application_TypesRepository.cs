using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.IRepositories
{
    public interface INew_Security_Application_TypesRepository:IRepository<new_security_application_types>
    {
        List<new_security_application_types> GetList();
    }
}
