using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.DAL.DbContext.Repositories
{
    public class ExcertsRepository:Repository<excerts>, IExcertsRepository
    {
        public ExcertsRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.excerts;
        }

        public List<excerts> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }
}
