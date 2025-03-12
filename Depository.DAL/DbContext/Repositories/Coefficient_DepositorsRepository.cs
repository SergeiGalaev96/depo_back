using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class Coefficient_DepositorsRepository:Repository<coefficient_depositors>, ICoefficient_DepositorsRepository
    {
        public Coefficient_DepositorsRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.coefficient_depositors;
        }

        public List<coefficient_depositors> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }
}
