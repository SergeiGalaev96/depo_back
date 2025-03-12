using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.Repositories
{
    public class Trading_SystemsRepository:Repository<trading_systems>, ITrading_SystemsRepository
    {
        public Trading_SystemsRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.trading_systems;
        }

        public List<trading_systems> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }
}
