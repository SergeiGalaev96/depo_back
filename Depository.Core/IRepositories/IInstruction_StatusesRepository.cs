using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IInstruction_StatusesRepository:IRepository<instruction_statuses>
    {
        bool isExistName(string name);
        List<instruction_statuses> GetList();
    }
}
