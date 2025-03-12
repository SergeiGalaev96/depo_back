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

    public interface ITask_TypesRepository : IRepository<task_types>
    {
        List<task_types> GetList();
    }
    public class Task_TypesRepository : Repository<task_types>, ITask_TypesRepository
    {
        public Task_TypesRepository(ApplicationDbContext context) : base(context)
        {
            DbSet = context.task_types;
        }

        public List<task_types> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }
}
