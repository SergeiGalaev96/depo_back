using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IService_TypesRepository:IRepository<service_types>
    {
        List<service_types> GetList();
        bool IsExistName(string name);
        service_types GetByName(string service_type_name);
    }
}
