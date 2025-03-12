using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class Trades_History_CurrenciesRepository:Repository<trades_history_currencies>, ITrades_History_CurrenciesRepository
    {
        public Trades_History_CurrenciesRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.trades_history_currencies;
        }

        public List<trades_history_currencies> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }
}
