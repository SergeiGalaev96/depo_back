using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.Core.Models.DTO;
using Depository.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depository.DAL.DbContext.Repositories
{
    public class Accounting_EntryRepository:Repository<accounting_entry>, IAccounting_EntryRepository
    {
        public Accounting_EntryRepository(ApplicationDbContext context):base(context)
        {
            DbSet = context.accounting_entry;
        }

        //public List<accounting_entryDTO> FullTextSearch(accounting_entryDTO accounting_entryDTO)
        //{
            
        //    List<accounting_entry> accounting_entryList = new List<accounting_entry>();
        //    List<accounting_entryDTO> accounting_entryDTOList = new List<accounting_entryDTO>();
        //    accounting_entryList = DbSet.Include(x => x.account_types).ToList();

        //    if (accounting_entryDTO.operation != null)
        //    {
        //        accounting_entryList = accounting_entryList.Where(x => !String.IsNullOrEmpty(accounting_entryDTO.operation) && x.operation.Equals(accounting_entryDTO.operation)).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(accounting_entryDTO.instruction_status))
        //    { 
        //        instruction_statusDTO instruction_statusDTO = new instruction_statusDTO(accounting_entryDTO.instruction_status);
        //        if (instruction_statusDTO.status_new != null)
        //        {
        //            accounting_entryList = accounting_entryList.Where(x => x.status_new == instruction_statusDTO.status_new).ToList();
        //        }
        //        if (instruction_statusDTO.status_old != null)
        //        {
        //            accounting_entryList = accounting_entryList.Where(x => x.status_old == instruction_statusDTO.status_old).ToList();
        //        }
        //    }
        //    if (accounting_entryDTO.instruction_type != null)
        //    {
        //        accounting_entryList = accounting_entryList.Where(x => !String.IsNullOrEmpty(accounting_entryDTO.instruction_type) && x.instructions.instruction_types.name.Equals(accounting_entryDTO.instruction_type)).ToList();
        //    }

        //    foreach (var accounting_entry in accounting_entryList)
        //    {
        //        var instruction_statusDTO = new instruction_statusDTO(accounting_entry.status_old, accounting_entry.status_new);
        //        var item = new accounting_entryDTO { id = accounting_entry.id,  operation = accounting_entry.operation, instruction_type = accounting_entry.instruction_type, accounting_position = accounting_entry.account_types.name, instruction_status = instruction_statusDTO.instruction_status };
        //        accounting_entryDTOList.Add(item);
        //    }
        //    return accounting_entryDTOList;
        //}

        public List<accounting_entry> GetByTemplate(int instruction_type, int status_old, int status_new)
        {
            return Queryable.Where(DbSet, x => x.instruction_type == instruction_type && x.status_old.Equals(status_old) && x.status_new.Equals(status_new)).ToList();
        }

        public List<accounting_entry> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).ToList();
        }

       

        public List<accounting_entry> GetViaNestedTables()
        {
            return DbSet.ToList();
        }
    }
}
