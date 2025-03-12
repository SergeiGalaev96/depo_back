using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IAccountsRepository:IRepository<accounts>
    {
        bool IsExistAccNumber(string acc_number);
        accounts FindByAccNumber(string acc_number);
        List<accounts> GetList();
        List<accounts> FullTextSearch(accounts account);
        accounts GetByNumber(string acc_number);
        List<accounts> GetTradeAccounts();
        List<accounts> GetViaNestedTables();
        List<accounts> FilterForClearing(string acc_number, int acc_member);
        List<accounts> FilterForClearing(int acc_member);
        List<accounts> GetByPartner(int partner_id);
    }
}
