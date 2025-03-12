using Microsoft.Extensions.Configuration;
using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using Depository.DAL.DbContext;
using Depository.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Depository.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    public class IncomingPackagesController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<IncomingPackagesController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IIncoming_PackagesService _incoming_PackagesService;
        private readonly ITrades_History_SecuritiesService _trades_History_SecuritiesService;
        private readonly IOrders_History_SecuritiesService _orders_History_SecuritiesService;
        private readonly ITrades_History_CurrenciesService _trades_History_CurrenciesService;

        public IncomingPackagesController(ILogger<IncomingPackagesController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IIncoming_PackagesService incoming_PackagesService, ITrades_History_SecuritiesService trades_History_SecuritiesService, IOrders_History_SecuritiesService orders_History_SecuritiesService, ITrades_History_CurrenciesService trades_History_CurrenciesService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _incoming_PackagesService = incoming_PackagesService;
            _trades_History_SecuritiesService = trades_History_SecuritiesService;
            _orders_History_SecuritiesService = orders_History_SecuritiesService;
            _trades_History_CurrenciesService = trades_History_CurrenciesService;
        }


        private async Task WriteToLogEntityOperationResult(EntityOperationResult<incoming_packages> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] incoming_packages incoming_package, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<incoming_packages>> entityOperationResults = new List<EntityOperationResult<incoming_packages>>();
                var entityOperationResult = await _incoming_PackagesService.Create(incoming_package, user_guid);
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
                    var transfer_orders = unitOfWork.incoming_packages.GetList();
                    return Ok(transfer_orders);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GeTraw_trade_securities");
                return Ok(ex.ToString());
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetBySector(int sector_id)
        {
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var incoming_packages = unitOfWork.incoming_packages.GetBySector(sector_id);
                    return Ok(incoming_packages);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetBySector");
                return Ok(ex.ToString());
            }
        }

        /// <summary>
        /// Выдает список необработанных отправленных биржой ценных бумаг.
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public async Task<IActionResult> GetRawBySector(int sector_id)
        {
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var transfer_orders = unitOfWork.incoming_packages.GetRawBySector(sector_id);
                    return Ok(transfer_orders);
                }
            }
            catch(Exception ex)
            {
                WriteToLogException(ex, "GetRawBySector");
                return Ok(ex.ToString());
            }
        }

        /// <summary>
        /// Переносит необработанные ЦБ в trade_history_securities/trade_history_currencies/order_history_securities.Устанавливает метку обработки ЦБ в  raw_trade_securities.
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        public async Task<IActionResult>Import([FromBody] incoming_packages incoming_package, Guid user_guid)
        {
            List<EntityOperationResult<trades_history_securities>> trade_History_SecuritiesOperationResults = new List<EntityOperationResult<trades_history_securities>>();
            List<EntityOperationResult<orders_history_securities>> orders_History_SecuritiesOperationResults = new List<EntityOperationResult<orders_history_securities>>();
            List<EntityOperationResult<trades_history_currencies>> trades_History_CurrenciesOperationResults = new List<EntityOperationResult<trades_history_currencies>>();

            try
            {
                    var trade_history_security = JsonConvert.DeserializeObject<trades_history_securities>(incoming_package.trd_str);
                    var trd_EntityOperationResult=await _trades_History_SecuritiesService.Create(trade_history_security, user_guid);
                    if (trd_EntityOperationResult.IsSuccess)
                    {
                        trade_History_SecuritiesOperationResults.Add(trd_EntityOperationResult);
                    }
                    incoming_package.processed = true;
                    incoming_package.trd_transfer_result = JsonConvert.SerializeObject(trade_History_SecuritiesOperationResults);

                    var orders_history_security = JsonConvert.DeserializeObject<orders_history_securities>(incoming_package.ord_str);
                    var ord_EntityOperationResult = await _orders_History_SecuritiesService.CreateOrders_History_Security(orders_history_security, user_guid);

                    if (ord_EntityOperationResult.IsSuccess)
                    {
                        orders_History_SecuritiesOperationResults.Add(ord_EntityOperationResult);
                    }
                    incoming_package.trd_transfer_result = JsonConvert.SerializeObject(trade_History_SecuritiesOperationResults);
                   

                    var trade_history_currency = JsonConvert.DeserializeObject<trades_history_currencies>(incoming_package.ordds_str);
                    var trdds_EntityOperationResult = await _trades_History_CurrenciesService.Create(trade_history_currency, user_guid);
                    if (trdds_EntityOperationResult.IsSuccess)
                    {
                        trades_History_CurrenciesOperationResults.Add(trdds_EntityOperationResult);
                    }
                    incoming_package.ordds_transfer_result= JsonConvert.SerializeObject(trades_History_CurrenciesOperationResults);
                    await _incoming_PackagesService.Update(incoming_package, user_guid);

                return Ok(trade_History_SecuritiesOperationResults);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "TransferToTHS");
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
                    var raw_trade_security = unitOfWork.incoming_packages.Get(id);
                    return Ok(raw_trade_security);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Getraw_trade_securities");
                return Ok(ex.ToString());
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] incoming_packages incoming_package, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<incoming_packages>> entityOperationResults = new List<EntityOperationResult<incoming_packages>>();
                var entityOperationResult = await _incoming_PackagesService.Update(incoming_package, user_guid);
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
                List<EntityOperationResult<incoming_packages>> entityOperationResults = new List<EntityOperationResult<incoming_packages>>();
                var entityOperationResult = await _incoming_PackagesService.Delete(id, user_guid);
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
