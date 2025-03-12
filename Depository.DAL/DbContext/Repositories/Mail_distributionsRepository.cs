using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Depository.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Depository.DAL.DbContext.Repositories
{
    public  class Mail_distributionsRepository:Repository<mail_distributions>, IMail_distributionsRepository
    {
        public Mail_distributionsRepository(ApplicationDbContext context) : base(context)
        {
            DbSet = context.mail_distributions;
        }

        public List<mail_distributions> GetByRecipient(int user_id_recipient)
        {
            return Queryable.Where(DbSet, x => x.recipient == user_id_recipient).Include(x=>x.mails).ToList();
        }

        public List<mail_distributions> GetTrashDistributions(int user_id_recipient)
        {
            return Queryable.Where(DbSet, x => x.recipient_status == (int)MailStatus.Trash).Include(x => x.mails).ToList();
        }

        public List<mail_distributions> GetBySender(int user_id_sender)
        {
            return Queryable.Where(DbSet, x => x.sender == user_id_sender).Include(x => x.mails).ToList();
        }

        public List<mail_distributions> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }

       

        public List<mail_distributions> GetUnreadDistributions(int user_id_recipient)
        {
            return Queryable.Where(DbSet, x => x.recipient == user_id_recipient && x.recipient_status==(int)MailStatus.Unread).Include(x => x.mails).ToList();
        }

        public int GetCountUnreadDistributions(int user_id_recipient)
        {
            return Queryable.Where(DbSet, x => x.recipient == user_id_recipient && x.recipient_status == (int)MailStatus.Unread).Count();
        }
    }
}
