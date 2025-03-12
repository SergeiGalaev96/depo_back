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
    public class InstructionReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InstructionReportsController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IInstruction_ReportsService _instruction_ReportService;
        public InstructionReportsController(ILogger<InstructionReportsController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IInstruction_ReportsService instruction_ReportService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _instruction_ReportService = instruction_ReportService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<instruction_reports> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] instruction_reports instruction_report, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<instruction_reports>> entityOperationResults = new List<EntityOperationResult<instruction_reports>>();
                var entityOperationResult = await _instruction_ReportService.Create(instruction_report, user_guid);
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
                    var instruction_reports = unitOfWork.instruction_reports.GetList();
                    return Ok(instruction_reports);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetInstruction_reports");
                return Ok(ex.ToString());
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
                    var instruction_report = unitOfWork.instruction_reports.Get(id);
                    return Ok(instruction_report);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetInstruction_report");
                return Ok(ex.ToString());
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] instruction_reports instruction_report, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<instruction_reports>> entityOperationResults = new List<EntityOperationResult<instruction_reports>>();
                var entityOperationResult = await _instruction_ReportService.Update(instruction_report, user_guid);
                entityOperationResults.Add(entityOperationResult);
                WriteToLogEntityOperationResult(entityOperationResult, "Update");
                return Ok(entityOperationResults);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Update");
                return Ok(ex.ToString());
            }
        }

        [HttpPost]
        public async Task<ActionResult> Delete([FromBody] int? id, Guid user_guid)
        {
            try
            {
                if (id == null) return BadRequest();
                List<EntityOperationResult<instruction_reports>> entityOperationResults = new List<EntityOperationResult<instruction_reports>>();
                var entityOperationResult = await _instruction_ReportService.Delete(id, user_guid);
                entityOperationResults.Add(entityOperationResult);
                WriteToLogEntityOperationResult(entityOperationResult, "Delete");
                return Ok(entityOperationResults);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Delete");
                return Ok(ex.ToString());
            }
        }
    }
}
