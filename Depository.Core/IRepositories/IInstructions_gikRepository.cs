using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IInstructions_gikRepository: IRepository<instructions_gik>
    {
        List<instructions_gik> GetList();
    }
}
