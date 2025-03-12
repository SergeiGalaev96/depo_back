using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IOrders_History_CurrenciesRepository:IRepository<orders_history_currencies>
    {
        List<orders_history_currencies> GetList();
        List<orders_history_currencies> GetByDate(DateTime order_date);
    }
}
