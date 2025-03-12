using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
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
using Flurl.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Depository.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    public class StockSecurityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StockSecurityController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IStock_SecurityService _stock_securityService;
        private readonly IOutgoing_PackagesService _outgoing_PackagesService;
        public StockSecurityController(ILogger<StockSecurityController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IStock_SecurityService stock_securityService, IOutgoing_PackagesService outgoing_PackagesService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _stock_securityService = stock_securityService;
            _outgoing_PackagesService = outgoing_PackagesService;
        }


        private async Task WriteToLogEntityOperationResult(EntityOperationResult<stock_security> entityOperationResult, string action)
        {
            if (entityOperationResult.IsSuccess)
                _logger.LogInformation(action + " : " + JsonConvert.SerializeObject(entityOperationResult));
            else _logger.LogError(action + " : " + JsonConvert.SerializeObject(entityOperationResult));
        }

        private async Task WriteToLogException(Exception exception, string action)
        {
            _logger.LogError(action + " : " + JsonConvert.SerializeObject(exception.ToString()));
        }


        private async Task<string> SendPostQuerySecurity(object[] objectArray)
        {
            try
            {
                var url = _configuration.GetValue<string>("Links:ExportToExchangeStockSecurityLink");
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

        /// <summary>
        /// Блокирует остаток ЦБ и отправляет в КФБ
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> ExportSecurity(Guid user_guid, int? sector_id, bool online)
        {

            var outgoing_package = new outgoing_packages();
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var user = unitOfWork.users.GetByUserId(user_guid);
                if (user != null)
                {
                    var securityItems = await BlockSecurity(sector_id ?? 1, user_guid);
                    if (online)
                    {
                        outgoing_package = unitOfWork.outgoing_packages.GetByDateAndSector(DateTime.Now.Date, sector_id ?? 1);
                        if (outgoing_package != null)
                        {
                            outgoing_package.pos_str = JsonConvert.SerializeObject(securityItems);
                            outgoing_package.pos_result = await SendPostQuerySecurity(securityItems.ToArray());
                            _outgoing_PackagesService.Update(outgoing_package, user_guid);
                        }
                        else
                        {
                            outgoing_package = new outgoing_packages();
                            outgoing_package.sector_id = sector_id ?? 1;
                            outgoing_package.pos_str = JsonConvert.SerializeObject(securityItems);
                            outgoing_package.pos_result = await SendPostQuerySecurity(securityItems.ToArray());
                            _outgoing_PackagesService.Create(outgoing_package, user_guid);

                        }
                    }

                }

            }
            //return outgoing_package;

            return Ok(EntityOperationResult<outgoing_packages>.Success(outgoing_package));
        }


        private async Task<List<SecurityItem>> BlockSecurity(int sector_id, Guid user_guid)
        {

            List<SecurityItem> exportedInstrumentDTOList = new List<SecurityItem>();
            List<EntityOperationResult<stock_security>> entityOperationResultList = new List<EntityOperationResult<stock_security>>();
            List<accounts> trade_accounts = new List<accounts>();
            List<stock_security> stock_Securities = new List<stock_security>();
            List<securities> securities = new List<securities>();
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    trade_accounts = unitOfWork.accounts.GetTradeAccounts();
                    if ((int)SectorValues.STANDART_SECURITY == sector_id)
                    {
                        securities = unitOfWork.securities.GetStandartSecurities();
                    }
                    else if ((int)SectorValues.GOV_SECURITY_NB_LOCATION == sector_id || (int)SectorValues.GOV_SECURITY_KSE_LOCATION == sector_id)
                    {
                        securities = unitOfWork.securities.GetGovermentSecurities();
                    }
                    stock_Securities = unitOfWork.stock_security.GetBlockedList();
                    foreach (var stock_security in stock_Securities)
                    {
                        var trade_account = trade_accounts.Where(x => x.id == stock_security.account).FirstOrDefault();
                        var security = securities.Where(x => x.id == stock_security.security).FirstOrDefault();
                        if (trade_account != null && security != null)
                        {
                            var quantity = stock_security.quantity;
                            stock_security.quantity_stop += quantity;
                            stock_security.quantity = 0;
                            SecurityItem stockInstrumentDTO = new SecurityItem
                            {
                                account_number = trade_account.accnumber,
                                security_code = security.code,
                                quantity = quantity
                                //quantity = stock_security.quantity_stop,
                                // quantity_stop = stock_security.quantity_stop
                            };
                            exportedInstrumentDTOList.Add(stockInstrumentDTO);


                            var entity = await _stock_securityService.Update(stock_security, user_guid);
                            entityOperationResultList.Add(entity);
                        }

                    }
                }
                return exportedInstrumentDTOList;
            }
            catch (Exception ex)
            {
                throw new Exception("Произошла ошибка во время операции: " + JsonConvert.SerializeObject(ex));
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetDataToExportInTS(int? location_id)
        {
            int depositor_code = 0;
            List<accounts> trade_accounts = new List<accounts>();
            List<stock_security> stock_Securities = new List<stock_security>();
            List<securities> securities = new List<securities>();
            List<model_export_to_exchangeDTO> model_Export_To_ExchangeDTOs = new List<model_export_to_exchangeDTO>();
            List<depositors> depositors = new List<depositors>();
            List<trades> trades = new List<trades>();
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    trade_accounts = unitOfWork.accounts.GetTradeAccounts();
                    securities = unitOfWork.securities.GetList();
                    stock_Securities = unitOfWork.stock_security.GetList().Where(x => x.quantity > 0).ToList();
                    depositors = unitOfWork.depositors.GetList();
                    foreach (var stock_security in stock_Securities)
                    {
                        var trade_account = trade_accounts.Where(x => x.id == stock_security.account).FirstOrDefault();
                        if (trade_account != null)
                        {
                            var depositor = depositors.Where(x => x.partner == trade_account.partner).FirstOrDefault();
                            if (depositor != null)
                            {
                                var trade = unitOfWork.trades.GetList().Where(x => x.depositor == depositor.id).FirstOrDefault();
                                if (trade != null) depositor_code = trade.depositor_code;
                                else throw new Exception($"Депонента №{depositor.id} нету в торговых системах");
                            }
                            var security = securities.Where(x => x.id == stock_security.security).FirstOrDefault();
                            //  if (security.code== "KG0103085815")
                            //   {
                            if (security == null || security.is_gov_security.Equals(true) || !security.send_to_trades.Equals(true) || security.location != location_id) continue;
                            var security_code = security.code;
                            model_Export_To_ExchangeDTOs.Add(
                                new model_export_to_exchangeDTO
                                {
                                    depositor_id_from_trades = depositor_code,
                                    account_number = trade_account.accnumber,
                                    quantity = stock_security.quantity,
                                    security_code = security_code
                                });
                            //  }

                        }
                    }
                }
                return Ok(model_Export_To_ExchangeDTOs);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "StockSecurity_GetDataToExportInTS");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetDataToExportInTSGov(int location_id)
        {
            List<accounts> trade_accounts = new List<accounts>();
            List<stock_security> stock_Securities = new List<stock_security>();
            List<securities> securities = new List<securities>();
            List<model_export_to_exchangeDTO> model_Export_To_ExchangeDTOs = new List<model_export_to_exchangeDTO>();
            List<depositors> depositors = new List<depositors>();
            List<trades> trades = new List<trades>();
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    trade_accounts = unitOfWork.accounts.GetTradeAccounts();
                    securities = unitOfWork.securities.GetList();
                    stock_Securities = unitOfWork.stock_security.GetList().Where(x => x.quantity > 0).ToList();
                    depositors = unitOfWork.depositors.GetList();
                    foreach (var stock_security in stock_Securities)
                    {
                        var trade_account = trade_accounts.Where(x => x.id == stock_security.account).FirstOrDefault();
                        var security = securities.Where(x => x.id == stock_security.security && x.is_gov_security.Equals(true)).FirstOrDefault();
                        if (trade_account != null && security != null && security.location != null)
                        {
                            var depositor = depositors.Where(x => x.partner == trade_account.partner).FirstOrDefault();
                            var trade = unitOfWork.trades.GetList().Where(x => x.depositor == depositor.id).FirstOrDefault();
                            if (trade != null)
                            {
                                var depositor_code = trade.depositor_code;
                                var founded_security = securities.Where(x => x.id == stock_security.security).FirstOrDefault();
                                if (security != null && security.send_to_trades.Equals(true) && security.location == location_id)
                                {
                                    var security_code = founded_security.code;
                                    model_Export_To_ExchangeDTOs.Add(
                                        new model_export_to_exchangeDTO
                                        {
                                            depositor_id_from_trades = depositor_code,
                                            account_number = trade_account.accnumber,
                                            quantity = stock_security.quantity,
                                            security_code = security_code
                                        });
                                }

                            }
                        }
                    }
                }
                return Ok(model_Export_To_ExchangeDTOs);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "StockSecurity_GetDataToExportInTS");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }

        [HttpGet]
        public async Task<IActionResult> ShowInPositions()
        {
            var show_in_positions = true;
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var stock_securities = unitOfWork.stock_security_views.ShowInPositions(show_in_positions);
                return Ok(stock_securities);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportInTSGov(Guid user_guid)
        {
            List<EntityOperationResult<stock_security>> entityOperationResultList = new List<EntityOperationResult<stock_security>>();
            List<accounts> trade_accounts = new List<accounts>();
            List<stock_security> stock_Securities = new List<stock_security>();
            List<securities> securities = new List<securities>();
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    trade_accounts = unitOfWork.accounts.GetTradeAccounts();
                    securities = unitOfWork.securities.GetList();
                    stock_Securities = unitOfWork.stock_security.GetList();
                    foreach (var stock_security in stock_Securities)
                    {
                        var trade_account = trade_accounts.Where(x => x.id == stock_security.account).FirstOrDefault();
                        var security = securities.Where(x => x.id == stock_security.security && x.is_gov_security.Equals(true)).FirstOrDefault();
                        if (trade_account != null && security != null)
                        {
                            var quantity = stock_security.quantity;
                            stock_security.quantity_stop = stock_security.quantity_stop + quantity;
                            stock_security.quantity = 0;
                            var entity = await _stock_securityService.Update(stock_security, user_guid);
                            entityOperationResultList.Add(entity);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                entityOperationResultList.Add(EntityOperationResult<stock_security>
                            .Failure()
                            .AddError(JsonConvert.SerializeObject(ex)));

            }
            return Ok(entityOperationResultList);
        }


        [HttpGet]
        public async Task<EntityOperationResult<stock_security>> Blocking(int stock_security_id, int quantity, Guid user_guid)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var stock_security = unitOfWork.stock_security.Get(stock_security_id);
                if (stock_security != null && stock_security.quantity >= quantity)
                {
                    stock_security.quantity = stock_security.quantity - quantity;
                    stock_security.quantity_stop = stock_security.quantity_stop + quantity;
                    var entity = await _stock_securityService.Update(stock_security, user_guid);
                    return entity;
                }
                else return EntityOperationResult<stock_security>
                              .Failure()
                              .AddError($"Does not exist stock_security_id or quantity is not enough");
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
                    var stock_Security = unitOfWork.stock_security.Get(id);
                    return Ok(stock_Security);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Get_stock_security");
                return Ok(ex.ToString());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Gets()
        {
            List<stock_securityDTO> stock_Securities = new List<stock_securityDTO>();
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var accounts = unitOfWork.accounts.GetList();
                    var securities = unitOfWork.securities.GetList();
                    var stock_securityList = unitOfWork.stock_security.GetList();
                    foreach (var stock_security in stock_securityList)
                    {
                        try
                        {
                            stock_securityDTO stock_securityDTO = new stock_securityDTO();
                            stock_securityDTO.id = stock_security.id;
                            if (stock_security.account != null)
                            {
                                var account = accounts.Where(x => x.id == stock_security.account).FirstOrDefault();
                                if (account != null)
                                {
                                    stock_securityDTO.account = account.accnumber;
                                    stock_securityDTO.partner = account.partner;
                                }
                                else continue;
                            }
                            else stock_securityDTO.account = "";
                            if (stock_security.security != null)
                            {
                                //var security = securities.Where(x => x.id == stock_security.security).FirstOrDefault();
                                stock_securityDTO.security = stock_security.security;
                                //     security !=null ? security.id : null;
                            }
                            stock_securityDTO.account_id = stock_security.account;
                            stock_securityDTO.quantity = stock_security.quantity;
                            stock_securityDTO.quantity_stop = stock_security.quantity_stop;
                            stock_securityDTO.quantity_pledge = stock_security.quantity_pledge;
                            stock_securityDTO.created_at = stock_security.created_at;
                            stock_securityDTO.updated_at = stock_security.updated_at;
                            stock_securityDTO.deleted = stock_security.deleted;


                            stock_Securities.Add(stock_securityDTO);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    return Ok(stock_Securities);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetStockSecurity");
                return Ok(ex.ToString());
            }
        }


        [HttpGet]
        public async Task<IActionResult> Gets2()
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var stock_securities = unitOfWork.stock_security.GetList();
                return Ok(stock_securities);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] stock_security stock_security, Guid user_guid)
        {
            if (stock_security.quantity == null)
            {
                stock_security.quantity = 0;
            }
            if (stock_security.quantity_stop == null)
            {
                stock_security.quantity_stop = 0;
            }
            if (stock_security.quantity_pledge == null)
            {
                stock_security.quantity_pledge = 0;
            }
            try
            {
                List<EntityOperationResult<stock_security>> entityOperationResults = new List<EntityOperationResult<stock_security>>();
                var entityOperationResult = await _stock_securityService.Create(stock_security, user_guid);
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
        public async Task<ActionResult> Update([FromBody] stock_security stock_security, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<stock_security>> entityOperationResults = new List<EntityOperationResult<stock_security>>();
                var entityOperationResult = await _stock_securityService.Update(stock_security, user_guid);


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

        [HttpPost]
        public async Task<ActionResult> Delete([FromBody] int? id, Guid user_guid)
        {
            try
            {
                if (id == null) return BadRequest();
                List<EntityOperationResult<stock_security>> entityOperationResults = new List<EntityOperationResult<stock_security>>();
                var entityOperationResult = await _stock_securityService.DeleteStockSecurity(id, user_guid);
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
