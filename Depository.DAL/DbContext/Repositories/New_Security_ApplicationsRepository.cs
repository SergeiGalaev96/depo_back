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
    public class New_Security_ApplicationsRepository:Repository<new_security_applications>, INew_Security_ApplicationsRepository
    {
        public List<new_security_applications> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }

        public New_Security_ApplicationsRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.new_security_applications;
        }
    }
}
