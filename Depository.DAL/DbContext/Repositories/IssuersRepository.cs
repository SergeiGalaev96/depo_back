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
    public class IssuersRepository:Repository<issuers>, IIssuersRepository
    {
        public IssuersRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.issuers;
            
        }

        public List<issuers> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }

        public List<issuers> GetViaNestedTables()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true))
                .Include(x => x.registrars).ThenInclude(x => x.partners).ToList();
        }

        public issuers GetViaNestedTables(int id)
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true) && x.id == id)
                .Include(x => x.registrars).ThenInclude(x => x.partners).FirstOrDefault();
        }

        public List<issuers>GetListByRegistrar(int registrar_id)
        {
            return Queryable.Where(DbSet, x => x.registrar==registrar_id).ToList();
        }
    }
}
