using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class Payments_For_Cd_ServicesRepository:Repository<payments_for_cd_services>, IPayments_For_Cd_ServicesRepository
    {
        public Payments_For_Cd_ServicesRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.payments_for_cd_services;
        }

        public List<payments_for_cd_services> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }

        public double GetPaymentsByDepositor(int depositor, DateTime dtFrom)
        {
            return Queryable.Where(DbSet, x => x.date < dtFrom && x.depositor == depositor).Sum(s => s.quantity);
        }

        public double GetPaymentsBetweenPeriod(int depositor, DateTime dtFrom, DateTime dtTo)
        {
            return Queryable.Where(DbSet, x => x.date >= dtFrom && x.date<=dtTo && x.depositor == depositor).Sum(s => s.quantity);
        }
    }
}
