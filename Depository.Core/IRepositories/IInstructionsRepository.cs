using Depository.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.IRepositories
{
    public interface IInstructionsRepository : IRepository<instructions>
    {
        List<instructions> GetList();
        List<instructions> GetListByPage(int skip, int size);
        List<instructions> GetListByDate(DateTime date);
        List<instructions> GetListByFrom(int from);
        List<instructions> GetListByTo(int from);
        List<instructions> GetListByInstructionType(int instructionType);
        List<instructions> GetListBySecurity(int security);
        List<instructions> FullTextSearch(instructions instruction);
        int GetCountNewInstruction();
        instructions GetViaNestedTables(int id);
        List<instructions> GetByPeriodAndDepositor(int depositor, DateTime periodStart, DateTime periodEnd);
        bool CheckUrgentInstructions();
        List<instructions> GetByRegistrar(DateTime dt_from, DateTime dt_to, IEnumerable<issuers> issuers);
        List<instructions> GetInstructionsByDepositorPartner(int skip, int size, int depositor_partner);
        List<instructions> GetInstructionsByRegistrarPartner(int skip, int size, int registrar_partner);
        List<instructions> GetInstructionsRegistrar(int skip, int size);
        List<instructions> GetInstructionsByDepository(int skip, int size);
        List<instructions> GetInstructionsByCorrDepositoryPartner(int skip, int size, int corr_depository_partner);
        List<instructions> GetByDepositor(DateTime dt_from, DateTime dt_to, int depositor_id);
        List<instructions> GetByServiceType(DateTime dt_from, DateTime dt_to, service_types service_type);
        List<instructions> GetByPeriod(DateTime dt_from, DateTime dt_to);
    }
}
