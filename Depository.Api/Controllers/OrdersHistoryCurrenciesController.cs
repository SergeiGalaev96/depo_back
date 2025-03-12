using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
using static Dapper.SqlMapper;
using Depository.Core.Models.DTO;
using System.Security.Principal;
using System.Text.Json;

namespace Depository.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrdersHistoryCurrenciesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrdersHistoryCurrenciesController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IStock_CurrencyService _stock_CurrencyService;
        private readonly IOrders_History_CurrenciesService _orders_history_currenciesService;
        public OrdersHistoryCurrenciesController(ILogger<OrdersHistoryCurrenciesController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IOrders_History_CurrenciesService orders_history_currenciesService, IStock_CurrencyService stock_CurrencyService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _stock_CurrencyService = stock_CurrencyService;
            _orders_history_currenciesService = orders_history_currenciesService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<orders_history_currencies> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] orders_history_currencies orders_history_currency, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<orders_history_currencies>> entityOperationResults = new List<EntityOperationResult<orders_history_currencies>>();
                var entityOperationResult = await _orders_history_currenciesService.CreateOrders_History_Currency(orders_history_currency, user_guid);
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
                    var orders_history_currencies = unitOfWork.orders_history_currencies.GetList();
                    return Ok(orders_history_currencies);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Getorders_history_currencies");
                return Ok(ex.ToString());
            }
        }

        [HttpPost]
        public async Task<IActionResult> Import([FromBody] List<orders_history_currencies> orders_history_currencies, DateTime order_date, Guid user_guid)
        {
            List<EntityOperationResult<orders_history_currencies>> entityOperationResults = new List<EntityOperationResult<orders_history_currencies>>();
            try
            {
                foreach (var order_history_currencies in orders_history_currencies)
                {
                    order_history_currencies.order_date = order_date;
                    var entityOperationResult = await _orders_history_currenciesService.CreateOrders_History_Currency(order_history_currencies, user_guid);

                    entityOperationResults.Add(entityOperationResult);
                    var error = entityOperationResults.Where(x => x.Errors != null).FirstOrDefault();
                    if (error != null) return Ok(entityOperationResults);

                }
                entityOperationResults = RecalcAfterImport(order_date, user_guid);
                return Ok(entityOperationResults);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Import_orders_history_currencies");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }

        private List<EntityOperationResult<orders_history_currencies>> RecalcAfterImport(DateTime order_date, Guid user_guid)
        {
            List<EntityOperationResult<orders_history_currencies>> entityOperationResults = new List<EntityOperationResult<orders_history_currencies>>();

            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var orders_history_currencies = unitOfWork.orders_history_currencies.GetByDate(order_date);
                    var stock_Currencies = unitOfWork.stock_currency.GetList();
                    if (orders_history_currencies.Count() == 0)
                    {
                        orders_history_currencies orders_history_currency = new orders_history_currencies();
                        var entity = EntityOperationResult<orders_history_currencies>.Success(orders_history_currency);
                        entityOperationResults.Add(entity);
                    }
                    var accounts = unitOfWork.accounts.GetList();
                    var currencies = unitOfWork.currencies.GetList();

                    foreach (var order_history_currencies in orders_history_currencies) // Calc new stock amounts
                    {
                        var account = accounts.Where(x => x.accnumber.Equals(order_history_currencies.account_number)).FirstOrDefault();
                        var currency = currencies.Where(x => x.code.Equals(order_history_currencies.currency_code)).FirstOrDefault();
                        var stock_currency = stock_Currencies.Where(x => x.account == account.id && x.currency == currency.id).FirstOrDefault();
                        if (stock_currency == null)
                        {
                            var entity = EntityOperationResult<orders_history_currencies>.Failure()
                           .AddError($"order_history_currencies code={order_history_currencies.currency_code} не существует в позициях ДС");
                            entityOperationResults.Add(entity);
                            continue;
                        }
                        if (stock_currency.quantity_stop >= order_history_currencies.quantity)
                        {
                            var quantity_stop = order_history_currencies.quantity; // Amount to be hold on field "blocked"
                            stock_currency.quantity = stock_currency.quantity_stop - quantity_stop + stock_currency.quantity; // Calc quantity field
                            stock_currency.quantity_stop = quantity_stop; // Calc quantity_stop field
                            var entityOperationResult = _stock_CurrencyService.Update(stock_currency, user_guid);
                            if (entityOperationResult.Result.IsSuccess)
                            {
                                order_history_currencies.processed = true;
                                var entity = _orders_history_currenciesService.UpdateOrders_History_Currency(order_history_currencies, user_guid);
                                entityOperationResults.Add(entity.Result);
                            }
                        }
                        else
                        {

                            var entity = EntityOperationResult<orders_history_currencies>.Failure()
                           .AddError($"На счете {account.accnumber} не достаточно ДС для разблокирования!");
                            entityOperationResults.Add(entity);
                            continue;
                        }


                    }
                    foreach (var stock_currency in stock_Currencies)
                    {
                        var currency = currencies.Where(x => x.id == stock_currency.currency).FirstOrDefault();
                        var account = accounts.Where(x => x.id == stock_currency.account).FirstOrDefault();
                        if (currency != null && account != null)
                        {
                            var orders_history_currency = orders_history_currencies.Where(x => x.account_number.Equals(account.accnumber) && x.currency_code.Equals(currency.code)).FirstOrDefault();
                            if (orders_history_currency == null && stock_currency.quantity_stop > 0)
                            {
                                var quantity_stop = stock_currency.quantity_stop;
                                stock_currency.quantity = quantity_stop + stock_currency.quantity;
                                stock_currency.quantity_stop = 0;
                                var entityOperationResult = _stock_CurrencyService.Update(stock_currency, user_guid);

                            }
                        }
                    }
                    return entityOperationResults;
                }
            }
            catch (Exception ex)
            {
                var entity = EntityOperationResult<orders_history_currencies>
                          .Failure()
                          .AddError(JsonConvert.SerializeObject(ex));
                entityOperationResults.Add(entity);
                return entityOperationResults;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Import2([FromBody] List<orders_history_currencies> import_currencies, DateTime order_date, Guid user_guid)
        {
            List<orders_history_currencies_output> orders_history_output_list = new List<orders_history_currencies_output>();

            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var accounts = unitOfWork.accounts.GetList();
                    var currencies = unitOfWork.currencies.GetList();
                    var stock_Currencies = unitOfWork.stock_currency.GetList();

                    foreach (var stock_currency in stock_Currencies)
                    {
                        var account = accounts.Where(x => x.id == stock_currency.account).FirstOrDefault();
                        var currency = currencies.Where(x => x.id == stock_currency.currency).FirstOrDefault();

                        var import_currency = import_currencies.Where(x => x.account_number == account.accnumber && x.currency_code == currency.code).FirstOrDefault();
                        if (import_currency == null) // track doesn't exist or file is empty, unblock it
                        {
                            if (stock_currency.quantity_stop > 0)
                            {
                                stock_currency.quantity = stock_currency.quantity_stop + stock_currency.quantity;
                                stock_currency.quantity_stop = 0;
                                var entityOperationResult = _stock_CurrencyService.Update(stock_currency, user_guid);// Update stock_currencies
                                if (entityOperationResult.Result.IsSuccess == true)
                                {
                                    var ord_history_output = new orders_history_currencies_output() { account = account.accnumber, currency = currency.code, quantity = stock_currency.quantity, status_message = "ДС освобождены автоматически!" };
                                    orders_history_output_list.Add((ord_history_output));
                                }
                            }
                        }
                        else
                        {
                            if (stock_currency.quantity_stop >= import_currency.quantity)
                            {
                                stock_currency.quantity = stock_currency.quantity_stop - import_currency.quantity + stock_currency.quantity;
                                stock_currency.quantity_stop = import_currency.quantity;
                                var entityOperationResult = _stock_CurrencyService.Update(stock_currency, user_guid);// Update stock_currencies

                                if (entityOperationResult.Result.IsSuccess == true)// Create history
                                {
                                    var new_ord_history = new orders_history_currencies();
                                    new_ord_history = import_currency;
                                    new_ord_history.order_date = order_date;
                                    new_ord_history.processed = true;

                                    await _orders_history_currenciesService.CreateOrders_History_Currency(new_ord_history, user_guid);
                                    import_currency.processed = true;
                                    var ord_history_output = new orders_history_currencies_output() { account = account.accnumber, currency = currency.code, quantity = import_currency.quantity, status_message = "OK" };
                                    orders_history_output_list.Add((ord_history_output));
                                }
                                else
                                {
                                    var ord_history_output = new orders_history_currencies_output() { account = account.accnumber, currency = currency.code, quantity = import_currency.quantity, status_message = JsonConvert.SerializeObject(entityOperationResult) };
                                    orders_history_output_list.Add((ord_history_output));
                                }
                            }
                            else
                            {
                                var ord_history_output = new orders_history_currencies_output() { account = account.accnumber, currency = currency.code, quantity = import_currency.quantity, status_message = "Не достаточно ДС для разблокирования!" };
                                orders_history_output_list.Add((ord_history_output));
                                import_currency.processed = true;
                            }

                        }

                    }
                    foreach (var import_Currency in import_currencies) // Repot tracks that not found in stock
                    {
                        if (import_Currency.processed != true)
                        {
                            var ord_history_output = new orders_history_currencies_output() { account = import_Currency.account_number, currency = import_Currency.currency_code, quantity = import_Currency.quantity, status_message = "Счет не найден в позициях по ДС!" };
                            orders_history_output_list.Add((ord_history_output));
                        }

                    }
                    return Ok(orders_history_output_list);
                }
            }
            catch (Exception ex)
            {
                var ord_history_output = new orders_history_currencies_output() { account = "null", currency = "null", quantity = 0, status_message = JsonConvert.SerializeObject(ex) };
                orders_history_output_list.Add((ord_history_output));
                return Ok(orders_history_output_list);
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
                    var orders_history_currency = unitOfWork.orders_history_currencies.Get(id);
                    return Ok(orders_history_currency);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Getorders_history_currency");
                return Ok(ex.ToString());
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] orders_history_currencies orders_history_currency, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<orders_history_currencies>> entityOperationResults = new List<EntityOperationResult<orders_history_currencies>>();
                var entityOperationResult = await _orders_history_currenciesService.UpdateOrders_History_Currency(orders_history_currency, user_guid);
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
                List<EntityOperationResult<orders_history_currencies>> entityOperationResults = new List<EntityOperationResult<orders_history_currencies>>();
                var entityOperationResult = await _orders_history_currenciesService.DeleteOrders_History_Currency(id, user_guid);
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
