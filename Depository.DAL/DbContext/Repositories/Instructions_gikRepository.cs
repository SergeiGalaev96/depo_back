using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class Instructions_gikRepository : Repository<instructions_gik>, IInstructions_gikRepository
    {
        public Instructions_gikRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.instructions_gik;
        }

        public List<instructions_gik> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }
}
