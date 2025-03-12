using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using Depository.DAL.DbContext;
using Depository.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Depository.Api.Controllers
{
    [Route("api/[controller]/[action]")]

    public class NewSecurityApplicationTypesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NewSecurityApplicationTypesController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly INew_Security_Application_TypesService _new_security_application_typesService;

        public NewSecurityApplicationTypesController(ILogger<NewSecurityApplicationTypesController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, INew_Security_Application_TypesService new_security_application_typesService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _new_security_application_typesService = new_security_application_typesService;
        }
        private async Task WriteToLogEntityOperationResult(EntityOperationResult<new_security_application_types> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] new_security_application_types new_security_application_type, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<new_security_application_types>> entityOperationResults = new List<EntityOperationResult<new_security_application_types>>();
                var entityOperationResult = await _new_security_application_typesService.Create(new_security_application_type, user_guid);
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
                    var new_security_application_types = unitOfWork.new_security_application_types.GetList();
                    return Ok(new_security_application_types);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetAccounts");
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
                    var new_security_application_type = unitOfWork.new_security_application_types.Get(id);
                    return Ok(new_security_application_type);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Get");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] new_security_application_types new_security_application_type, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<new_security_application_types>> entityOperationResults = new List<EntityOperationResult<new_security_application_types>>();
                var entityOperationResult = await _new_security_application_typesService.Update(new_security_application_type, user_guid);
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
                List<EntityOperationResult<new_security_application_types>> entityOperationResults = new List<EntityOperationResult<new_security_application_types>>();
                var entityOperationResult = await _new_security_application_typesService.Delete(id, user_guid);
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
