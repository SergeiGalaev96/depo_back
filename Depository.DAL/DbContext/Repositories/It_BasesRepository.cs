using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class It_BasesRepository:Repository<it_bases>, IIt_BasesRepository
    {
        public It_BasesRepository(ApplicationDbContext context) : base(context)
        {
            DbSet = context.it_bases;
        }

        public bool IsExistName(string name)
        {
            return Queryable.Any(DbSet, x => x.name.Equals(name));
        }

        public List<it_bases> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }
}
