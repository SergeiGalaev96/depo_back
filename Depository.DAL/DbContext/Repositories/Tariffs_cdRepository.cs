using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class Tariffs_cdRepository:Repository<tariffs_cd>, ITariffs_cdRepository
    {
        public Tariffs_cdRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.tariffs_cd;
        }

        public List<tariffs_cd> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }
}
