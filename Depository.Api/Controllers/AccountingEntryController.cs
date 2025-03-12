using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Depository.Core;
using Depository.Core.Models;
using Depository.Core.Models.DTO;
using Depository.DAL;
using Depository.DAL.DbContext;
using Depository.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Depository.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AccountingEntryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountingEntryController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IAccounting_EntryService _accounting_entryService;
        public AccountingEntryController(ILogger<AccountingEntryController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IAccounting_EntryService accounting_entryService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _accounting_entryService = accounting_entryService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<accounting_entry> entityOperationResult, string action)
        {
            if (entityOperationResult.IsSuccess)
                _logger.LogInformation(action + " : " + JsonConvert.SerializeObject(entityOperationResult));
            else _logger.LogError(action + " : " + JsonConvert.SerializeObject(entityOperationResult));
        }

        private async Task WriteToLogException(Exception exception, string action)
        {
            _logger.LogError(action + " : " + exception.ToString());
        }


        [HttpGet]
        public async Task<IActionResult> SayHi()
        {
            return Ok("hi");
        }
            //  [HttpPost]
            //public async Task<IActionResult> FullTextSearch([FromBody] accounting_entryDTO accounting_entryDTO, int page, int size)
            //{
            //    List<accounting_entryDTO> accounting_entryDTOList = new List<accounting_entryDTO>();
            //    var skip = (page - 1) * size;
            //    if (page <= 0 || size <= 0)
            //    {
            //        return BadRequest();
            //    }
            //    else if (accounting_entryDTO == null)
            //    {
            //        try
            //        {

            //            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            //            {
            //                accounting_entryDTOList = unitOfWork.accounting_entry.FullTextSearch(accounting_entryDTO).Skip(skip).Take(size).ToList();
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            WriteToLogException(ex, "FullTextSearch");
            //            return BadRequest();
            //        }
            //    }
            //    else
            //    {
            //        try
            //        {

            //            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            //            {
            //                accounting_entryDTOList = unitOfWork.accounting_entry.FullTextSearch(accounting_entryDTO).Skip(skip).Take(size).ToList();
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            WriteToLogException(ex, "FullTextSearch");
            //            return BadRequest();
            //        }
            //    }
            //    return Ok(accounting_entryDTOList);
            //}

            [HttpPost]
        public async Task<ActionResult> Create([FromBody] accounting_entry accounting_entry, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<accounting_entry>> entityOperationResults = new List<EntityOperationResult<accounting_entry>>();
                var entityOperationResult = await _accounting_entryService.CreateAccounting_Entry(accounting_entry, user_guid);
                entityOperationResults.Add(entityOperationResult);
                WriteToLogEntityOperationResult(entityOperationResult, "Create");
                return Ok(entityOperationResults);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Create");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Gets()
        {
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var accounting_entries = unitOfWork.accounting_entry.GetList();
                    return Ok(accounting_entries);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetAccounting_Entries");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }

       

        

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] accounting_entry accounting_entry, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<accounting_entry>> entityOperationResults = new List<EntityOperationResult<accounting_entry>>();
                var entityOperationResult = await _accounting_entryService.UpdateAccounting_Entry(accounting_entry, user_guid);
                entityOperationResults.Add(entityOperationResult);
                WriteToLogEntityOperationResult(entityOperationResult, "Update");
                return Ok(entityOperationResults);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Update");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }

        [HttpPost]
        public async Task<ActionResult> Delete([FromBody] int? id, Guid user_guid)
        {
            try
            {
                if (id == null) return BadRequest();
                List<EntityOperationResult<accounting_entry>> entityOperationResults = new List<EntityOperationResult<accounting_entry>>();
                var entityOperationResult = await _accounting_entryService.DeleteAccounting_Entry(id, user_guid);
                entityOperationResults.Add(entityOperationResult);
                WriteToLogEntityOperationResult(entityOperationResult, "Delete");
                return Ok(entityOperationResults);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Delete");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }
    }
}
