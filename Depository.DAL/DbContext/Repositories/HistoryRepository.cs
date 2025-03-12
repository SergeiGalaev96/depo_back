using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class HistoryRepository:Repository<history>, IHistoryRepository
    {
        public HistoryRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.history;
        }

        public List<history> GetByObjectName(string object_name, int object_id)
        {
            return Queryable.Where(DbSet, x => x.object_name.Equals(object_name) && x.object_id == object_id).ToList();
        }
    }
}
