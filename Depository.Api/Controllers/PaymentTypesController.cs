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
    public class PaymentTypesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PaymentTypesController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IPaymentTypesService _paymentTypesService;
        public PaymentTypesController(ILogger<PaymentTypesController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IPaymentTypesService paymentTypesService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _paymentTypesService = paymentTypesService;
        }


        private async Task WriteToLogEntityOperationResult(EntityOperationResult<payment_types> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] payment_types payment_type, Guid user_guid)
        {
            try
            {

                List<EntityOperationResult<payment_types>> entityOperationResults = new List<EntityOperationResult<payment_types>>();
                var entityOperationResult = await _paymentTypesService.Create(payment_type, user_guid);
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
                    var payment_types = unitOfWork.payment_types.GetList();
                    return Ok(payment_types);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "gets");
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
                    var payment_type = unitOfWork.payment_types.Get(id);
                    return Ok(payment_type);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "get");
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] payment_types payment_type, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<payment_types>> entityOperationResults = new List<EntityOperationResult<payment_types>>();
                var entityOperationResult = await _paymentTypesService.Update(payment_type, user_guid);
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
        public async Task<ActionResult> Delete([FromBody] int? id, [FromBody] Guid user_guid)
        {
            try
            {
                if (id == null) return BadRequest();
                List<EntityOperationResult<payment_types>> entityOperationResults = new List<EntityOperationResult<payment_types>>();
                var entityOperationResult = await _paymentTypesService.Delete(id, user_guid);
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
