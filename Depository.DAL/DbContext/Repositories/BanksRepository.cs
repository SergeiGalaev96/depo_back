using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.Repositories
{
    public class BanksRepository:Repository<banks>, IBanksRepository
    {
        public BanksRepository(ApplicationDbContext context) : base(context)
        {
            DbSet = context.banks;
        }

        public banks GetFirstBank()
        {
            return DbSet.FirstOrDefault();
        }

        public List<banks> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }

        public bool IsExistBIС(string bic)
        {
            return Queryable.Any(DbSet, x => x.bik.Equals(bic));
        }
    }
}
