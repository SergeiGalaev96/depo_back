using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IMailsRepository:IRepository<mails>
    {
        List<mails> GetList();
        List<mails> GetBySender(int user_id_sender);
    }


}
