using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class Instruction_ReportsRepository:Repository<instruction_reports>, IInstruction_ReportsRepository
    {
        public Instruction_ReportsRepository(ApplicationDbContext context) : base(context)
        {
            DbSet = context.instruction_reports;
        }

        public List<instruction_reports> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }

    }
}
