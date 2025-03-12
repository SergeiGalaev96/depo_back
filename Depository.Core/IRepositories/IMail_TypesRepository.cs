using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.IRepositories
{
    public interface IMail_TypesRepository:IRepository<mail_types>
    {
        List<mail_types> GetList();
    }
}
