using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IAccount_StatusesRepository:IRepository<account_statuses>
    {
        List<account_statuses> GetList();
        bool IsExistName(string name);
    }
}
