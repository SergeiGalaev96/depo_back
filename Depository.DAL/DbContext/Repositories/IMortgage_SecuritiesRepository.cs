using Depository.Core.IRepositories;
using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public interface IMortgage_SecuritiesRepository:IRepository<mortgage_securities>
    {
        List<mortgage_securities> GetList();
    }
}
