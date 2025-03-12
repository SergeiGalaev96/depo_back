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
    public class Accounting_Report_TypesRepository:Repository<accounting_report_types>, IAccounting_Report_TypesRepository
    {
        public Accounting_Report_TypesRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.accounting_report_types;
        }
        public List<accounting_report_types> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }
}
