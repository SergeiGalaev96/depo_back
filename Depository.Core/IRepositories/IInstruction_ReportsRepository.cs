using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IInstruction_ReportsRepository:IRepository<instruction_reports>
    {
        List<instruction_reports> GetList();
    }
}
