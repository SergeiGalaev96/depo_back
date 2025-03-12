using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public  interface ITrades_History_SecuritiesRepository:IRepository<trades_history_securities>
    {
        List<trades_history_securities> GetList();
        List<trades_history_securities> GetByDate(DateTime trade_date);
        List<trades_history_securities> GetListByPage(int pageNumber, int pageSize);
        List<trades_history_securities> GetListByArray(int[] list);
    }
}
