using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IContractsRepository:IRepository<contracts>
    {
        List<contracts> GetList();
    }
}
