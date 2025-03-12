using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IInstruction_TypesRepository:IRepository<instruction_types>
    {
        List<instruction_types> GetList();

    }
}
