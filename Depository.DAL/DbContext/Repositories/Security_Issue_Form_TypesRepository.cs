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

    public interface ISecurity_Issue_Form_TypesRepository:IRepository<security_issue_form_types>
    {
        List<security_issue_form_types> GetList();
    }
    public class Security_Issue_Form_TypesRepository : Repository<security_issue_form_types>, ISecurity_Issue_Form_TypesRepository
    {
        public Security_Issue_Form_TypesRepository(ApplicationDbContext context) : base(context)
        {
            DbSet = context.security_issue_form_types;
        }

        public List<security_issue_form_types> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }
}
