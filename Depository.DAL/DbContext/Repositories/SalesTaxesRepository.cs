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
    public interface ISalesTaxesRepository : IRepository<sales_taxes>
    {

        List<sales_taxes> GetList();
    }
    public class SalesTaxesRepository : Repository<sales_taxes>, ISalesTaxesRepository
    {
        public SalesTaxesRepository(ApplicationDbContext context) : base(context)
        {
            DbSet = context.sales_taxes;
        }



        public List<sales_taxes> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Equals(true)).ToList();
        }


    }
}
