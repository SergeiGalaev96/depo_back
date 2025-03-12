using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.IRepositories
{
    public interface IExcertsRepository:IRepository<excerts>
    {
        List<excerts> GetList();
    }
}
