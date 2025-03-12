using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface ITrades_History_CurrenciesRepository:IRepository<trades_history_currencies>
    {
        List<trades_history_currencies> GetList();
    }
}
