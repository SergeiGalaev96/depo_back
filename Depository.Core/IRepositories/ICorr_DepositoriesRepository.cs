using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface ICorr_DepositoriesRepository:IRepository<corr_depositories>
    {
        List<corr_depositories> GetList();
    }
}
