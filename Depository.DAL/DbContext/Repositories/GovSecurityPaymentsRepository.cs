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

    public interface IGovSecurityPaymentsRepository : IRepository<gov_securities_payments>
    {
        List<gov_securities_payments> GetList();
        List<gov_securities_payments> GetBySecurityId(int security_id);
    }

    public class GovSecurityPaymentsRepository : Repository<gov_securities_payments>, IGovSecurityPaymentsRepository
    {
        public GovSecurityPaymentsRepository(ApplicationDbContext context) : base(context)
        {
            DbSet = context.gov_securities_payments;
        }

        public List<gov_securities_payments> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Equals(true)).ToList();
        }

        public List<gov_securities_payments>GetBySecurityId(int security_id)
        {
            return Queryable.Where(DbSet, x => !x.deleted.Equals(true) && x.security == security_id).ToList();
        }
    }
}
