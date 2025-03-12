using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IAccount_ManagersRepository:IRepository<account_managers>
    {
        List<account_managers> GetList();
    }

}
