using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
   public interface IStock_CurrencyRepository:IRepository<stock_currency>
   {
        List<stock_currency> GetList();
        List<stock_currency> GetBlockedList();
        bool IsExist(int account, int currency);
        stock_currency GetByParam(int account, int currency);
    }
}
