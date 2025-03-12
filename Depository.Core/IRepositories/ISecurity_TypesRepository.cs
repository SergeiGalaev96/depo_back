using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
   public interface ISecurity_TypesRepository:IRepository<security_types>
    {
        List<security_types> GetList();
    }
}
