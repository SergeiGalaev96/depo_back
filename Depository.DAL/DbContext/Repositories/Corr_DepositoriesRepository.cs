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
    public class Corr_DepositoriesRepository:Repository<corr_depositories>, ICorr_DepositoriesRepository
    {
        public Corr_DepositoriesRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.corr_depositories;
        }

        public List<corr_depositories> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).Include(x => x.partners).ToList(); ;
        }
    }
}
