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

    public interface ISectorsRepository:IRepository<sectors>
    {
        List<sectors> GetList();
    }
    public class SectorsRepository :  Repository<sectors>, ISectorsRepository
    {
        public SectorsRepository(ApplicationDbContext context) : base(context)
        {
            DbSet = context.sectors;
        }

        public List<sectors> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }
}
