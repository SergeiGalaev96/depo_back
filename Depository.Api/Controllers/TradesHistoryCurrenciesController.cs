using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using Depository.DAL.DbContext;
using Depository.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Depository.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TradesHistoryCurrenciesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TradesHistoryCurrenciesController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly ITrades_History_CurrenciesService _trades_history_currenciesService;
        public TradesHistoryCurrenciesController(ILogger<TradesHistoryCurrenciesController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, ITrades_History_CurrenciesService trades_history_currenciesService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _trades_history_currenciesService = trades_history_currenciesService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<trades_history_currencies> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] trades_history_currencies trades_history_currency, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<trades_history_currencies>> entityOperationResults = new List<EntityOperationResult<trades_history_currencies>>();
                var entityOperationResult = await _trades_history_currenciesService.Create(trades_history_currency, user_guid);
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
                    var trades_history_currencies = unitOfWork.trades_history_currencies.GetList();
                    return Ok(trades_history_currencies);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Gettrades_history_currencies");
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
                    var trades_history_currency = unitOfWork.trades_history_currencies.Get(id);
                    return Ok(trades_history_currency);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Gettrades_history_currency");
                return Ok(ex.ToString());
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] trades_history_currencies trades_history_currency, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<trades_history_currencies>> entityOperationResults = new List<EntityOperationResult<trades_history_currencies>>();
                var entityOperationResult = await _trades_history_currenciesService.Update(trades_history_currency, user_guid);
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
                List<EntityOperationResult<trades_history_currencies>> entityOperationResults = new List<EntityOperationResult<trades_history_currencies>>();
                var entityOperationResult = await _trades_history_currenciesService.Delete(id, user_guid);
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
