using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IMail_statusesRepository:IRepository<mail_statuses>
    {
        List<mail_statuses> GetList();
    }
}
