using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class MailsRepository : Repository<mails>, IMailsRepository
    {
        public MailsRepository(ApplicationDbContext context) : base(context)
        {
            DbSet = context.mails;
        }

        public List<mails> GetBySender(int user_id_sender)
        {
            return Queryable.Where(DbSet, x => x.sender == user_id_sender).ToList();
        }

        public List<mails> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }
}
