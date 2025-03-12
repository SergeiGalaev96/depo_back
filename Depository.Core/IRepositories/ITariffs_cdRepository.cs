using Depository.Core.IRepositories;
using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface ITariffs_cdRepository:IRepository<tariffs_cd>
    {
        List<tariffs_cd> GetList();
    }
}
