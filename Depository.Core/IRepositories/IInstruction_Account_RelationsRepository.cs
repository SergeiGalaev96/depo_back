using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IInstruction_Account_RelationsRepository:IRepository<instruction_account_relations>
    {
        List<instruction_account_relations> GetList();
        instruction_account_relations GetByTypeIdAndShortNumber(int instruction_type_id, string account_short_number);
    }
}
