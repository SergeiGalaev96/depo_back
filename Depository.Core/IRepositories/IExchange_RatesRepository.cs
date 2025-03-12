using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IExchange_RatesRepository:IRepository<exchange_rates>
    {
        bool isExistRate(DateTime rateDate, int currency);
        List<exchange_rates> GetList();
        List<exchange_rates> GetTodayRate();
    }
}
