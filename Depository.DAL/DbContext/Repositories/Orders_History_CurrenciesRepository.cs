using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class Orders_History_CurrenciesRepository:Repository<orders_history_currencies>, IOrders_History_CurrenciesRepository
    {
        public Orders_History_CurrenciesRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.orders_history_currencies;
        }

        public List<orders_history_currencies> GetByDate(DateTime order_date)
        {
            return Queryable.Where(DbSet, x => x.order_date.Date.Equals(order_date.Date) && !x.processed.Value.Equals(true)).ToList();
        }

        public List<orders_history_currencies> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }
}
