using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IHistoryRepository:IRepository<history>
    {
        List<history> GetByObjectName(string object_name, int object_id);
    }
}
