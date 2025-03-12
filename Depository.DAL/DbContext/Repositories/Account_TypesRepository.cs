using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.Repositories
{
    public  class Account_TypesRepository:Repository<account_types>, IAccount_TypesRepository
    {
        public Account_TypesRepository(ApplicationDbContext context) : base(context)
        {
            DbSet = context.account_types;
        }

        public account_types GetByCode(int code)
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true) && x.code.Equals(code)).FirstOrDefault();
        }

        public List<account_types> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }
}
