using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class Mail_statusesRepository:Repository<mail_statuses>, IMail_statusesRepository
    {
        public Mail_statusesRepository(ApplicationDbContext context) : base(context)
        {
            DbSet = context.mail_statuses;
        }

        public List<mail_statuses> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }
}
