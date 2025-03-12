using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IPayer_TypesRepository:IRepository<payer_types>
    {
         List<payer_types> GetList();
    }
}
