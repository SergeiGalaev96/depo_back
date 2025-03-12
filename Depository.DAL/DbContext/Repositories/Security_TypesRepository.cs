using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.Repositories
{
    public class Security_TypesRepository:Repository<security_types>, ISecurity_TypesRepository
    {
        public Security_TypesRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.security_types;
        }
        public List<security_types> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }

        
    }
}
