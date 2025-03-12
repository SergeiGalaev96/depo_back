using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface ICoefficient_DepositorsRepository:IRepository<coefficient_depositors>
    {
        List<coefficient_depositors> GetList();
    }
}
