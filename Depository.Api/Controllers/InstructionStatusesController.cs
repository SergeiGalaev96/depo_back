using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Depository.Core;
using Depository.Core.Models;
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
    public class InstructionStatusesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InstructionStatusesController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IInstruction_StatusesService _instruction_StatusesService;
        public InstructionStatusesController(ILogger<InstructionStatusesController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IInstruction_StatusesService instruction_StatusesService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _instruction_StatusesService = instruction_StatusesService;
        }
        private async Task WriteToLogEntityOperationResult(EntityOperationResult<instruction_statuses> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] instruction_statuses instruction_status, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<instruction_statuses>> entityOperationResults = new List<EntityOperationResult<instruction_statuses>>();
                var entityOperationResult = await _instruction_StatusesService.Create(instruction_status, user_guid);
                entityOperationResults.Add(entityOperationResult);
                WriteToLogEntityOperationResult(entityOperationResult, "Create");
                return Ok(entityOperationResults);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Create");
                return Ok(ex.ToString());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Gets()
        {
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var instruction_statuses = unitOfWork.instruction_statuses.GetList();
                    return Ok(instruction_statuses);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetInstruction_statuses");
                return Ok(JsonConvert.SerializeObject(ex));
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
                    var account = unitOfWork.accounts.Get(id);
                    return Ok(account);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetAccount");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] instruction_statuses instruction_status, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<instruction_statuses>> entityOperationResults = new List<EntityOperationResult<instruction_statuses>>();
                var entityOperationResult = await _instruction_StatusesService.Update(instruction_status, user_guid);
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
                List<EntityOperationResult<instruction_statuses>> entityOperationResults = new List<EntityOperationResult<instruction_statuses>>();
                var entityOperationResult = await _instruction_StatusesService.Delete(id, user_guid);
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
