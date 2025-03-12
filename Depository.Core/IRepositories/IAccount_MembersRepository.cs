using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IAccount_MembersRepository:IRepository<account_members>
    {
        List<account_members> GetList();
        bool IsExistName(string name);
    }
}
