using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.DAL.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.Repositories
{
    public class Instruction_Account_RelationsRepository:Repository<instruction_account_relations>, IInstruction_Account_RelationsRepository
    {
        public Instruction_Account_RelationsRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.instruction_account_relations;
        }

        public instruction_account_relations GetByTypeIdAndShortNumber(int instruction_type_id, string account_short_number)
        {
            return Queryable.Where(DbSet, x => x.instruction_type_id == instruction_type_id && x.account_short_number.Equals(account_short_number)).FirstOrDefault();
        }

        public List<instruction_account_relations> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }

       
    }
}
