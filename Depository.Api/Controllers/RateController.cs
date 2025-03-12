using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
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

    public class RateController : ControllerBase
    {
        private IHttpClientFactory _httpClientFactory;
        private readonly ILogger<RateController> _logger;
        private readonly IExchange_RatesService _exchange_RatesService;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;

        public RateController(ILogger<RateController> logger, IHttpClientFactory httpClientFactory, IUnitOfWorkFactory unitOfWorkFactory, IExchange_RatesService exchange_RatesService, IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _exchange_RatesService = exchange_RatesService;
            _unitOfWorkFactory = unitOfWorkFactory;
            _configuration = configuration;
        }


        private async Task WriteToLogEntityOperationResult(EntityOperationResult<exchange_rates> entityOperationResult, string action)
        {
            if (entityOperationResult.IsSuccess)
                _logger.LogInformation(action + " : " + JsonConvert.SerializeObject(entityOperationResult));
            else _logger.LogError(action + " : " + JsonConvert.SerializeObject(entityOperationResult));
        }

        private async Task WriteToLogException(Exception exception, string action)
        {
            _logger.LogError(action + " : " + exception.ToString());
        }

        [HttpGet]
        public async Task<IActionResult> Gets()
        {
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var exchange_Rates = unitOfWork.exchange_rates.GetList();
                    return Ok(exchange_Rates);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetsRate");
                return Ok(ex.ToString());
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetTodayRate()
        {
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var exchange_Rates = unitOfWork.exchange_rates.GetTodayRate();
                    return Ok(exchange_Rates);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetTodayRate");
                return Ok(ex.ToString());
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetCurrentRates()
        {
            try
            {
                List<EntityOperationResult<exchange_rates>> entityOperationResults = new List<EntityOperationResult<exchange_rates>>();
                var bankLink = _configuration.GetValue<string>("Links:BankLink");
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(bankLink);
                string result = await client.GetStringAsync("");
                var rates = SerializeRates(result);
                foreach (var rate in rates.Currency)
                {
                    using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                    {
                        var currentRateUnit = unitOfWork.currencies.GetIdByCode(rate.ISOCode);
                        if (currentRateUnit != null)
                        {
                            exchange_rates exchange_Rates = new exchange_rates { date = DateTime.ParseExact(rates.Date, "dd.MM.yyyy", null), currency = currentRateUnit.id, rate = Double.Parse(Regex.Replace(rate.Value, "[.,]", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)), created_at = DateTime.Now, updated_at = DateTime.Now };
                            var entityOperationResult = await _exchange_RatesService.CreateExchangeRates(exchange_Rates);
                            entityOperationResults.Add(entityOperationResult);
                        }

                    }
                }
                return Ok(entityOperationResults);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetCurrentRates");
                return BadRequest();
            }
        }


        private CurrencyRates SerializeRates(string rawRate)
        {
            CurrencyRates currencyRates;
            XmlSerializer formatter = new XmlSerializer(typeof(CurrencyRates));
            using (TextReader reader = new StringReader(rawRate))
            {
                currencyRates = (CurrencyRates)formatter.Deserialize(reader);
            }
            return currencyRates;
        }
    }
}
