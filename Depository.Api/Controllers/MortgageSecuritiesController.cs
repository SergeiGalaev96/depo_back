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
    public class MortgageSecuritiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MortgageSecuritiesController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IMortgage_SecuritiesService _mortgage_SecurityService;
        public MortgageSecuritiesController(ILogger<MortgageSecuritiesController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IMortgage_SecuritiesService mortgage_SecurityService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _mortgage_SecurityService = mortgage_SecurityService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<mortgage_securities> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] mortgage_securities mortgage_security, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<mortgage_securities>> entityOperationResults = new List<EntityOperationResult<mortgage_securities>>();
                var entityOperationResult = await _mortgage_SecurityService.Create(mortgage_security, user_guid);
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
                    var mortgage_securities = unitOfWork.mortgage_securities.GetList();
                    return Ok(mortgage_securities);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetMortgagesSecurities");
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
                    var mortgage_security = unitOfWork.mortgage_securities.Get(id);
                    return Ok(mortgage_security);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetMortgagesSecurity");
                return Ok(ex.ToString());
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] mortgage_securities mortgage_security, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<mortgage_securities>> entityOperationResults = new List<EntityOperationResult<mortgage_securities>>();
                var entityOperationResult = await _mortgage_SecurityService.Update(mortgage_security, user_guid);
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
                List<EntityOperationResult<mortgage_securities>> entityOperationResults = new List<EntityOperationResult<mortgage_securities>>();
                var entityOperationResult = await _mortgage_SecurityService.Delete(id, user_guid);
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
