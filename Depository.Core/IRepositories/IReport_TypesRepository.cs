using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IReport_TypesRepository:IRepository<report_types>
    {
        List<report_types> GetList();
    }
}
