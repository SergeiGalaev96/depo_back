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
    public class ServiceTypesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ServiceTypesController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IService_TypesService _service_typesService;

        public ServiceTypesController(ILogger<ServiceTypesController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IService_TypesService service_typesService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _service_typesService = service_typesService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<service_types> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] service_types service_type, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<service_types>> entityOperationResults = new List<EntityOperationResult<service_types>>();
                var entityOperationResult = await _service_typesService.CreateServiceType(service_type, user_guid);
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
                    var service_types = unitOfWork.service_types.GetList();
                    return Ok(service_types);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetsServiceTypes");
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
                    var service_type = unitOfWork.service_types.Get(id);
                    return Ok(service_type);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetServiceType");
                return Ok(ex.ToString());
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] service_types service_type, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<service_types>> entityOperationResults = new List<EntityOperationResult<service_types>>();
                var entityOperationResult = await _service_typesService.UpdateServiceType(service_type, user_guid);
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
                List<EntityOperationResult<service_types>> entityOperationResults = new List<EntityOperationResult<service_types>>();
                var entityOperationResult = await _service_typesService.DeleteServiceType(id, user_guid);
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

