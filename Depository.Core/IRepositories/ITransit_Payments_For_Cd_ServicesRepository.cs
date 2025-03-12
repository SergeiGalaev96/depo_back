using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface ITransit_Payments_For_Cd_ServicesRepository:IRepository<transit_payments_for_cd_services>
    {
        List<transit_payments_for_cd_services> GetList();
    }
}
