using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.Repositories
{
    public class CountriesRepository:Repository<countries>, ICountriesRepository
    {
        public CountriesRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.countries;
        }

        public List<countries> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }

        public bool IsExistName(string countryName)
        {
            
            return Queryable.Any(DbSet, x => x.name.Equals(countryName));
        }
    }
}
