
using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using Depository.DAL.DbContext;
using Depository.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Depository.Api.Controllers
{
    [Route("api/[controller]/[action]")]
  
    public class SecurityIssueFormTypesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SecurityIssueFormTypesController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly ISecurity_Issue_Form_TypesService _securityIssueFormTypesService;
        public SecurityIssueFormTypesController(ILogger<SecurityIssueFormTypesController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, ISecurity_Issue_Form_TypesService securityIssueFormTypesService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _securityIssueFormTypesService = securityIssueFormTypesService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<security_issue_form_types> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] security_issue_form_types security_issue_form_type, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<security_issue_form_types>> entityOperationResults = new List<EntityOperationResult<security_issue_form_types>>();
                var entityOperationResult = await _securityIssueFormTypesService.Create(security_issue_form_type, user_guid);
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
                    var security_issue_form_types = unitOfWork.security_issue_form_types.GetList();
                    return Ok(security_issue_form_types);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Getsecurity_issue_form_types");
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
                    var security_issue_form_type = unitOfWork.security_issue_form_types.Get(id);
                    return Ok(security_issue_form_type);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Getsecurity_issue_form_type");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] security_issue_form_types security_issue_form_type, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<security_issue_form_types>> entityOperationResults = new List<EntityOperationResult<security_issue_form_types>>();
                var entityOperationResult = await _securityIssueFormTypesService.Update(security_issue_form_type, user_guid);
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
                List<EntityOperationResult<security_issue_form_types>> entityOperationResults = new List<EntityOperationResult<security_issue_form_types>>();
                var entityOperationResult = await _securityIssueFormTypesService.Delete(id, user_guid);
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
