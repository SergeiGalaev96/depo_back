using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface ISecuritiesRepository:IRepository<securities>
    {
        List<securities> GetList();
        List<securities> GetViaNestedTables();
        securities GetByCode(string code);
        List<securities> GetByIssuer(int issuer_id);
        List<securities> GetStandartSecurities();
        List<securities> GetGovermentSecurities();
    }
}
