using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.Repositories
{
    public class Instruction_TypesRepository : Repository<instruction_types>, IInstruction_TypesRepository
    {
        public Instruction_TypesRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.instruction_types;
        }

        public List<instruction_types> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }
}
