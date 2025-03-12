using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface ITariffs_registrarsRepository:IRepository<tariffs_registrars>
    {
        List<tariffs_registrars> GetList();
    }
}
