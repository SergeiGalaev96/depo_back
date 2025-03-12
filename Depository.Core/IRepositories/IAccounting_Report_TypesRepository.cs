﻿using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.IRepositories
{
    public interface IAccounting_Report_TypesRepository:IRepository<accounting_report_types>
    {
        List<accounting_report_types> GetList();
    }
}
