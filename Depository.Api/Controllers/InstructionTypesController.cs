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

    public class InstructionTypesController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<InstructionTypesController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IInstruction_TypesService _instruction_typesService;
        public InstructionTypesController(ILogger<InstructionTypesController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IInstruction_TypesService instruction_typesService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _instruction_typesService = instruction_typesService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<instruction_types> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] instruction_types instruction_type, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<instruction_types>> entityOperationResults = new List<EntityOperationResult<instruction_types>>();
                var entityOperationResult = await _instruction_typesService.CreateInstruction_Type(instruction_type, user_guid);
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
                    var instruction_types = unitOfWork.instruction_types.GetList();
                    return Ok(instruction_types);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetInstructionTypes");
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
                    var instruction_type = unitOfWork.instruction_types.Get(id);
                    return Ok(instruction_type);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetInstructionType");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] instruction_types instruction_type, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<instruction_types>> entityOperationResults = new List<EntityOperationResult<instruction_types>>();
                var entityOperationResult = await _instruction_typesService.UpdateInstruction_Type(instruction_type, user_guid);
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
                List<EntityOperationResult<instruction_types>> entityOperationResults = new List<EntityOperationResult<instruction_types>>();
                var entityOperationResult = await _instruction_typesService.DeleteInstruction_Type(id, user_guid);
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
