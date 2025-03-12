using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IQuestinaries_GikRepository:IRepository<questinaries_gik>
    {
        List<questinaries_gik> GetList();
    }
}
