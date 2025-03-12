using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class Legal_StatusesRepository:Repository<legal_statuses>, ILegal_StatusesRepository
    {
        public Legal_StatusesRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.legal_statuses;
        }

        public List<legal_statuses> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }
}
