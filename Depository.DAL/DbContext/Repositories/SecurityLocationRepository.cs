using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.DAL.DbContext.Repositories
{

    public interface ISecurityLocationRepository : IRepository<security_location>
    {
        List<security_location> GetList();
    }

    public class SecurityLocationRepository : Repository<security_location>, ISecurityLocationRepository
    {
        public SecurityLocationRepository(ApplicationDbContext context) : base(context)
        {
            DbSet = context.security_location;
        }

      
        public List<security_location> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }


    }
}
