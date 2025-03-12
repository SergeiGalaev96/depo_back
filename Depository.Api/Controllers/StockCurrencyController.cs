using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Depository.Api.Extentions.Enums;
using Depository.Core;
using Depository.Core.Models;
using Depository.Core.Models.DTO;
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
    public class StockCurrencyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StockCurrencyController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IStock_CurrencyService _stock_currencyService;
        private readonly IOutgoing_PackagesService _outgoing_PackagesService;

        public StockCurrencyController(ILogger<StockCurrencyController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IStock_CurrencyService stock_currencyService, IOutgoing_PackagesService outgoing_PackagesService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _stock_currencyService = stock_currencyService;
            _outgoing_PackagesService = outgoing_PackagesService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<stock_currency> entityOperationResult, string action)
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
        public async Task<IActionResult> ShowInPositions()
        {
            var show_in_positions = true;
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var stock_currencies = unitOfWork.stock_currency_views.ShowInPositions(show_in_positions);
                return Ok(stock_currencies);
            }
        }




        [HttpGet]
        public async Task<IActionResult> GetDataToExportInTS()
        {
            List<accounts> trade_accounts = new List<accounts>();
            List<currencies> currencies = new List<currencies>();
            List<model_currency_export_to_ExchangeDTO> model_currency_export_to_ExchangeDTOs = new List<model_currency_export_to_ExchangeDTO>();
            List<depositors> depositors = new List<depositors>();
            List<stock_currency> stock_Currencies = new List<stock_currency>();
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    trade_accounts = unitOfWork.accounts.GetTradeAccounts();
                    currencies = unitOfWork.currencies.GetList();
                    stock_Currencies = unitOfWork.stock_currency.GetList().Where(x => x.quantity > 0).ToList();
                    depositors = unitOfWork.depositors.GetList();
                    foreach (var stock_currency in stock_Currencies)
                    {
                        var trade_account = trade_accounts.Where(x => x.id == stock_currency.account).FirstOrDefault();
                        if (trade_account != null)
                        {
                            var depositor = depositors.Where(x => x.partner == trade_account.partner).FirstOrDefault();
                            var depositor_code = unitOfWork.trades.GetList().Where(x => x.depositor == depositor.id).FirstOrDefault().depositor_code;
                            var currency_code = currencies.Where(x => x.id == stock_currency.currency).FirstOrDefault().code;
                            model_currency_export_to_ExchangeDTOs.Add
                                (
                                new model_currency_export_to_ExchangeDTO
                                {
                                    account_number = trade_account.accnumber,
                                    currency_code = currency_code,
                                    depositor_id_from_trades = depositor_code,
                                    quantity = stock_currency.quantity
                                });
                        }
                    }
                }
                return Ok(model_currency_export_to_ExchangeDTOs);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "StockCurrency_GetDataToExportInTS");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }


        /// <summary>
        /// Блокирует остаток ДС и отправляет в КФБ
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> ExportCurrency(Guid user_guid, bool online)
        {

            var outgoing_Package = new outgoing_packages();
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var user = unitOfWork.users.GetByUserId(user_guid);
                if (user != null)
                {
                    var currencyItems = await BlockCurrency(user_guid);
                    if (online)
                    {
                        outgoing_Package = unitOfWork.outgoing_packages.GetByDateAndSector(DateTime.Now.Date, (int)SectorValues.STANDART_CURRENCY);
                        if (outgoing_Package != null)
                        {
                            outgoing_Package.pos_str = JsonConvert.SerializeObject(currencyItems);
                            outgoing_Package.pos_result = await SendPostQueryCurrency(currencyItems.ToArray());
                            _outgoing_PackagesService.Update(outgoing_Package, user_guid);
                        }
                        else
                        {
                            outgoing_Package = new outgoing_packages();
                            outgoing_Package.sector_id = (int)SectorValues.STANDART_CURRENCY;
                            outgoing_Package.pos_str = JsonConvert.SerializeObject(currencyItems);
                            outgoing_Package.pos_result = await SendPostQueryCurrency(currencyItems.ToArray());
                            _outgoing_PackagesService.Create(outgoing_Package, user_guid);
                        }

                    }
                }
            }
            return Ok(EntityOperationResult<outgoing_packages>.Success(outgoing_Package));
        }


        private async Task<List<CurrencyItem>> BlockCurrency(Guid user_guid)
        {

            List<CurrencyItem> currencyItems = new List<CurrencyItem>();
            List<EntityOperationResult<stock_currency>> entityOperationResultList = new List<EntityOperationResult<stock_currency>>();
            List<accounts> trade_accounts = new List<accounts>();
            List<stock_currency> stock_Currencies = new List<stock_currency>();
            List<currencies> currencies = new List<currencies>();
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    trade_accounts = unitOfWork.accounts.GetTradeAccounts();
                    currencies = unitOfWork.currencies.GetList();
                    stock_Currencies = unitOfWork.stock_currency.GetBlockedList();
                    foreach (var stock_currency in stock_Currencies)
                    {
                        var trade_account = trade_accounts.Where(x => x.id == stock_currency.account).FirstOrDefault();
                        var currency = currencies.Where(x => x.id == stock_currency.currency).FirstOrDefault();
                        if (trade_account != null && currency != null)
                        {
                            var quantity = stock_currency.quantity;
                            stock_currency.quantity_stop += quantity;
                            stock_currency.quantity = 0;
                            CurrencyItem currencyItem = new CurrencyItem
                            {
                                account_number = trade_account.accnumber,
                                currency_code = currency.code,
                                quantity = quantity
                                // quantity = stock_currency.quantity_stop
                                //  quantity_stop = stock_currency.quantity_stop
                            };
                            currencyItems.Add(currencyItem);
                            var entity = await _stock_currencyService.Update(stock_currency, user_guid);
                            entityOperationResultList.Add(entity);
                        }

                    }
                }
                return currencyItems;
            }
            catch (Exception ex)
            {
                throw new Exception("Произошла ошибка во время операции: " + JsonConvert.SerializeObject(ex));
            }

        }


        private async Task<string> SendPostQueryCurrency(object[] objectArray)
        {
            try
            {
                var url = _configuration.GetValue<string>("Links:ExportToExchangeStockCurrencyLink");
                var client = new HttpClient();
                client.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string json = JsonConvert.SerializeObject(objectArray);
                StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, data);
                client.Dispose();
                string result = await response.Content.ReadAsStringAsync();
                return result;
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportInTS(Guid user_guid)
        {
            List<EntityOperationResult<stock_currency>> entityOperationResultList = new List<EntityOperationResult<stock_currency>>();
            List<accounts> trade_accounts = new List<accounts>();
            List<stock_currency> stock_Currencies = new List<stock_currency>();
            List<currencies> currencies = new List<currencies>();
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    trade_accounts = unitOfWork.accounts.GetTradeAccounts();
                    currencies = unitOfWork.currencies.GetList();
                    stock_Currencies = unitOfWork.stock_currency.GetList();
                    foreach (var stock_currency in stock_Currencies)
                    {
                        var trade_account = trade_accounts.Where(x => x.id == stock_currency.account).FirstOrDefault();
                        if (trade_account != null)
                        {
                            var quantity = stock_currency.quantity;
                            stock_currency.quantity_stop = quantity;
                            stock_currency.quantity = 0;
                            var entity = await _stock_currencyService.Update(stock_currency, user_guid);
                            entityOperationResultList.Add(entity);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                entityOperationResultList.Add(EntityOperationResult<stock_currency>
                            .Failure()
                            .AddError(JsonConvert.SerializeObject(ex)));

            }
            return Ok(entityOperationResultList);
        }

        [HttpGet]
        public async Task<EntityOperationResult<stock_currency>> Blocking(int stock_currency_id, int quantity, Guid user_guid)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var stock_currency = unitOfWork.stock_currency.Get(stock_currency_id);
                if (stock_currency != null && stock_currency.quantity >= quantity)
                {
                    stock_currency.quantity = stock_currency.quantity - quantity;
                    stock_currency.quantity_stop = stock_currency.quantity_stop + quantity;
                    var entity = await _stock_currencyService.Update(stock_currency, user_guid);
                    return entity;
                }
                else return EntityOperationResult<stock_currency>
                              .Failure()
                              .AddError($"Does not exist stock_currency_id or quantity is not enough");
            }
        }


        [HttpGet]
        public async Task<IActionResult> Gets()
        {
            List<stock_currencyDTO> stock_Currencies = new List<stock_currencyDTO>();
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var accounts = unitOfWork.accounts.GetList();
                    var currencies = unitOfWork.currencies.GetList();
                    var stock_currencyList = unitOfWork.stock_currency.GetList();
                    foreach (var stock_currency in stock_currencyList)
                    {
                        stock_currencyDTO stock_currencyDTO = new stock_currencyDTO();
                        stock_currencyDTO.id = stock_currency.id;
                        stock_currencyDTO.account_id = stock_currency.account;
                        stock_currencyDTO.quantity_pledge = stock_currency.quantity_pledge;
                        if (stock_currency.account != null)
                        {
                            var account = accounts.Where(x => x.id == stock_currency.account).FirstOrDefault();
                            if (account != null)
                            {
                                stock_currencyDTO.account = account.accnumber;
                                stock_currencyDTO.partner = account.partner;
                            }
                            else stock_currencyDTO.account = "";
                        }
                        else stock_currencyDTO.account = "";
                        if (stock_currency.currency != null)
                        {
                            var currency = currencies.Where(x => x.id == stock_currency.currency).FirstOrDefault();
                            if (currency != null)
                            {
                                stock_currencyDTO.currency = currency.code;
                            }
                            else stock_currencyDTO.currency = "";
                        }
                        else stock_currencyDTO.currency = "";
                        stock_currencyDTO.quantity = stock_currency.quantity;
                        stock_currencyDTO.quantity_stop = stock_currency.quantity_stop;
                        stock_Currencies.Add(stock_currencyDTO);
                    }
                    return Ok(stock_Currencies);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetStock_currency");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] stock_currency stock_currency, Guid user_guid)
        {
            if (stock_currency.quantity == null)
            {
                stock_currency.quantity = 0;
            }
            if (stock_currency.quantity_stop == null)
            {
                stock_currency.quantity_stop = 0;
            }
            if (stock_currency.quantity_pledge == null)
            {
                stock_currency.quantity_pledge = 0;
            }
            try
            {
                List<EntityOperationResult<stock_currency>> entityOperationResults = new List<EntityOperationResult<stock_currency>>();
                var entityOperationResult = await _stock_currencyService.Create(stock_currency, user_guid);
                entityOperationResults.Add(entityOperationResult);
                WriteToLogEntityOperationResult(entityOperationResult, "Create");
                return Ok(entityOperationResults);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Create");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }


        [HttpPost]
        public async Task<ActionResult> Update([FromBody] stock_currency stock_currency, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<stock_currency>> entityOperationResults = new List<EntityOperationResult<stock_currency>>();
                var entityOperationResult = await _stock_currencyService.Update(stock_currency, user_guid);
                entityOperationResults.Add(entityOperationResult);
                WriteToLogEntityOperationResult(entityOperationResult, "Update");
                return Ok(entityOperationResults);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Update");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }
    }
}
