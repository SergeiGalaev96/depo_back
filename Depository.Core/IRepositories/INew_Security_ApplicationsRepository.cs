using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.IRepositories
{
    public interface INew_Security_ApplicationsRepository:IRepository<new_security_applications>
    {
        List<new_security_applications> GetList();
    }
}
