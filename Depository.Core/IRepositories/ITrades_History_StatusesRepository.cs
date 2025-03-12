using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Depository.Core.IRepositories;
using Depository.Core.Models;

namespace Depository.DAL.DbContext.Repositories
{
    public interface ITrades_History_StatusesRepository : IRepository<trades_history_statuses>
    {
        List<trades_history_statuses> GetList();
    }
}
