using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface  IIssuersRepository:IRepository<issuers>
    {
        List<issuers> GetList();

        List<issuers> GetViaNestedTables();
        issuers GetViaNestedTables(int id);
        List<issuers> GetListByRegistrar(int registrar_id);
    }
}
