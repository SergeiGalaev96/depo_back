using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class Orders_History_SecuritiesRepository:Repository<orders_history_securities>, IOrders_History_SecuritiesRepository
    {
        public Orders_History_SecuritiesRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.orders_history_securities;
        }

        public List<orders_history_securities> GetByDate(DateTime order_date)
        {
            return Queryable.Where(DbSet, x => x.order_date.Date.Equals(order_date.Date) && !x.processed.Value.Equals(true)).ToList();
        }

        public List<orders_history_securities> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }
}
