using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class Accounts_GikRepository : Repository<accounts_gik>, IAccounts_GikRepository
    {
        public Accounts_GikRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.accounts_gik;
        }
        public List<accounts_gik> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }
}
