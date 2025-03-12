using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class TransactionsRepository:Repository<transactions>, ITransactionsRepository
    {
        public TransactionsRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.transactions;
        }

        public List<transactions> GetList(int size)
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).OrderByDescending(x => x.id).Take(size).ToList();
        }

        public List<transactions> GetListByPage(int skip, int size)
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).OrderByDescending(x => x.id).Skip(skip).Take(size).ToList();
        }
    }
}
