using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.DbContext;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Depository.DAL.Repositories
{
    public class DepositorsRepository:Repository<depositors>, IDepositorsRepository
    {
        public DepositorsRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.depositors;
        }

        public depositors FindByPartner(int partner)
        {
            return Queryable.Where(DbSet, x => x.partner == partner).FirstOrDefault();
        }

        public List<depositors> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).Include(x=>x.partners).ToList();
        }
    }
}
