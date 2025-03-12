using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface ITransfer_ordersRepository:IRepository<transfer_orders>
    {
        List<transfer_orders> GetList();
    }
}
