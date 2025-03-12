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


    public class NewSecurityApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NewSecurityApplicationsController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly INew_Security_ApplicationsService _new_security_applicationsService;

        public NewSecurityApplicationsController(ILogger<NewSecurityApplicationsController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, INew_Security_ApplicationsService new_security_applicationsService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _new_security_applicationsService = new_security_applicationsService;
        }
        private async Task WriteToLogEntityOperationResult(EntityOperationResult<new_security_applications> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] new_security_applications new_security_application, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<new_security_applications>> entityOperationResults = new List<EntityOperationResult<new_security_applications>>();
                var entityOperationResult = await _new_security_applicationsService.Create(new_security_application, user_guid);
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
                    var new_security_applications = unitOfWork.new_security_applications.GetList();
                    return Ok(new_security_applications);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Gets");
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
                    var new_security_application = unitOfWork.new_security_applications.Get(id);
                    return Ok(new_security_application);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Get");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] new_security_applications new_security_application, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<new_security_applications>> entityOperationResults = new List<EntityOperationResult<new_security_applications>>();
                var entityOperationResult = await _new_security_applicationsService.Update(new_security_application, user_guid);
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
                List<EntityOperationResult<new_security_applications>> entityOperationResults = new List<EntityOperationResult<new_security_applications>>();
                var entityOperationResult = await _new_security_applicationsService.Delete(id, user_guid);
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
