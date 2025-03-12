using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Depository.DAL.DbContext.Repositories
{
    public class Charge_For_Cd_ServicesRepository:Repository<charge_for_cd_services>, ICharge_For_Cd_ServicesRepository
    {
        public Charge_For_Cd_ServicesRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.charge_for_cd_services;
        }

        public List<service_type_dept> GetServiceTypeDeptByDepositor(int depositor, DateTime dtFrom, DateTime dtTo)
        {
            return Queryable.Where(DbSet, x => x.date >= dtFrom && x.date <= dtTo && x.depositor == depositor).GroupBy(g => g.service_type).Select(g => 
            new service_type_dept 
            { 
                service_type_id=g.Key,
                without_vat=g.Sum(x=>x.main_quantity), 
                total=g.Sum(x=>x.main_quantity), 
                transit=g.Sum(x=>x.transit_quantity),
                amount_total= g.Sum(x => x.main_quantity)+ g.Sum(x => x.transit_quantity),
            }).ToList();
        }

        public double? GetDeptByDepositor(int depositor, DateTime dtFrom)
        {
            return Queryable.Where(DbSet, x => x.date <= dtFrom && x.depositor == depositor).Sum(s => s.main_quantity + s.transit_quantity);
        }

        public List<charge_for_cd_services> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }

       
    }
}
