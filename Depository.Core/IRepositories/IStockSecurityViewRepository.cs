using Depository.Core.Models;
using Depository.Core.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.IRepositories
{


    public interface IStockSecurityViewRepository : IRepository<stock_security_view>
    {
        List<stock_security_view> GetList();
        List<stock_security_view> ShowInPositions(bool show_in_positions);
    }
}
