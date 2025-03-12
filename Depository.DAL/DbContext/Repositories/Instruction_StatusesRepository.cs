using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class Instruction_StatusesRepository:Repository<instruction_statuses>, IInstruction_StatusesRepository
    {
        public Instruction_StatusesRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.instruction_statuses;
        }

        public List<instruction_statuses> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }

        public bool isExistName(string name)
        {
            return Queryable.Any(DbSet, x => x.name.Equals(name));
        }
    }
}
