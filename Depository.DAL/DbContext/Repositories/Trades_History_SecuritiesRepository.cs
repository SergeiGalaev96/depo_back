using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class Trades_History_SecuritiesRepository:Repository<trades_history_securities>,  ITrades_History_SecuritiesRepository
    {
        public Trades_History_SecuritiesRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.trades_history_securities;
        }

        public List<trades_history_securities> GetByDate(DateTime trade_date)
        {
            return Queryable.Where(DbSet, x => x.trade_date.Date.Equals(trade_date.Date) && !x.processed.Value.Equals(true)).ToList();
        }

        public List<trades_history_securities> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }

        public List<trades_history_securities>GetListByPage(int pageNumber, int pageSize)
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).Skip(pageSize * pageNumber).Take(pageSize).ToList();
        }

        public List<trades_history_securities> GetListByArray(int[] list)
        {
            return Queryable.Where(DbSet, x=>list.Any(y=>y==x.id)).ToList();
        }
    }
}
