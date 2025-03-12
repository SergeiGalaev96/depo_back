using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class Tariffs_registrarsRepository:Repository<tariffs_registrars>, ITariffs_registrarsRepository
    {
        public Tariffs_registrarsRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.tariffs_registrars;
        }

        public List<tariffs_registrars> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }
}
