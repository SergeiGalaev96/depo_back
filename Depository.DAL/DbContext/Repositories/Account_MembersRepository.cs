using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.Repositories
{
    public class Account_MembersRepository:Repository<account_members>, IAccount_MembersRepository
    {
        public Account_MembersRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.account_members;
        }

        public List<account_members> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }

        public bool IsExistName(string name)
        {
            return Queryable.Any(DbSet, x => x.name.Equals(name));
        }
    }
}
