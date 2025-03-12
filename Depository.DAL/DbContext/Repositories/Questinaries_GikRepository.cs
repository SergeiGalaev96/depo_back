using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class Questinaries_GikRepository:Repository<questinaries_gik>, IQuestinaries_GikRepository
    {
        public Questinaries_GikRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.questinaries_gik;
        }

        public List<questinaries_gik> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }
}
