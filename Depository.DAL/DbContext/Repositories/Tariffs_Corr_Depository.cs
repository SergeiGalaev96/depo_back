using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class Tariffs_Corr_Depository:Repository<tariffs_corr_depository>, ITariffs_Corr_Depository
    {
        public Tariffs_Corr_Depository(ApplicationDbContext context):base(context)
        {
            DbSet = context.tariffs_corr_depository;
        }

        public List<tariffs_corr_depository> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }
}
