using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface ITransfer_order_statusesRepository:IRepository<transfer_order_statuses>
    {
        List<transfer_order_statuses> GetList();
    }
}
