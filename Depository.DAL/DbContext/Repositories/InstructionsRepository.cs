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
    public class InstructionsRepository : Repository<instructions>, IInstructionsRepository
    {
        public InstructionsRepository(ApplicationDbContext context) : base(context)
        {
            DbSet = context.instructions;
        }

        public List<instructions> FullTextSearch(instructions instruction)
        {
            List<instructions> instructionList = new List<instructions>();
            var rr = _context.instructions;
            instructionList = _context.instructions.ToList();
            if (instruction.executedDate > DateTime.MinValue)
            {
                instructionList = instructionList.Where(x => x.executedDate.Value.Date.Equals(instruction.executedDate.Value.Date)).ToList();
            }
            if (instruction.accFrom != null)
            {
                instructionList = instructionList.Where(x => x.accFrom == instruction.accFrom).ToList();
            }
            if (instruction.accTo != null)
            {
                instructionList = instructionList.Where(x => x.accTo == instruction.accTo).ToList();
            }
            if (instruction.ownerName != null)
            {
                instructionList = instructionList.Where(x => x.ownerName.Contains(instruction.ownerName)).ToList();
            }
            if (instruction.ownerAddress != null)
            {
                instructionList = instructionList.Where(x => !String.IsNullOrEmpty(x.ownerAddress) && x.ownerAddress.Contains(instruction.ownerAddress)).ToList();
            }
            if (instruction.ownerDocument != null)
            {
                instructionList = instructionList.Where(x => !String.IsNullOrEmpty(x.ownerDocument) && x.ownerDocument.Contains(instruction.ownerDocument)).ToList();
            }
            if (instruction.ownerName != null)
            {
                instructionList = instructionList.Where(x => !String.IsNullOrEmpty(x.ownerName) && x.ownerName.Contains(instruction.ownerName)).ToList();
            }
            if (instruction.security != null)
            {
                instructionList = instructionList.Where(x => x.security == instruction.security).ToList();
            }
            if (instruction.type != null)
            {
                instructionList = instructionList.Where(x => x.type == instruction.type).ToList();
            }
            if (instruction.created_at > DateTime.MinValue)
            {
                instructionList = instructionList.Where(x => x.created_at.Date.Equals(instruction.created_at.Date)).ToList();
            }
            if (instruction.depositor != null)
            {
                instructionList = instructionList.Where(x => x.depositor == instruction.depositor).ToList();
            }
            if (instruction.filled != null)
            {
                instructionList = instructionList.Where(x => x.filled == instruction.filled).ToList();
            }
            if (instruction.onExecution != null)
            {
                instructionList = instructionList.Where(x => x.onExecution == instruction.onExecution).ToList();
            }
            if (instruction.executed != null)
            {
                instructionList = instructionList.Where(x => x.executed == instruction.executed).ToList();
            }
            if (instruction.canceled != null)
            {
                instructionList = instructionList.Where(x => x.canceled == instruction.canceled).ToList();
            }
            if (instruction.signed != null)
            {
                instructionList = instructionList.Where(x => x.signed == instruction.signed).ToList();
            }
            return instructionList.OrderByDescending(x => x.updated_at).ToList();
        }

        public int GetCountNewInstruction()
        {
            return Queryable.Where(DbSet, x => x.opened.Value.Equals(false) && !x.deleted.Value.Equals(true)).Count();
        }

        public List<instructions> GetList()
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).OrderByDescending(x => x.updated_at).ToList();
        }

        public List<instructions> GetListByDate(DateTime date)
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true) && x.executedDate.Value.Date.Equals(date)).ToList();
        }

        public List<instructions> GetListByFrom(int from)
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true) && x.accFrom == from).ToList();
        }

        public List<instructions> GetListByInstructionType(int instructionType)
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true) && x.type == instructionType).ToList();
        }



        public List<instructions> GetListBySecurity(int security)
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true) && x.security == security).ToList();
        }

        public List<instructions> GetListByTo(int to)
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true) && x.accTo == to).ToList();
        }

        public instructions GetViaNestedTables(int id)
        {
            return Queryable.Where(DbSet, x => x.id == id)
                .Include(x => x.issuers)
                .Include(x => x.depositors).ThenInclude(x => x.partners)
                .Include(x => x.securities).ThenInclude(x => x.security_types)
                .Include(x => x.currencies)
                .FirstOrDefault();
        }

        public List<instructions> GetByPeriodAndDepositor(int depositor, DateTime periodStart, DateTime periodEnd)
        {
            return Queryable.Where(DbSet, x => x.depositor.Value == depositor && x.executedDate.Value >= periodStart && x.executedDate.Value <= periodEnd)
                 .Include(x => x.issuers)
                .Include(x => x.depositors).ThenInclude(x => x.partners)
                .Include(x => x.securities).ThenInclude(x => x.security_types)
                .Include(x => x.instruction_types).Include(x => x.currencies).ToList();
        }

        public bool CheckUrgentInstructions()
        {
            return Queryable.Any(DbSet, x => x.opened.Value.Equals(false) && !x.deleted.Value.Equals(true) && x.urgent.Value.Equals(true));
        }

        public List<instructions> GetListByPage(int skip, int size)
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)).OrderByDescending(x => x.id).Skip(skip).Take(size).ToList();
        }

        public List<instructions> GetByRegistrar(DateTime dt_from, DateTime dt_to, IEnumerable<issuers> issuers)
        {
            return Queryable.Where(DbSet, x => x.created_at >= dt_from && x.created_at <= dt_to && issuers.Any(y => y.id == x.issuer)).ToList();
        }

        // Поручения
        public List<instructions> GetInstructionsByDepositorPartner(int skip, int size, int depositor_partner)
        {
            List<instructions> instructionList = new List<instructions>();
            var rr = (from i in _context.instructions
                      join deps in _context.depositors on i.depositor equals deps.id
                      join deps2 in _context.depositors on i.depositor2 equals deps2.id
                      where deps.partner == depositor_partner || deps2.partner == depositor_partner && !i.deleted.Value.Equals(true)
                      select i).ToList();
            return rr;
        }

        // Поручения
        public List<instructions> GetInstructionsByCorrDepositoryPartner(int skip, int size, int corr_depository_partner)
        {
            List<instructions> instructionList = new List<instructions>();
            var rr = (from i in _context.instructions
                      join it in _context.instruction_types on i.type equals it.id
                      join corr_dep in _context.corr_depositories on i.corrDepository equals corr_dep.id
                      where corr_dep.partner == corr_depository_partner && it.corr == 1
                      select i).ToList();
            return rr;
        }
        // Поручения
        public List<instructions> GetInstructionsByDepository(int skip, int size)
        {
            return Queryable.Where(DbSet, x => x.onExecution.Value.Equals(true)).ToList();
        }

        // Сводный приказ
        public List<instructions> GetInstructionsByRegistrarPartner(int skip, int size, int registrar_partner)
        {
            List<instructions> instructionList = new List<instructions>();
            var rr = (from i in _context.instructions
                      join it in _context.instruction_types on i.type equals it.id
                      join secs in _context.securities on i.security equals secs.id
                      join regs in _context.registrars on secs.registrar equals regs.id
                      where regs.partner == registrar_partner && !i.deleted.Value.Equals(true) && it.instruction_registrar == true && i.onExecution.Value.Equals(true)
                      select i).ToList();
            return rr;
        }

        // Сводный приказ
        public List<instructions> GetInstructionsRegistrar(int skip, int size)
        {
            List<instructions> instructionList = new List<instructions>();
            var rr = (from i in _context.instructions
                      join it in _context.instruction_types on i.type equals it.id
                      where it.instruction_registrar == true && i.onExecution.Value.Equals(true)
                      select i).ToList();
            return rr;
        }


        public List<instructions> GetByDepositor(DateTime dt_from, DateTime dt_to, int depositor_id)
        {
            return Queryable.Where(DbSet,
                            x => !x.deleted.Value.Equals(true)
                            && x.executed.Value.Equals(true)
                            && x.created_at >= dt_from
                            && x.created_at <= dt_to
                            && (x.depositor.Value == depositor_id || x.depositor2.Value == depositor_id))
                            .ToList();
        }

        public List<instructions> GetByServiceType(DateTime dt_from, DateTime dt_to, service_types service_type)
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true) && service_type.instruction_type == x.type
                                              && x.executedDate.Value >= dt_from && x.executedDate.Value <= dt_to).ToList();
        }

        public List<instructions> GetByPeriod(DateTime dt_from, DateTime dt_to)
        {
            return Queryable.Where(DbSet, x => !x.deleted.Value.Equals(true)
                                             && x.executedDate.Value >= dt_from && x.executedDate.Value <= dt_to).ToList();
        }

    }
}
