using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IPartnersRepository:IRepository<partners>
    {
        List<partners> GetList();
        partners GetViaContacts(int partner);
    }
}
