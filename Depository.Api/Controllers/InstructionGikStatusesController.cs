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
    [ApiController]
    public class InstructionGikStatusesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InstructionGikStatusesController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IInstructionGikStatusesService _instructionGikStatusesService;

        public InstructionGikStatusesController(ILogger<InstructionGikStatusesController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IInstructionGikStatusesService instructionGikStatusesService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _instructionGikStatusesService = instructionGikStatusesService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<instructions_gik_statuses> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] instructions_gik_statuses instructions_gik_status, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<instructions_gik_statuses>> entityOperationResults = new List<EntityOperationResult<instructions_gik_statuses>>();
                var entityOperationResult = await _instructionGikStatusesService.Create(instructions_gik_status, user_guid);
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
                    var instruction_gik_statuses = unitOfWork.instruction_gik_statuses.GetList();
                    return Ok(instruction_gik_statuses);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetCities");
                return BadRequest();
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
                    var instruction_gik_status = unitOfWork.instruction_gik_statuses.Get(id);
                    return Ok(instruction_gik_status);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetCity");
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] instructions_gik_statuses instructions_gik_status, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<instructions_gik_statuses>> entityOperationResults = new List<EntityOperationResult<instructions_gik_statuses>>();
                var entityOperationResult = await _instructionGikStatusesService.Update(instructions_gik_status, user_guid);
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
        public async Task<ActionResult> Delete([FromBody] int? id, Guid user_guid)
        {
            try
            {
                if (id == null) return BadRequest();
                List<EntityOperationResult<instructions_gik_statuses>> entityOperationResults = new List<EntityOperationResult<instructions_gik_statuses>>();
                var entityOperationResult = await _instructionGikStatusesService.Delete(id, user_guid);
                entityOperationResults.Add(entityOperationResult);
                WriteToLogEntityOperationResult(entityOperationResult, "Delete");
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
