using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IAccounts_GikRepository:IRepository<accounts_gik>
    {
        List<accounts_gik> GetList();
    }
}
