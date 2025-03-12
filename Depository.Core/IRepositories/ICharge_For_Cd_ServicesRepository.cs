using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface ICharge_For_Cd_ServicesRepository:IRepository<charge_for_cd_services>
    {
        List<charge_for_cd_services> GetList();
        double? GetDeptByDepositor(int depositor, DateTime dtFrom);
        List<service_type_dept> GetServiceTypeDeptByDepositor(int depositor, DateTime dtFrom, DateTime dtTo);

    }
}
