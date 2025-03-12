using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.Repositories
{
    public class AccountsRepository : Repository<accounts>, IAccountsRepository
    {
        public AccountsRepository(ApplicationDbContext context) : base(context)
        {
            DbSet = context.accounts;
        }

        public List<accounts> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }

        public bool IsExistAccNumber(string acc_number)
        {
            return Queryable.Any(DbSet, x => x.accnumber.Equals(acc_number));
        }

        public List<accounts> FullTextSearch(accounts account)
        {
            List<accounts> accountList = new List<accounts>();
            accountList = _context.accounts.ToList();
            if (account.dateopen > DateTime.MinValue)
            {
                accountList = accountList.Where(x => x.dateopen.Value.Date.Equals(account.dateopen.Value.Date)).ToList();
            }
            if (account.accnumber!=null)
            {
                accountList = accountList.Where(x => !String.IsNullOrEmpty(account.accnumber) && x.accnumber.Contains(account.accnumber)).ToList();
            }
            if (account.acc_member!=null)
            {
                accountList = accountList.Where(x => x.acc_member == account.acc_member).ToList();
            }
            return accountList;
        }

        public accounts GetByNumber(string acc_number)
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true) && x.accnumber.Equals(acc_number)).FirstOrDefault();
        }

        public List<accounts> GetTradeAccounts()
        {
            return Queryable.Where(DbSet, x => x.istrading_account.Value.Equals(true)).ToList();
        }

        public List<accounts> GetViaNestedTables()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true))
                .Include(x => x.partners).ToList();
        }

        public accounts FindByAccNumber(string acc_number)
        {
            return Queryable.Where(DbSet, x => x.accnumber.Equals(acc_number)).FirstOrDefault();
        }

        public List<accounts> FilterForClearing(string acc_number, int acc_member)
        {
            return Queryable.Where(DbSet, x => x.accnumber.Equals(acc_number) && x.acc_member == acc_member).ToList();
        }


        public List<accounts> FilterForClearing(int acc_member)
        {
            return Queryable.Where(DbSet, x =>  x.acc_member == acc_member).ToList();
        }

        public List<accounts> GetByPartner(int partner_id)
        {
            return Queryable.Where(DbSet, x => x.partner == partner_id).ToList();
        }
    }
}
