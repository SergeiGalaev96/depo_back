using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class Payer_TypesRepository:Repository<payer_types>, IPayer_TypesRepository
    {
        public Payer_TypesRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.payer_types;
        }

        public List<payer_types> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }
}
