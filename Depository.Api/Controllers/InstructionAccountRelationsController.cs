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
    public class InstructionAccountRelationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InstructionAccountRelationsController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IInstruction_Account_RelationsService _instruction_Account_RelationService;


        public InstructionAccountRelationsController(ILogger<InstructionAccountRelationsController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IInstruction_Account_RelationsService instruction_Account_RelationService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _instruction_Account_RelationService = instruction_Account_RelationService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<instruction_account_relations> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] instruction_account_relations instruction_account_relation, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<instruction_account_relations>> entityOperationResults = new List<EntityOperationResult<instruction_account_relations>>();
                var entityOperationResult = await _instruction_Account_RelationService.CreateInstruction_Account_Relation(instruction_account_relation, user_guid);
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
                    var instruction_account_relations = unitOfWork.instruction_account_relations.GetList();
                    return Ok(instruction_account_relations);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetAccounts");
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
                    var instruction_account_relation = unitOfWork.instruction_account_relations.Get(id);
                    return Ok(instruction_account_relation);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetAccount");
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] instruction_account_relations instruction_account_relation, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<instruction_account_relations>> entityOperationResults = new List<EntityOperationResult<instruction_account_relations>>();
                var entityOperationResult = await _instruction_Account_RelationService.UpdateInstruction_Account_Relation(instruction_account_relation, user_guid);
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
                List<EntityOperationResult<instruction_account_relations>> entityOperationResults = new List<EntityOperationResult<instruction_account_relations>>();
                var entityOperationResult = await _instruction_Account_RelationService.DeleteInstruction_Account_Relation(id, user_guid);
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
