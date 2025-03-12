using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface ILocalitiesRepository:IRepository<localities>
    {
        List<localities> GetList();
    }
}
