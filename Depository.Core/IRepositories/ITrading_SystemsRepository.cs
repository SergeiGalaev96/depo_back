using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface ITrading_SystemsRepository:IRepository<trading_systems>
    {
        List<trading_systems> GetList();
    }
}
