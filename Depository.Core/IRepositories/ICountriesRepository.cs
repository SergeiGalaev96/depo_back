using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface ICountriesRepository:IRepository<countries>
    {
        bool IsExistName(string countryName);
        List<countries> GetList();
    }
}
