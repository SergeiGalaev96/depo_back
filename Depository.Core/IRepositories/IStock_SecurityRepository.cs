using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IStock_SecurityRepository : IRepository<stock_security>
    {
        List<stock_security> GetList();
        stock_security GetBySecurity(int security_id);
        List<stock_security> GetListBySecurity(int security_id);
        List<stock_security> GetBlockedList();
    }
}
