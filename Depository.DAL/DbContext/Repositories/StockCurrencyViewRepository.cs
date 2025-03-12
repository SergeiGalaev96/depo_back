using Depository.Core.IRepositories;
using Depository.Core.Models.DTO;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.DAL.DbContext.Repositories
{


    public class StockCurrencyViewRepository : Repository<stock_currency_view>, IStockCurrencyViewRepository
    {
        public StockCurrencyViewRepository(ApplicationDbContext context) : base(context)
        {
            DbSet = context.stock_currency_view;
        }



        public List<stock_currency_view> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }

        public List<stock_currency_view> ShowInPositions(bool show_in_positions)
        {
            return Queryable.Where(DbSet, x => x.show_in_positions == show_in_positions && (x.quantity > 0 || x.quantity_stop > 0 || x.quantity_pledge > 0)).ToList();
        }
    }
}
