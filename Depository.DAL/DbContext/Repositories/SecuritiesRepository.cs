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
    public class SecuritiesRepository:Repository<securities>, ISecuritiesRepository
    {
        public SecuritiesRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.securities;
        }

        public securities GetByCode(string code)
        {
            return Queryable.Where(DbSet, x => x.code.Equals(code)).FirstOrDefault();
        }

        public List<securities> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }

        public List<securities> GetViaNestedTables()
        {
            return DbSet.Include(x => x.security_types)
                        .ToList();
        }

        public List<securities> GetByIssuer(int issuer_id)
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true) && x.issuer == issuer_id).ToList();
        }

        public List<securities> GetStandartSecurities()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true) && !x.is_gov_security.Value.Equals(true)).ToList();
        }

        public List<securities> GetGovermentSecurities()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true) && x.is_gov_security.Value.Equals(true)).ToList();
        }
    }
}
