using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface ITariffs_Corr_Depository:IRepository<tariffs_corr_depository>
    {
        List<tariffs_corr_depository> GetList();
    }
}
