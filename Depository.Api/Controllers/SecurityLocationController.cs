using Depository.Core.Models;
using Depository.Core;
using Depository.DAL.DbContext;
using Depository.DAL;
using Depository.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;

namespace Depository.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    public class SecurityLocationController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<SecurityLocationController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly ISecurityLocationsService _securityLocationService;
        public SecurityLocationController(ILogger<SecurityLocationController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, ISecurityLocationsService securityLocationService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _securityLocationService = securityLocationService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<security_location> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] security_location security_location, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<security_location>> entityOperationResults = new List<EntityOperationResult<security_location>>();
                var entityOperationResult = await _securityLocationService.Create(security_location, user_guid);
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
                    var security_locations = unitOfWork.security_location.GetList();
                    return Ok(security_locations);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Getsecurity_locations");
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
                    var account_type = unitOfWork.security_location.Get(id);
                    return Ok(account_type);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Getsecurity_location");
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] security_location security_location, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<security_location>> entityOperationResults = new List<EntityOperationResult<security_location>>();
                var entityOperationResult = await _securityLocationService.Update(security_location, user_guid);
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
                List<EntityOperationResult<security_location>> entityOperationResults = new List<EntityOperationResult<security_location>>();
                var entityOperationResult = await _securityLocationService.Delete(id, user_guid);
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
