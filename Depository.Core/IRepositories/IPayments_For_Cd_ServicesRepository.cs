using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IPayments_For_Cd_ServicesRepository:IRepository<payments_for_cd_services>
    {
        List<payments_for_cd_services> GetList();
        double GetPaymentsByDepositor(int depositor, DateTime dtFrom);
        double GetPaymentsBetweenPeriod(int depositor, DateTime dtFrom, DateTime dtTo);
    }
}
