using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.DAL.DbContext.Repositories
{
    public interface IInstructionRegistrarReportsRepository : IRepository<instruction_registrar_reports>
    {
        List<instruction_registrar_reports> GetList();
    }

    public class InstructionRegistrarReportsRepository : Repository<instruction_registrar_reports>, IInstructionRegistrarReportsRepository
    {
        public InstructionRegistrarReportsRepository(ApplicationDbContext context) : base(context)
        {
            DbSet = context.instruction_registrar_reports;
        }

        public List<instruction_registrar_reports> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Equals(true)).ToList();
        }

        
    }
}
