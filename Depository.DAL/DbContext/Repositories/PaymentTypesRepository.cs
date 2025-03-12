using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.DAL.DbContext.Repositories
{

    public interface IPaymentTypesRepository : IRepository<payment_types>
    {

        List<payment_types> GetList();
    }
    public class PaymentTypesRepository : Repository<payment_types>, IPaymentTypesRepository
    {
        public PaymentTypesRepository(ApplicationDbContext context) : base(context)
        {
            DbSet = context.payment_types;
        }



        public List<payment_types> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Equals(true)).ToList();
        }


    }

}
