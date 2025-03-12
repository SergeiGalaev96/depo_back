using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class Report_TypesRepository:Repository<report_types>, IReport_TypesRepository
    {
        public Report_TypesRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.report_types;
        }

        public List<report_types> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }
}
