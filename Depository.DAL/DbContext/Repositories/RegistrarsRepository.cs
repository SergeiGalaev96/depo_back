using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.Repositories
{
    public class RegistrarsRepository:Repository<registrars>, IRegistrarsRepository
    {
        public RegistrarsRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.registrars;
        }

        public List<registrars> GetList()
        {
             return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).Include(x => x.partners).ToList();
        }
    }
}
