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

    public interface IThs_tasksRepository : IRepository<ths_tasks>
    {
        List<ths_tasks> GetList();
    }
    public class Ths_tasksRepository:Repository<ths_tasks>, IThs_tasksRepository
    {

        public Ths_tasksRepository(ApplicationDbContext context) : base(context)
        {
            DbSet = context.ths_tasks;
        }

        public List<ths_tasks> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }

}
