using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Depository.DAL.DbContext;
using Depository.DAL;
using Depository.Domain.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Depository.Core.Models;
using Depository.Core;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Depository.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class InstructionRegistrarReportsController : ControllerBase
    {




        private readonly ApplicationDbContext _context;
        private readonly ILogger<InstructionRegistrarReportsController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IInstructionRegistrarReportsService _instructionRegistrarReportsService;

        public InstructionRegistrarReportsController(ILogger<InstructionRegistrarReportsController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IInstructionRegistrarReportsService instructionRegistrarReportsService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _instructionRegistrarReportsService = instructionRegistrarReportsService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<instruction_registrar_reports> entityOperationResult, string action)
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
        public async Task<IActionResult> Gets()
        {
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var instruction_registrar_reports = unitOfWork.instruction_registrar_reports.GetList();
                    return Ok(instruction_registrar_reports);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Gets");
                return BadRequest();
            }
        }


        [HttpPost]
        public async Task<ActionResult> Create([FromBody] instruction_registrar_reports instruction_registrar_report, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<instruction_registrar_reports>> entityOperationResults = new List<EntityOperationResult<instruction_registrar_reports>>();
                var entityOperationResult = await _instructionRegistrarReportsService.Create(instruction_registrar_report, user_guid);
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
                    var instruction_registrar_report = unitOfWork.instruction_registrar_reports.Get(id);
                    return Ok(instruction_registrar_report);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Get");
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] instruction_registrar_reports instruction_registrar_report, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<instruction_registrar_reports>> entityOperationResults = new List<EntityOperationResult<instruction_registrar_reports>>();
                var entityOperationResult = await _instructionRegistrarReportsService.Update(instruction_registrar_report, user_guid);
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
                List<EntityOperationResult<instruction_registrar_reports>> entityOperationResults = new List<EntityOperationResult<instruction_registrar_reports>>();
                var entityOperationResult = await _instructionRegistrarReportsService.Delete(id, user_guid);
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
