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

    public interface IInstructionGikStatusesRepository : IRepository<instructions_gik_statuses>
    {
        List<instructions_gik_statuses> GetList();
        bool IsExistName(string name);
    }

    public class InstructionGikStatusesRepository : Repository<instructions_gik_statuses>, IInstructionGikStatusesRepository
    {
        public InstructionGikStatusesRepository(ApplicationDbContext context) : base(context)
        {
            DbSet = context.instructions_gik_statuses;
        }
        public List<instructions_gik_statuses> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Equals(true)).ToList();
        }

        public bool IsExistName(string name)
        {
            return Queryable.Any(DbSet, x => x.name.Equals(name));
        }
    }
}
