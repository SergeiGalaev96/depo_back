using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class ContractsRepository:Repository<contracts>, IContractsRepository
    {
        public ContractsRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.contracts;
        }

        public List<contracts> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }
}
