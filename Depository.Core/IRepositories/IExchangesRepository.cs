using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public  interface IExchangesRepository:IRepository<exchanges>
    {
        List<exchanges> GetList();
    }
}
