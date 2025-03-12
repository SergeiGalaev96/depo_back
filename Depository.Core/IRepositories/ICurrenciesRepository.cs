using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface ICurrenciesRepository:IRepository<currencies>
    {
        currencies GetIdByCode(string code);
        List<currencies> GetList();
    }
}
