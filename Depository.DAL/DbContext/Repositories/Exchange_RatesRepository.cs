using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.Repositories
{
    public class Exchange_RatesRepository:Repository<exchange_rates>, IExchange_RatesRepository
    {
        public Exchange_RatesRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.exchange_rates;
        }

        public bool isExistRate(DateTime rateDate, int currency)
        {
           return DbSet.Any(p=>p.date.Equals(rateDate) && p.currency==currency);
        }

        public List<exchange_rates> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }

        public List<exchange_rates> GetTodayRate()
        {
            return Queryable.Where(DbSet, x => x.date.Date.Equals(DateTime.Now.Date)).ToList();
        }
    }
}
