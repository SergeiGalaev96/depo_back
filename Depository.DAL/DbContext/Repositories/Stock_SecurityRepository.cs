using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class Stock_SecurityRepository:Repository<stock_security>, IStock_SecurityRepository
    {
        public Stock_SecurityRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.stock_security;
        }

        public List<stock_security> GetBlockedList()
        {
            return Queryable.Where(DbSet, x => x.quantity > 0).ToList();
        }

        public stock_security GetBySecurity(int security_id)
        {
            return Queryable.Where(DbSet, x => x.security == security_id).FirstOrDefault();
        }

        public List<stock_security> GetList()
        {
            return Queryable.Where(DbSet, x => x.quantity>0 || x.quantity_stop>0).ToList();
        }

        public List<stock_security> GetListBySecurity(int security_id)
        {
            return Queryable.Where(DbSet, x => x.security == security_id).ToList();
        }
    }
}
