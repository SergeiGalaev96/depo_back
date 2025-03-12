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
    public class TransitChargeForCdServicesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TransitChargeForCdServicesController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly ITransit_Charge_For_Cd_ServicesService _transit_Charge_For_Cd_ServicesService;
        public TransitChargeForCdServicesController(ILogger<TransitChargeForCdServicesController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, ITransit_Charge_For_Cd_ServicesService transit_Charge_For_Cd_ServicesService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _transit_Charge_For_Cd_ServicesService = transit_Charge_For_Cd_ServicesService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<transit_charge_for_cd_services> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] transit_charge_for_cd_services transit_charge_for_cd_service, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<transit_charge_for_cd_services>> entityOperationResults = new List<EntityOperationResult<transit_charge_for_cd_services>>();
                var entityOperationResult = await _transit_Charge_For_Cd_ServicesService.CreateTransit_Charge_For_Cd_Service(transit_charge_for_cd_service, user_guid);
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
                    var transit_charge_for_cd_services = unitOfWork.transit_charge_for_cd_services.GetList();
                    return Ok(transit_charge_for_cd_services);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetsTransit_charge_for_cd_services");
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
                    var transit_charge_for_cd_service = unitOfWork.transit_charge_for_cd_services.Get(id);
                    return Ok(transit_charge_for_cd_service);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetTransit_charge_for_cd_service");
                return Ok(ex.ToString());
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] transit_charge_for_cd_services transit_charge_for_cd_service, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<transit_charge_for_cd_services>> entityOperationResults = new List<EntityOperationResult<transit_charge_for_cd_services>>();
                var entityOperationResult = await _transit_Charge_For_Cd_ServicesService.UpdateTransit_Charge_For_Cd_Service(transit_charge_for_cd_service, user_guid);
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
                List<EntityOperationResult<transit_charge_for_cd_services>> entityOperationResults = new List<EntityOperationResult<transit_charge_for_cd_services>>();
                var entityOperationResult = await _transit_Charge_For_Cd_ServicesService.DeleteTransit_Charge_For_Cd_Service(id, user_guid);
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
