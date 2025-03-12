using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class Service_TypesRepository:Repository<service_types>, IService_TypesRepository
    {
        public Service_TypesRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.service_types;
        }

        public List<service_types> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }

        public bool IsExistName(string name)
        {
            return Queryable.Any(DbSet, x => x.name.Equals(name));
        }

        public service_types GetByName(string service_type_name)
        {
            return Queryable.Where(DbSet, x => x.name.Equals(service_type_name)).FirstOrDefault();
        }
    }
}
