using Depository.Core.Models;
using Depository.Core;
using Depository.DAL.DbContext;
using Depository.DAL;
using Depository.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;

namespace Depository.Api.Controllers
{

    [Route("api/[controller]/[action]")]
    public class AccountingEntryStopTypesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountingEntryStopTypesController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IAccountingEntryStopTypesService _accountingEntryStopTypesService;
        public AccountingEntryStopTypesController(ILogger<AccountingEntryStopTypesController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IAccountingEntryStopTypesService accountingEntryStopTypesService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _accountingEntryStopTypesService = accountingEntryStopTypesService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<accounting_entry_stop_types> entityOperationResult, string action)
        {
            if (entityOperationResult.IsSuccess)
                _logger.LogInformation(action + " : " + JsonConvert.SerializeObject(entityOperationResult));
            else _logger.LogError(action + " : " + JsonConvert.SerializeObject(entityOperationResult));
        }

        private async Task WriteToLogException(Exception exception, string action)
        {
            _logger.LogError(action + " : " + exception.ToString());
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] accounting_entry_stop_types accounting_entry_stop_type, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<accounting_entry_stop_types>> entityOperationResults = new List<EntityOperationResult<accounting_entry_stop_types>>();
                var entityOperationResult = await _accountingEntryStopTypesService.Create(accounting_entry_stop_type, user_guid);
                entityOperationResults.Add(entityOperationResult);
                WriteToLogEntityOperationResult(entityOperationResult, "Create");
                return Ok(entityOperationResults);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Create");
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Gets()
        {
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var accounting_entry_stop_types = unitOfWork.accounting_entry_stop_types.GetList();
                    return Ok(accounting_entry_stop_types);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Gets");
                return Ok(ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var accounting_entry_stop_type = unitOfWork.accounting_entry_stop_types.Get(id);
                    return Ok(accounting_entry_stop_type);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Get");
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] accounting_entry_stop_types accounting_entry_stop_type, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<accounting_entry_stop_types>> entityOperationResults = new List<EntityOperationResult<accounting_entry_stop_types>>();
                var entityOperationResult = await _accountingEntryStopTypesService.Update(accounting_entry_stop_type, user_guid);
                entityOperationResults.Add(entityOperationResult);
                WriteToLogEntityOperationResult(entityOperationResult, "Update");
                return Ok(entityOperationResults);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Update");
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Delete([FromBody] int id, Guid user_guid)
        {
            try
            {
                if (id == null) return BadRequest();
                List<EntityOperationResult<accounting_entry_stop_types>> entityOperationResults = new List<EntityOperationResult<accounting_entry_stop_types>>();
                var entityOperationResult = await _accountingEntryStopTypesService.Delete(id, user_guid);
                entityOperationResults.Add(entityOperationResult);
                WriteToLogEntityOperationResult(entityOperationResult, "Update");
                return Ok(entityOperationResults);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Delete");
                return BadRequest();
            }
        }


    }

}
