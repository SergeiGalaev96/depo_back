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
    public class GovSecurityPaymentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GovSecurityPaymentsController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IGovSecurityPaymentsService _govSecurityPaymentsService;
        public GovSecurityPaymentsController(ILogger<GovSecurityPaymentsController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IGovSecurityPaymentsService govSecurityPaymentsService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _govSecurityPaymentsService = govSecurityPaymentsService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<gov_securities_payments> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] gov_securities_payments gov_securities_payment, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<gov_securities_payments>> entityOperationResults = new List<EntityOperationResult<gov_securities_payments>>();
                var entityOperationResult = await _govSecurityPaymentsService.Create(gov_securities_payment, user_guid);
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
                    var gov_securities_payments = unitOfWork.gov_securities_payments.GetList();
                    return Ok(gov_securities_payments);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetBanks");
                return Ok(ex);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetBySecurityId(int security_id)
        {
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var gov_securities_payments = unitOfWork.gov_securities_payments.GetBySecurityId(security_id);
                    return Ok(gov_securities_payments);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Gets");
                return Ok(ex);
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
                    var gov_security_payment = unitOfWork.gov_securities_payments.Get(id);
                    return Ok(gov_security_payment);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetBank");
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] gov_securities_payments gov_securities_payment, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<gov_securities_payments>> entityOperationResults = new List<EntityOperationResult<gov_securities_payments>>();
                var entityOperationResult = await _govSecurityPaymentsService.Update(gov_securities_payment, user_guid);
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
        public async Task<ActionResult> Delete([FromBody] int id, Guid user_guid)
        {
            try
            {
                if (id == null) return BadRequest();
                List<EntityOperationResult<gov_securities_payments>> entityOperationResults = new List<EntityOperationResult<gov_securities_payments>>();
                var entityOperationResult = await _govSecurityPaymentsService.Delete(id, user_guid);
                entityOperationResults.Add(entityOperationResult);
                WriteToLogEntityOperationResult(entityOperationResult, "Update");
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
