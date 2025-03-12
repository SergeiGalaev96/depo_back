using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IMail_distributionsRepository:IRepository<mail_distributions>
    {
        List<mail_distributions> GetList();
        List<mail_distributions> GetUnreadDistributions(int user_id_recipient);
        List<mail_distributions> GetByRecipient(int user_id_recipient);
        List<mail_distributions> GetTrashDistributions(int user_id_recipient);
        List<mail_distributions> GetBySender(int user_id_sender);
        int GetCountUnreadDistributions(int user_id_recipient);
    }
}
