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
    public class ServiceGroupsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ServiceGroupsController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IService_GroupsService _service_groupsService;

        public ServiceGroupsController(ILogger<ServiceGroupsController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IService_GroupsService service_groupsService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _service_groupsService = service_groupsService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<service_groups> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] service_groups service_group, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<service_groups>> entityOperationResults = new List<EntityOperationResult<service_groups>>();
                var entityOperationResult = await _service_groupsService.CreateServiceGroup(service_group, user_guid);
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
                    var service_groups = unitOfWork.service_groups.GetList();
                    return Ok(service_groups);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetsServiceGroups");
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
                    var service_group = unitOfWork.service_groups.Get(id);
                    return Ok(service_group);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetServiceGroup");
                return Ok(ex.ToString());
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] service_groups service_group, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<service_groups>> entityOperationResults = new List<EntityOperationResult<service_groups>>();
                var entityOperationResult = await _service_groupsService.UpdateServiceGroup(service_group, user_guid);
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
                List<EntityOperationResult<service_groups>> entityOperationResults = new List<EntityOperationResult<service_groups>>();
                var entityOperationResult = await _service_groupsService.DeleteServiceGroup(id, user_guid);
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
