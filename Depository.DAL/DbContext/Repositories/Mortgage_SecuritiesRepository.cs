using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class Mortgage_SecuritiesRepository : Repository<mortgage_securities>, IMortgage_SecuritiesRepository
    {
        public Mortgage_SecuritiesRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.mortgage_securities;
        }
        public List<mortgage_securities> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }
}
