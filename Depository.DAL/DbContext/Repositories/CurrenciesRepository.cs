using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.Repositories
{
    public class CurrenciesRepository:Repository<currencies>, ICurrenciesRepository
    {
        public CurrenciesRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.currencies;
        }

        public List<currencies> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }

        public currencies GetIdByCode(string code)
        {
            return Queryable.Where(DbSet, x => x.code.Equals(code)).FirstOrDefault();
        }
    }
}
