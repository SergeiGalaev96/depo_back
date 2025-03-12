using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.DbContext;
using Depository.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.DAL.DbContext.Repositories
{
    public interface IAccountingEntryStopTypesRepository : IRepository<accounting_entry_stop_types>
    {

        List<accounting_entry_stop_types> GetList();
    }
    public class AccountingEntryStopTypesRepository : Repository<accounting_entry_stop_types>, IAccountingEntryStopTypesRepository
    {
        public AccountingEntryStopTypesRepository(ApplicationDbContext context) : base(context)
        {
            DbSet = context.accounting_entry_stop_types;
        }



        public List<accounting_entry_stop_types> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Equals(true)).ToList();
        }


    }
}
