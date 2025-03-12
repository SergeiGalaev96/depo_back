using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IRecipient_TypesRepository:IRepository<recipient_types>
    {
        List<recipient_types> GetList();
    }
}
