using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IPartner_TypesRepository:IRepository<partner_types>
    {
        List<partner_types> GetList();
        bool IsExistName(string name);
    }
}
