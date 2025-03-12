using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IDepositorsRepository:IRepository<depositors>
    {
        List<depositors> GetList();
        depositors FindByPartner(int partner);
       
    }
}
