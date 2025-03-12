using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IOrders_History_SecuritiesRepository:IRepository<orders_history_securities>
    {
        List<orders_history_securities> GetList();
        List<orders_history_securities> GetByDate(DateTime order_date);
    }
}
