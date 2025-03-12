using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.Repositories
{
    public class PartnersRepository:Repository<partners>, IPartnersRepository
    {
        public PartnersRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.partners;
        }

        public List<partners> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }

        public partners GetViaContacts(int partner)
        {
            return Queryable.Where(DbSet, x => (x.id == partner) && (!x.deleted.Value.Equals(true)))
                    .Include(x => x.partner_contacts).FirstOrDefault();
        }
    }
}
