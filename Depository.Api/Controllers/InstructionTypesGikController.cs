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
    public class InstructionTypesGikController : Controller
    {


    private readonly ApplicationDbContext _context;
    private readonly ILogger<InstructionTypesGikController> _logger;
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IConfiguration _configuration;
    private readonly IInstructionTypesGikService _instructionsTypesGikService;
    public InstructionTypesGikController(ILogger<InstructionTypesGikController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IInstructionTypesGikService instructionsTypesGikService)
    {
        _logger = logger;
        _configuration = configuration;
        _context = context;
        _unitOfWorkFactory = unitOfWorkFactory;
        _instructionsTypesGikService = instructionsTypesGikService;
    }

    private async Task WriteToLogEntityOperationResult(EntityOperationResult<instruction_types_gik> entityOperationResult, string action)
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
    public async Task<ActionResult> Create([FromBody] instruction_types_gik instruction_type_gik, Guid user_guid)
    {
        try
        {
            List<EntityOperationResult<instruction_types_gik>> entityOperationResults = new List<EntityOperationResult<instruction_types_gik>>();
            var entityOperationResult = await _instructionsTypesGikService.Create(instruction_type_gik, user_guid);
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
                var instruction_types_giks = unitOfWork.instruction_types_gik.GetList();
                return Ok(instruction_types_giks);
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
                var instruction_type_gik = unitOfWork.instruction_types_gik.Get(id);
                return Ok(instruction_type_gik);
            }
        }
        catch (Exception ex)
        {
            WriteToLogException(ex, "instruction_type_gik");
            return Ok(ex.ToString());
        }
    }

    [HttpPost]
    public async Task<ActionResult> Update([FromBody] instruction_types_gik instruction_type_gik, Guid user_guid)
    {
        try
        {
            List<EntityOperationResult<instruction_types_gik>> entityOperationResults = new List<EntityOperationResult<instruction_types_gik>>();
            var entityOperationResult = await _instructionsTypesGikService.Update(instruction_type_gik, user_guid);
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
            List<EntityOperationResult<instruction_types_gik>> entityOperationResults = new List<EntityOperationResult<instruction_types_gik>>();
            var entityOperationResult = await _instructionsTypesGikService.Delete(id, user_guid);
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
