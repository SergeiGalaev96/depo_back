using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class Transfer_ordersRepository:Repository<transfer_orders>, ITransfer_ordersRepository
    {
        public Transfer_ordersRepository(ApplicationDbContext context) : base(context)
        {
            DbSet = context.transfer_orders;
        }
        public List<transfer_orders> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }
}
