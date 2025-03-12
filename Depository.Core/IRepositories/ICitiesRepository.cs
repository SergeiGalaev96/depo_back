using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface ICitiesRepository:IRepository<cities>
    {
        List<cities> GetList();
        bool IsExistName(string name);
    }

}
