using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.Repositories
{
    public class DepositoriesRepository : Repository<depositories>, IDepositoriesRepository
    {
        public DepositoriesRepository(ApplicationDbContext context) : base(context)
        {
            DbSet = context.depositories;
        }

        public depositories GetByStatus(int status_id)
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).FirstOrDefault();
        }

        public List<depositories> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).Include(x => x.partners).ToList();
        }

        
    }
}
