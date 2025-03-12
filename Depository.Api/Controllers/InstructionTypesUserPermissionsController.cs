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
    public class InstructionTypesUserPermissionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InstructionTypesUserPermissionsController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IInstruction_Types_User_PermissionsService _instruction_types_user_permissionsService;
        public InstructionTypesUserPermissionsController(ILogger<InstructionTypesUserPermissionsController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IInstruction_Types_User_PermissionsService instruction_types_user_permissionsService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _instruction_types_user_permissionsService = instruction_types_user_permissionsService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<instruction_types_user_permissions> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] instruction_types_user_permissions instruction_types_user_permission, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<instruction_types_user_permissions>> entityOperationResults = new List<EntityOperationResult<instruction_types_user_permissions>>();
                var entityOperationResult = await _instruction_types_user_permissionsService.Create(instruction_types_user_permission, user_guid);
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
                    var instruction_types_user_permissions = unitOfWork.instruction_types_user_permissions.GetList();
                    return Ok(instruction_types_user_permissions);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Getinstruction_types_user_permission");
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
                    var instruction_types_user_permission = unitOfWork.instruction_types_user_permissions.Get(id);
                    return Ok(instruction_types_user_permission);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Getinstruction_types_user_permission");
                return Ok(ex.ToString());
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] instruction_types_user_permissions instruction_types_user_permission, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<instruction_types_user_permissions>> entityOperationResults = new List<EntityOperationResult<instruction_types_user_permissions>>();
                var entityOperationResult = await _instruction_types_user_permissionsService.Update(instruction_types_user_permission, user_guid);
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
                List<EntityOperationResult<instruction_types_user_permissions>> entityOperationResults = new List<EntityOperationResult<instruction_types_user_permissions>>();
                var entityOperationResult = await _instruction_types_user_permissionsService.Delete(id, user_guid);
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
