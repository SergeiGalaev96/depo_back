using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.DAL.DbContext.Repositories
{
    public class Trades_History_StatusesRepository:Repository<trades_history_statuses>, ITrades_History_StatusesRepository
    {
        public List<trades_history_statuses> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }

        public Trades_History_StatusesRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.trades_history_statuses;
        }
    }
}
