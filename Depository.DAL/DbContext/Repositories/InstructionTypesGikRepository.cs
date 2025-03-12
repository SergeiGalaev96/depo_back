using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.DAL.DbContext.Repositories
{

    public interface IInstructionTypesGikRepository : IRepository<instruction_types_gik>
    {
        List<instruction_types_gik> GetList();
    }
    public class InstructionTypesGikRepository : Repository<instruction_types_gik>, IInstructionTypesGikRepository
    {
        public InstructionTypesGikRepository(ApplicationDbContext context) : base(context)
        {
            DbSet = context.instruction_types_gik;
        }

        public List<instruction_types_gik> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Equals(true)).ToList();
        }
    }
}
