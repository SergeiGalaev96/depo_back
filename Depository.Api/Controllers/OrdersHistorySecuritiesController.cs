using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using crypto;
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
using static Dapper.SqlMapper;

namespace Depository.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrdersHistorySecuritiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrdersHistorySecuritiesController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IStock_SecurityService _stock_securityService;
        private readonly IOrders_History_SecuritiesService _orders_history_securitiesService;
        public OrdersHistorySecuritiesController(ILogger<OrdersHistorySecuritiesController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IOrders_History_SecuritiesService orders_history_securitiesService, IStock_SecurityService stock_securityService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _stock_securityService = stock_securityService;
            _orders_history_securitiesService = orders_history_securitiesService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<orders_history_securities> entityOperationResult, string action)
        {
            if (entityOperationResult.IsSuccess)
                _logger.LogInformation(action + " : " + JsonConvert.SerializeObject(entityOperationResult));
            else _logger.LogError(action + " : " + JsonConvert.SerializeObject(entityOperationResult));
        }



        private async Task WriteToLogException(Exception exception, string action)
        {
            _logger.LogError(action + " : " + JsonConvert.SerializeObject(exception));
        }




        /// <summary>
        /// Импортирует/записывает остатки по ЦБ которые отдает Биржа после ТС
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Import([FromBody] List<orders_history_securities> orders_history_securities, DateTime order_date, Guid user_guid)
        {
            List<EntityOperationResult<orders_history_securities>> entityOperationResults = new List<EntityOperationResult<orders_history_securities>>();
            try
            {
                if (orders_history_securities != null)
                {
                    foreach (var order_history_securities in orders_history_securities)
                    {
                        order_history_securities.order_date = order_date;
                        var entityOperationResult = await _orders_history_securitiesService.CreateOrders_History_Security(order_history_securities, user_guid);

                        // entityOperationResults.Add(entityOperationResult);
                    }
                }
                entityOperationResults = RecalcAfterImport(order_date, user_guid);
                if (entityOperationResults.Count() > 0)
                {
                    return Ok(entityOperationResults);
                }
                else
                {
                    var result = EntityOperationResult<orders_history_securities>.Success(new orders_history_securities());
                    entityOperationResults.Add(result);
                    return Ok(entityOperationResults);
                }

            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Import_orders_history_securities");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }




        /// <summary>
        /// Пересчитывает остаток по ЦБ учитывая импортированных данных.
        /// </summary>

        private List<EntityOperationResult<orders_history_securities>> RecalcAfterImport(DateTime order_date, Guid user_guid)
        {
            List<EntityOperationResult<orders_history_securities>> entityOperationResults = new List<EntityOperationResult<orders_history_securities>>();
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var orders_history_securities = unitOfWork.orders_history_securities.GetByDate(order_date);
                    var stock_Securities = unitOfWork.stock_security.GetList();
                    var accounts = unitOfWork.accounts.GetList();
                    var securities = unitOfWork.securities.GetList();
                    foreach (var order_history_securities in orders_history_securities)
                    {
                        var account = accounts.Where(x => x.accnumber.Equals(order_history_securities.account_number)).FirstOrDefault();
                        var security = securities.Where(x => x.code.Equals(order_history_securities.security_code)).FirstOrDefault();
                        if (account != null && security != null)
                        {
                            var stock_security = stock_Securities.Where(x => x.account == account.id && x.security == security.id).FirstOrDefault();
                            if (stock_security != null && account != null && security != null)
                            {
                                if (stock_security.quantity_stop >= order_history_securities.quantity)
                                {
                                    var quantity_stop = stock_security.quantity_stop; // Amount to be hold on field "blocked"
                                    stock_security.quantity = quantity_stop - order_history_securities.quantity + stock_security.quantity; // Calc quantity field
                                    stock_security.quantity_stop = order_history_securities.quantity; // Calc quantity_stop field

                                    var entityOperationResult = _stock_securityService.Update(stock_security, user_guid);
                                    if (entityOperationResult.Result.IsSuccess)
                                    {
                                        order_history_securities.processed = true;
                                        var entity = _orders_history_securitiesService.UpdateOrders_History_Security(order_history_securities, user_guid);
                                        entityOperationResults.Add(entity.Result);
                                    }
                                }
                                else
                                {
                                    var entity = EntityOperationResult<orders_history_securities>.Failure()
                                   .AddError($"На счете {account.accnumber} не достаточно ЦБ для разблокирования!");
                                    entityOperationResults.Add(entity);
                                    continue;
                                }
                            }
                        }
                    }

                    foreach (var stock_security in stock_Securities)
                    {
                        var security = securities.Where(x => x.id == stock_security.security).FirstOrDefault();
                        var account = accounts.Where(x => x.id == stock_security.account).FirstOrDefault();
                        if (security != null && account != null)
                        {
                            var order_history_security = orders_history_securities.Where(x => x.account_number.Equals(account.accnumber) && x.security_code.Equals(security.code)).FirstOrDefault();
                            if (order_history_security == null && stock_security.quantity_stop > 0)
                            {
                                var quantity_stop = stock_security.quantity_stop;
                                stock_security.quantity = quantity_stop + stock_security.quantity;
                                stock_security.quantity_stop = 0;
                                var entityOperationResult = _stock_securityService.Update(stock_security, user_guid);
                                if (entityOperationResult.Result.IsSuccess)
                                {

                                    var operationResult = EntityOperationResult<orders_history_securities>
                                    .Success(new orders_history_securities());
                                    entityOperationResults.Add(operationResult);
                                }
                            }
                        }

                    }
                    return entityOperationResults;
                }
            }
            catch (Exception ex)
            {
                var entity = EntityOperationResult<orders_history_securities>
                           .Failure()
                           .AddError(JsonConvert.SerializeObject(ex));
                entityOperationResults.Add(entity);
                return entityOperationResults;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Import2([FromBody] List<orders_history_securities> import_securities, DateTime order_date, Guid user_guid)
        {
            List<orders_history_securities_output> orders_history_output_list = new List<orders_history_securities_output>();

            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var accounts = unitOfWork.accounts.GetList();
                    var securities = unitOfWork.securities.GetList();
                    var stock_Securities = unitOfWork.stock_security.GetList();

                    foreach (var stock_security in stock_Securities)
                    {
                        var security = securities.Where(x => x.id == stock_security.security).FirstOrDefault();
                        var account = accounts.Where(x => x.id == stock_security.account).FirstOrDefault();

                        var import_security = import_securities.Count() > 0 ? import_securities.Where(x => x.account_number == account.accnumber && x.security_code == security.code).FirstOrDefault() : null;
                        // var import_security = import_securities.Where(x => x.account_number == account.accnumber && x.security_code == security.code).FirstOrDefault();
                        if (import_security == null) // track doesn't exist or file is empty, unblock it
                        {
                            if (stock_security.quantity_stop > 0)
                            {
                                stock_security.quantity = stock_security.quantity_stop + stock_security.quantity;
                                stock_security.quantity_stop = 0;
                                var entityOperationResult = _stock_securityService.Update(stock_security, user_guid);// Update stock_securities
                                if (entityOperationResult.Result.IsSuccess == true)
                                {
                                    var ord_history_output = new orders_history_securities_output() { account = account.accnumber, security = security.code, quantity = stock_security.quantity, status_message = "ЦБ освобождены автоматически!" };
                                    orders_history_output_list.Add((ord_history_output));
                                }
                            }
                        }
                        else
                        {
                            if (stock_security.quantity_stop >= import_security.quantity)
                            {
                                stock_security.quantity = stock_security.quantity_stop - import_security.quantity + stock_security.quantity;
                                stock_security.quantity_stop = import_security.quantity;
                                var entityOperationResult = _stock_securityService.Update(stock_security, user_guid);// Update stock_securities

                                if (entityOperationResult.Result.IsSuccess == true)// Create history
                                {
                                    var new_ord_history = new orders_history_securities();
                                    new_ord_history = import_security;
                                    new_ord_history.order_date = order_date;
                                    new_ord_history.processed = true;

                                    await _orders_history_securitiesService.CreateOrders_History_Security(new_ord_history, user_guid);
                                    import_security.processed = true;
                                    var ord_history_output = new orders_history_securities_output() { account = account.accnumber, security = security.code, quantity = import_security.quantity, status_message = "OK" };
                                    orders_history_output_list.Add((ord_history_output));
                                }
                                else
                                {
                                    var ord_history_output = new orders_history_securities_output() { account = account.accnumber, security = security.code, quantity = import_security.quantity, status_message = JsonConvert.SerializeObject(entityOperationResult) };
                                    orders_history_output_list.Add((ord_history_output));
                                }
                            }
                            else
                            {
                                var ord_history_output = new orders_history_securities_output() { account = account.accnumber, security = security.code, quantity = import_security.quantity, status_message = "Не достаточно ЦБ для разблокирования!" };
                                orders_history_output_list.Add((ord_history_output));
                                import_security.processed = true;
                            }

                        }

                    }
                    foreach (var import_security in import_securities) // Repot tracks that not found in stock
                    {
                        if (import_security.processed != true)
                        {
                            var ord_history_output = new orders_history_securities_output() { account = import_security.account_number, security = import_security.security_code, quantity = import_security.quantity, status_message = "Счет не найден в позициях по ЦБ!" };
                            orders_history_output_list.Add((ord_history_output));
                        }

                    }
                    return Ok(orders_history_output_list);
                }
            }
            catch (Exception ex)
            {
                var ord_history_output = new orders_history_securities_output() { account = "null", security = "null", quantity = 0, status_message = JsonConvert.SerializeObject(ex) };
                orders_history_output_list.Add((ord_history_output));
                return Ok(orders_history_output_list);
            }
        }


        [HttpPost]
        public async Task<ActionResult> Create([FromBody] orders_history_securities orders_history_security, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<orders_history_securities>> entityOperationResults = new List<EntityOperationResult<orders_history_securities>>();
                var entityOperationResult = await _orders_history_securitiesService.CreateOrders_History_Security(orders_history_security, user_guid);
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
                    var orders_history_securities = unitOfWork.orders_history_securities.GetList();
                    return Ok(orders_history_securities);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Getorders_history_securities");
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
                    var orders_history_security = unitOfWork.orders_history_securities.Get(id);
                    return Ok(orders_history_security);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Getorders_history_security");
                return Ok(ex.ToString());
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] orders_history_securities orders_history_security, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<orders_history_securities>> entityOperationResults = new List<EntityOperationResult<orders_history_securities>>();
                var entityOperationResult = await _orders_history_securitiesService.UpdateOrders_History_Security(orders_history_security, user_guid);
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
                List<EntityOperationResult<orders_history_securities>> entityOperationResults = new List<EntityOperationResult<orders_history_securities>>();
                var entityOperationResult = await _orders_history_securitiesService.DeleteOrders_History_Security(id, user_guid);
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
