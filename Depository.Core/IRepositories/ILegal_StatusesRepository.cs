using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface ILegal_StatusesRepository:IRepository<legal_statuses>
    {
        List<legal_statuses> GetList();
    }
}
