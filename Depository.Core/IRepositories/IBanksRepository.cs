
using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IBanksRepository : IRepository<banks>
    {
         banks GetFirstBank();
         bool IsExistBIС(string bic);
         List<banks> GetList();
    }
}
