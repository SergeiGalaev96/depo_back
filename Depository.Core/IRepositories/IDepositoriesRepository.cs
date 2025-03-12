using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IDepositoriesRepository:IRepository<depositories>
    {
        List<depositories> GetList();
        depositories GetByStatus(int status_id);
    }
}
