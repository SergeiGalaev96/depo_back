using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.DAL.DbContext.Repositories
{
    public class New_Security_Application_TypesRepository:Repository<new_security_application_types>, INew_Security_Application_TypesRepository
    {
        public List<new_security_application_types> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }

        public New_Security_Application_TypesRepository(ApplicationDbContext context) : base(context)
        {
            DbSet = context.new_security_application_types;
        }
    }
}
