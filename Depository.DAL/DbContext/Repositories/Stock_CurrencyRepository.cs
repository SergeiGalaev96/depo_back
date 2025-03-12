using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class Stock_CurrencyRepository:Repository<stock_currency>, IStock_CurrencyRepository
    {
        public Stock_CurrencyRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.stock_currency;
        }

        public List<stock_currency> GetList()
        {
            return Queryable.Where(DbSet, x => x.quantity > 0 || x.quantity_stop > 0).ToList();
        }

        public List<stock_currency> GetBlockedList()
        {
            return Queryable.Where(DbSet, x => x.quantity > 0).ToList();
        }

        public bool IsExist(int account, int currency)
        {
            return Queryable.Any(DbSet, x=>x.account==account && x.currency == currency);
        }

        public stock_currency GetByParam(int account, int currency)
        {
            return Queryable.Where(DbSet, x => x.account == account && x.currency == currency).FirstOrDefault();
        }
    }
}
