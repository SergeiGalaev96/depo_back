using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IService_GroupsRepository:IRepository<service_groups>
    {
        List<service_groups> GetList();
    }
}
