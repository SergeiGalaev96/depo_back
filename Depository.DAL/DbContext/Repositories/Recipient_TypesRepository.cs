using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class Recipient_TypesRepository:Repository<recipient_types>, IRecipient_TypesRepository
    {
        public Recipient_TypesRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.recipient_types;
        }

        public List<recipient_types> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }

    }
}
