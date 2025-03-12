using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using Depository.DAL.DbContext;
using Depository.DAL.DbContext.Repositories;
using Depository.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Depository.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    public class PaymentsForCdServicesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PaymentsForCdServicesController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IPayments_For_Cd_ServicesService _payments_For_Cd_ServicesService;
        public PaymentsForCdServicesController(ILogger<PaymentsForCdServicesController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IPayments_For_Cd_ServicesService payments_For_Cd_ServicesService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _payments_For_Cd_ServicesService = payments_For_Cd_ServicesService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<payments_for_cd_services> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] payments_for_cd_services payment_for_cd_services, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<payments_for_cd_services>> entityOperationResults = new List<EntityOperationResult<payments_for_cd_services>>();
                var entityOperationResult = await _payments_For_Cd_ServicesService.CreatePayment_For_Cd_Services(payment_for_cd_services, user_guid);
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
                    var payments_for_cd_services = unitOfWork.payments_for_cd_services.GetList();
                    return Ok(payments_for_cd_services);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetPayments_for_cd_services");
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
                    var payment_for_cd_services = unitOfWork.payments_for_cd_services.Get(id);
                    return Ok(payment_for_cd_services);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetPayment_for_cd_services");
                return Ok(ex.ToString());
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] payments_for_cd_services payment_for_cd_services, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<payments_for_cd_services>> entityOperationResults = new List<EntityOperationResult<payments_for_cd_services>>();
                var entityOperationResult = await _payments_For_Cd_ServicesService.UpdatePayment_For_Cd_Services(payment_for_cd_services, user_guid);
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
                List<EntityOperationResult<payments_for_cd_services>> entityOperationResults = new List<EntityOperationResult<payments_for_cd_services>>();
                var entityOperationResult = await _payments_For_Cd_ServicesService.DeletePayment_For_Cd_Services(id, user_guid);
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
