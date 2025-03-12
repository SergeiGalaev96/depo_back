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
    public interface ISecurity_BalanceRepository:IRepository<security_balance>
    {
        List<security_balance> GetList();
        List<security_balance> GetByParams(DateTime dt_from, DateTime dt_to, List<accounts> accounts);
        List<security_balance> GetByPeriod(DateTime dt_from, DateTime dt_to);
    }
    public class Security_BalanceRepository:Repository<security_balance>, ISecurity_BalanceRepository
    {
        public Security_BalanceRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.security_balance;
        }

        public List<security_balance> GetByParams(DateTime dt_from, DateTime dt_to, List<accounts> accounts)
        {
            return Queryable.Where(DbSet, x => x.date_begin.Date>= dt_from && x.date_begin.Date <= dt_to && accounts.Any(y => y.id == x.account)).ToList();
        }

        public List<security_balance> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }

        public List<security_balance> GetByPeriod(DateTime dt_from, DateTime dt_to)
        {
            return Queryable.Where(DbSet, x => x.date_begin.Date >= dt_from && x.date_begin.Date <= dt_to).ToList();
        }
    }
}
