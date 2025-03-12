using AutoMapper.Configuration;
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
    public class KSExchangesController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<StockSecurityController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly ITrades_History_SecuritiesService _trades_History_SecuritiesService;
        private readonly IOrders_History_SecuritiesService _orders_History_SecuritiesService;
        private readonly ITrades_History_CurrenciesService _trades_History_CurrenciesService;
        private readonly IIncoming_PackagesService _incoming_PackagesService;

        public KSExchangesController(IConfiguration configuration,
            ITrades_History_SecuritiesService trades_History_SecuritiesService,
            IOrders_History_SecuritiesService orders_History_SecuritiesService,
            ITrades_History_CurrenciesService trades_History_CurrenciesService,
             IIncoming_PackagesService incoming_PackagesService)
        {
            _trades_History_SecuritiesService = trades_History_SecuritiesService;
            _orders_History_SecuritiesService = orders_History_SecuritiesService;
            _trades_History_CurrenciesService = trades_History_CurrenciesService;
            _incoming_PackagesService = incoming_PackagesService;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<incoming_packages> ImportAllPackage([FromBody]  incoming_packages incoming_package, Guid user_guid)
        {
            List<EntityOperationResult<trades_history_securities>> trade_History_SecuritiesOperationResults = new List<EntityOperationResult<trades_history_securities>>();
            List<EntityOperationResult<orders_history_securities>> orders_History_SecuritiesOperationResults = new List<EntityOperationResult<orders_history_securities>>();
            List<EntityOperationResult<trades_history_currencies>> trades_History_CurrenciesOperationResults = new List<EntityOperationResult<trades_history_currencies>>();

            try
            {
                var trade_history_security = JsonConvert.DeserializeObject<trades_history_securities>(incoming_package.trd_str);
                var trd_EntityOperationResult = await _trades_History_SecuritiesService.Create(trade_history_security, user_guid);
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
                incoming_package.ordds_transfer_result = JsonConvert.SerializeObject(trades_History_CurrenciesOperationResults);
                await _incoming_PackagesService.Update(incoming_package, user_guid);

                return incoming_package;
            }
            catch (Exception ex)
            {
                throw new Exception(JsonConvert.SerializeObject(ex));
            }
        }
    }
}
