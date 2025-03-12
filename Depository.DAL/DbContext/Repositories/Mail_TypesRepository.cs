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
    public class Mail_TypesRepository:Repository<mail_types>, IMail_TypesRepository
    {
        public Mail_TypesRepository(ApplicationDbContext context) : base(context)
        {
            DbSet = context.mail_types;
        }

        public List<mail_types> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }


   
}
