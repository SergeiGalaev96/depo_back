using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface ITransactionsRepository:IRepository<transactions>
    {
        List<transactions> GetList(int size);
        List<transactions> GetListByPage(int skip, int size);
    }
}
