using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IOper_DaysRepository:IRepository<oper_days>
    {
        List<oper_days> GetList();
        bool IsExist(DateTime dateTime);
        bool IsDayOpened(DateTime dateTime);
        bool IsDayClosed(DateTime dateTime);
        oper_days GetByDate(DateTime oper_date);
        oper_days GetLastClosedDate();
    }
}
