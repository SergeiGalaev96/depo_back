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
using Depository.Core.Models.DTO;
using AutoMapper;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using static Dapper.SqlMapper;

namespace Depository.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TradesHistorySecuritiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TradesHistorySecuritiesController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IStock_SecurityService _stock_securityService;
        private readonly ITrades_History_SecuritiesService _trades_history_securitiesService;
        private readonly IInstructionsService _instructionsService;
        public TradesHistorySecuritiesController(ILogger<TradesHistorySecuritiesController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, ITrades_History_SecuritiesService trades_history_securitiesService, IStock_SecurityService stock_securityService, IInstructionsService instructionsService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _stock_securityService = stock_securityService;
            _trades_history_securitiesService = trades_history_securitiesService;
            _instructionsService = instructionsService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<trades_history_securities> entityOperationResult, string action)
        {
            if (entityOperationResult.IsSuccess)
                _logger.LogInformation(action + " : " + JsonConvert.SerializeObject(entityOperationResult));
            else _logger.LogError(action + " : " + JsonConvert.SerializeObject(entityOperationResult));
        }

        private async Task WriteToLogException(Exception exception, string action)
        {
            _logger.LogError(action + " : " + exception.ToString());
        }

        private async Task<transactions> CreateTransaction(transactions transaction)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var trans_entity = unitOfWork.transactions.InsertAsyncWithoutHistory(transaction);
                await unitOfWork.CompleteAsync();
                return trans_entity.Result;
            }
        }

        //private async Task<string> ExportToExchange(List<trades_history_securities> trades_history_securities)
        //{
        //    var url = _configuration.GetValue<string>("Links:ExportLink");
        //    var client = new HttpClient();
        //    client.DefaultRequestHeaders
        //    .Accept
        //    .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //    string json = JsonConvert.SerializeObject(exportedInstrumentDTOList);
        //    StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
        //    var response = await client.PostAsync(url, data);
        //    client.Dispose();
        //    string result = await response.Content.ReadAsStringAsync();
        //    return result;
        //}

        /// <summary>
        /// Импортирует/записывает заключенные сделки по итогам торговой сессии(TRD)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Import([FromBody] List<trades_history_securities> trades_history_securities, DateTime trade_date, Guid user_guid)
        {
            List<EntityOperationResult<trades_history_securities>> entityOperationResults = new List<EntityOperationResult<trades_history_securities>>();
            List<EntityOperationResult<trade_history_output>> thoList = new List<EntityOperationResult<trade_history_output>>();
            List<trade_history_output> trade_history_outputs = new List<trade_history_output>();

            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var statusList = unitOfWork.trades_history_statuses.GetList();
                    if (trades_history_securities != null)
                    {
                        foreach (var trade_history_securities in trades_history_securities)
                        {
                            trade_history_securities.trade_date = trade_date;
                            //trade_history_securities.trade_id = trade_history_securities.id;
                            trade_history_securities.id = 0;
                            var entityOperationResult = await _trades_history_securitiesService.Create(trade_history_securities, user_guid);
                            var trade_history_security = entityOperationResult.Entity;
                            if (entityOperationResult.Errors != null)
                            {
                                var error = entityOperationResult.Errors.FirstOrDefault();
                                thoList.Add(EntityOperationResult<trade_history_output>.Failure().AddError(error));
                                return Ok(thoList);
                            }
                            var ths_status = statusList.Where(x => x.id == trade_history_security.status).FirstOrDefault();
                            var trade_history_output = new trade_history_output() { id = trade_history_security.trade_id, status_code = ths_status.code, status_message = ths_status != null ? ths_status.name : "" };
                            thoList.Add(EntityOperationResult<trade_history_output>.Success(trade_history_output));
                        }
                    }
                    entityOperationResults = RecalcAfterImport(trade_date, user_guid);

                    return Ok(thoList);
                }


            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Import_trades_history_securities");
                thoList.Add(EntityOperationResult<trade_history_output>.Failure().AddError(JsonConvert.SerializeObject(ex)));
                return Ok(thoList);
            }
        }




        [HttpPost]
        public async Task<IActionResult> ImportGov([FromBody] List<trades_history_securities> trades_history_securities, DateTime trade_date, Guid user_guid)
        {
            List<EntityOperationResult<trades_history_securities>> entityOperationResults = new List<EntityOperationResult<trades_history_securities>>();
            List<EntityOperationResult<trade_history_output>> trade_history_outputs_entityOperationResults = new List<EntityOperationResult<trade_history_output>>();
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var statusList = unitOfWork.trades_history_statuses.GetList();
                    foreach (var trade_history_securities in trades_history_securities)
                    {

                        trade_history_securities.trade_date = trade_date;
                        //trade_history_securities.trade_id = trade_history_securities.id;
                        trade_history_securities.id = 0;

                        var entityOperationResult = await _trades_history_securitiesService.Create(trade_history_securities, user_guid);
                        var trade_history_security = entityOperationResult.Entity;
                        if (entityOperationResult.Errors != null) return Ok(entityOperationResult.Errors.FirstOrDefault());
                        try
                        {
                            var ths_status = statusList.Where(x => x.id == trade_history_security.status).FirstOrDefault();
                            var trade_history_output = new trade_history_output() { id = trade_history_security.trade_id, status_code = ths_status.code, status_message = ths_status != null ? ths_status.name : "" };
                            trade_history_outputs_entityOperationResults.Add(EntityOperationResult<trade_history_output>.Success(trade_history_output));

                        }
                        catch (Exception ex)
                        {
                            continue;
                        }

                    }
                }
                entityOperationResults = RecalcGovAfterImport(trade_date, user_guid);
                return Ok(trade_history_outputs_entityOperationResults);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Import_trades_history_securities");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }

        [HttpPost]
        public async Task<IActionResult> ImportAsJson([FromBody] List<trades_history_securitiesDTO> trades_history_securityDTOList, DateTime trade_date, Guid user_guid)
        {
            List<EntityOperationResult<trades_history_securities>> entityOperationResults = new List<EntityOperationResult<trades_history_securities>>();

            try
            {
                var config = new MapperConfiguration(cfg => cfg.CreateMap<trades_history_securitiesDTO, trades_history_securities>()
                                                            .ForMember(dest => dest.trade_id, source => source.MapFrom(source => source.id))
                                                            .ForMember(dest => dest.id, source => source.MapFrom(source => 0)));
                // Настройка AutoMapper
                var mapper = new Mapper(config);
                var trades_history_securities = mapper.Map<List<trades_history_securities>>(trades_history_securityDTOList);
                foreach (var trade_history_security in trades_history_securities)
                {

                    trade_history_security.trade_date = trade_date;
                    var entityOperationResult = await _trades_history_securitiesService.Create(trade_history_security, user_guid);


                    entityOperationResults.Add(entityOperationResult);
                }
                return Ok(entityOperationResults);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "ImportAsJson");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }

        private List<EntityOperationResult<trades_history_securities>> RecalcGovAfterImport([FromBody] DateTime trade_date, Guid user_guid)
        {

            List<EntityOperationResult<trades_history_securities>> entityOperationResults = new List<EntityOperationResult<trades_history_securities>>();

            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {

                    var trades_history_securities = unitOfWork.trades_history_securities.GetByDate(trade_date);
                    var stock_Securities = unitOfWork.stock_security.GetList();
                    var accounts = unitOfWork.accounts.GetList();
                    var securities = unitOfWork.securities.GetList().Where(x => x.is_gov_security.Equals(true)).ToList();


                    foreach (var stock_security in stock_Securities)
                    {
                        var security = securities.Where(x => x.id == stock_security.security).FirstOrDefault();
                        var account = unitOfWork.accounts.Get(stock_security.account);
                        //accounts.Where(x => x.id == stock_security.account).FirstOrDefault();
                        if (security != null && account != null)
                        {
                            var trade_history_security = trades_history_securities.Where(x => x.seller_account.Equals(account.accnumber) && x.security_code.Equals(security.code)).FirstOrDefault();
                            if (trade_history_security == null && stock_security.quantity_stop > 0)
                            {
                                var quantity_stop = stock_security.quantity_stop;
                                stock_security.quantity = quantity_stop + stock_security.quantity;
                                stock_security.quantity_stop = 0;
                                var entityOperationResult = _stock_securityService.Update(stock_security, user_guid);
                                if (entityOperationResult.Result.IsSuccess)
                                {

                                    var operationResult = EntityOperationResult<trades_history_securities>
                                    .Success(new trades_history_securities());
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
                WriteToLogException(ex, "RecalcAfterImport_trades_history_securities");
                return entityOperationResults;
            }

        }



        /// <summary>
        /// Пересчитывает остаток после заключенных сделок по итогам торговой сессии(TRD)
        /// </summary>
       // [HttpPost]
        private List<EntityOperationResult<trades_history_securities>> RecalcAfterImport([FromBody] DateTime trade_date, Guid user_guid)
        {

            List<EntityOperationResult<trades_history_securities>> entityOperationResults = new List<EntityOperationResult<trades_history_securities>>();

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var trades_history_securities = unitOfWork.trades_history_securities.GetByDate(trade_date);
                var stock_Securities = unitOfWork.stock_security.GetList();
                var accounts = unitOfWork.accounts.GetList();
                var securities = unitOfWork.securities.GetList().Where(x => !x.is_gov_security.Equals(true)).ToList();



                foreach (var stock_security in stock_Securities)
                {
                    var security = securities.Where(x => x.id == stock_security.security).FirstOrDefault();
                    var account = accounts.Where(x => x.id == stock_security.account).FirstOrDefault();
                    if (security != null && account != null)
                    {
                        var trade_history_security = trades_history_securities.Where(x => x.seller_account.Equals(account.accnumber) && x.security_code.Equals(security.code)).FirstOrDefault();
                        if (trade_history_security == null && stock_security.quantity_stop > 0)
                        {
                            var quantity_stop = stock_security.quantity_stop;
                            stock_security.quantity = quantity_stop + stock_security.quantity;
                            stock_security.quantity_stop = 0;
                            var entityOperationResult = _stock_securityService.Update(stock_security, user_guid);
                            if (entityOperationResult.Result.IsSuccess)
                            {

                                var operationResult = EntityOperationResult<trades_history_securities>
                                .Success(new trades_history_securities());
                                entityOperationResults.Add(operationResult);
                            }
                        }
                    }
                }

                return entityOperationResults;
            }


        }

        private async Task<EntityOperationResult<trades_history_securities>> GetUpdateResult(Task<EntityOperationResult<stock_security>> entityOperationResult, trades_history_securities trade_history_securities, Guid user_guid)
        {
            if (entityOperationResult.Result.IsSuccess)
            {
                trade_history_securities.processed = true;
                var entity = await _trades_history_securitiesService.Update(trade_history_securities, user_guid);
                return entity;
            }
            else return EntityOperationResult<trades_history_securities>
                                .Failure()
                                .AddError(JsonConvert.SerializeObject(entityOperationResult.Result.Errors));
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] trades_history_securities trades_history_security, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<trades_history_securities>> entityOperationResults = new List<EntityOperationResult<trades_history_securities>>();
                var entityOperationResult = await _trades_history_securitiesService.Create(trades_history_security, user_guid);
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
        public async Task<IActionResult> Gets(int pageNumber = 1, int pageSize = 1000)
        {
            try
            {
                pageNumber = pageNumber > 0 ? pageNumber - 1 : pageNumber;
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var trades_history_securities = unitOfWork.trades_history_securities.GetListByPage(pageNumber, pageSize);
                    return Ok(trades_history_securities);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Gettrades_history_securities");
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
                    var trades_history_security = unitOfWork.trades_history_securities.Get(id);
                    return Ok(trades_history_security);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Gettrades_history_security");
                return Ok(ex.ToString());
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] trades_history_securities trades_history_security, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<trades_history_securities>> entityOperationResults = new List<EntityOperationResult<trades_history_securities>>();
                var entityOperationResult = await _trades_history_securitiesService.Update(trades_history_security, user_guid);
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
                List<EntityOperationResult<trades_history_securities>> entityOperationResults = new List<EntityOperationResult<trades_history_securities>>();
                var entityOperationResult = await _trades_history_securitiesService.Delete(id, user_guid);
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
