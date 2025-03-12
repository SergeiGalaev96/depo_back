using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.DAL.DbContext.Repositories
{
    public interface IVatsRepository : IRepository<vats>
    {

        List<vats> GetList();
    }
    public class VatsRepository : Repository<vats>, IVatsRepository
    {
        public VatsRepository(ApplicationDbContext context) : base(context)
        {
            DbSet = context.vats;
        }

      

        public List<vats> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Equals(true)).ToList();
        }


    }
}
