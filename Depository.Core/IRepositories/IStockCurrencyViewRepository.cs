using Depository.Core.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.IRepositories
{
    public interface IStockCurrencyViewRepository : IRepository<stock_currency_view>
    {
        List<stock_currency_view> GetList();
        List<stock_currency_view> ShowInPositions(bool show_in_positions);
    }
}
