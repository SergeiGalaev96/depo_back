using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class Transit_Charge_For_Cd_ServicesRepository:Repository<transit_charge_for_cd_services>, ITransit_Charge_For_Cd_ServicesRepository
    {
        public Transit_Charge_For_Cd_ServicesRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.transit_charge_for_cd_services;
        }

        public List<transit_charge_for_cd_services> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }
    }
}
