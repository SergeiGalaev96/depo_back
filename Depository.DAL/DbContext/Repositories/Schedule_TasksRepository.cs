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
    public interface ISchedule_TasksRepository:IRepository<schedule_tasks>
    {
        List<schedule_tasks> GetList();
    }
    public class Schedule_TasksRepository:Repository<schedule_tasks>, ISchedule_TasksRepository
    {
        public Schedule_TasksRepository(ApplicationDbContext context) : base(context)
        {
            DbSet = context.schedule_tasks;
        }

        public List<schedule_tasks> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }
}
