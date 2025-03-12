using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.Repositories
{
    public class ExchangesRepository:Repository<exchanges>, IExchangesRepository
    {
        public ExchangesRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.exchanges;
        }

        public List<exchanges> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }
}
