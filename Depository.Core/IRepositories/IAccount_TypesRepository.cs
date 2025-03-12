using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IAccount_TypesRepository:IRepository<account_types>
    {
        List<account_types> GetList();
        account_types GetByCode(int code);
    }
}
