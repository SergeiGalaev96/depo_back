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
    

    public interface ILimitsRepository : IRepository<limits>
    {
        List<limits> GetList();
    }


    public class LimitsRepository : Repository<limits>, ILimitsRepository
    {
        public LimitsRepository(ApplicationDbContext context) : base(context)
        {
            DbSet = context.limits;
        }

        public List<limits> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }


    }
}
