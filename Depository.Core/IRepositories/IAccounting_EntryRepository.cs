using Depository.Core.Models;
using Depository.Core.Models.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IAccounting_EntryRepository:IRepository<accounting_entry>
    {
        
        List<accounting_entry> GetList();
       // List<accounting_entryDTO> FullTextSearch(accounting_entryDTO accounting_entryDTO);
        List<accounting_entry> GetViaNestedTables();
        List<accounting_entry> GetByTemplate(int instruction_type, int status_old, int status_new);
    }
}
