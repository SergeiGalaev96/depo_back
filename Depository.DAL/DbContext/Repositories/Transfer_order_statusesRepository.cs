using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class Transfer_order_statusesRepository : Repository<transfer_order_statuses>, ITransfer_order_statusesRepository
    {
        public Transfer_order_statusesRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.transfer_order_statuses;
        }
        public List<transfer_order_statuses> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }
}
