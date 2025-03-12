using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class Account_ManagersRepository:Repository<account_managers>, IAccount_ManagersRepository
    {
        public Account_ManagersRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.account_managers;
        }

        public List<account_managers> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }
}
