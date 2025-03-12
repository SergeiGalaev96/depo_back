using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class CitiesRepository : Repository<cities>, ICitiesRepository
    {
        public CitiesRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.cities;
        }
        public List<cities> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }

        public bool IsExistName(string name)
        {
            return Queryable.Any(DbSet, x => x.name.Equals(name));
        }
    }
}
