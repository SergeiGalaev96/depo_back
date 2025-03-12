using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.Repositories
{
    public class Partner_ContactsRepository:Repository<partner_contacts>, IPartner_ContactsRepository
    {
        public Partner_ContactsRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.partner_contacts;
        }

        public List<partner_contacts> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }
}
