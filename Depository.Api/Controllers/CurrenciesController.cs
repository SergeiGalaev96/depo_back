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
    [ApiController]
    public class CurrenciesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CurrenciesController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly  ICurrenciesService _currenciesService;
        public CurrenciesController(ILogger<CurrenciesController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, ICurrenciesService currenciesService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _currenciesService = currenciesService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<currencies> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] currencies currency, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<currencies>> entityOperationResults = new List<EntityOperationResult<currencies>>();
                var entityOperationResult = await _currenciesService.CreateCurrency(currency, user_guid);
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
                    var currencies = unitOfWork.currencies.GetList();
                    return Ok(currencies);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetCurrencies");
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
                    var currency = unitOfWork.currencies.Get(id);
                    return Ok(currency);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetCurrency");
                return BadRequest();
            }
        }


        [HttpPost]
        public async Task<ActionResult> Update([FromBody] currencies currency, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<currencies>> entityOperationResults = new List<EntityOperationResult<currencies>>();
                var entityOperationResult = await _currenciesService.UpdateCurrency(currency, user_guid);
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
                List<EntityOperationResult<currencies>> entityOperationResults = new List<EntityOperationResult<currencies>>();
                var entityOperationResult = await _currenciesService.DeleteCurrency(id, user_guid);
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
