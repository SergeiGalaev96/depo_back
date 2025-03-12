using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IPartner_ContactsRepository:IRepository<partner_contacts>
    {
        List<partner_contacts> GetList();
    }
}
