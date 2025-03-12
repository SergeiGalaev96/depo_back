using Depository.Core.Models;
using Depository.Core;
using Depository.DAL;
using Depository.DAL.DbContext;
using Depository.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Depository.Api.Controllers
{

    [Route("api/[controller]/[action]")]
    public class InstructionsGikController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<InstructionsGikController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IInstructions_gikService _instructionsGikService;
        public InstructionsGikController(ILogger<InstructionsGikController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IInstructions_gikService instructionsGikService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _instructionsGikService = instructionsGikService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<instructions_gik> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] instructions_gik instructions_gik, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<instructions_gik>> entityOperationResults = new List<EntityOperationResult<instructions_gik>>();
                var entityOperationResult = await _instructionsGikService.Create(instructions_gik, user_guid);
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
                    var instructions_giks = unitOfWork.instructions_gik.GetList();
                    return Ok(instructions_giks);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Getinstructions_giks");
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
                    var instructions_gik = unitOfWork.instructions_gik.Get(id);
                    return Ok(instructions_gik);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Getinstructions_gik");
                return Ok(ex.ToString());
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] instructions_gik instructions_gik, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<instructions_gik>> entityOperationResults = new List<EntityOperationResult<instructions_gik>>();
                var entityOperationResult = await _instructionsGikService.Update(instructions_gik, user_guid);
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
                List<EntityOperationResult<instructions_gik>> entityOperationResults = new List<EntityOperationResult<instructions_gik>>();
                var entityOperationResult = await _instructionsGikService.Delete(id, user_guid);
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
