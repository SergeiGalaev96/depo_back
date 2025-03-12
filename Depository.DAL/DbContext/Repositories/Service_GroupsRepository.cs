using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class Service_GroupsRepository:Repository<service_groups>, IService_GroupsRepository
    {
        public Service_GroupsRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.service_groups;
        }

        public List<service_groups> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }
}
