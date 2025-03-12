using Depository.Api.Extentions.Enums;
using Depository.Core;
using Depository.Core.Models;
using Depository.Core.Models.DTO;
using Depository.DAL;
using Depository.Domain.Services;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Api.Utilities
{
    public class ServiceTaskExecutor
    {

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IOper_DaysService _operDaysService;
        private readonly ITrades_History_SecuritiesService _trades_History_SecuritiesService;
        private readonly IOrders_History_SecuritiesService _orders_History_SecuritiesService;
        private readonly ITrades_History_CurrenciesService _trades_History_CurrenciesService;
        private readonly IIncoming_PackagesService _incoming_PackagesService;
        private readonly IStock_SecurityService _stock_securityService;
        private readonly IStock_CurrencyService _stock_CurrencyService;
        private readonly IOutgoing_PackagesService _outgoing_PackagesService;
        private readonly ISchedule_TasksService _schedule_TasksService;
        private readonly IThsTasksService _thsTasksService;
        private readonly IConfiguration _configuration;

        public ServiceTaskExecutor(IOrders_History_SecuritiesService orders_History_SecuritiesService,
            ITrades_History_CurrenciesService trades_History_CurrenciesService,
            IUnitOfWorkFactory unitOfWorkFactory, ISchedule_TasksService schedule_TasksService,
            IOper_DaysService operDaysService, IIncoming_PackagesService incoming_PackagesService,
            IStock_SecurityService stock_securityService, IOutgoing_PackagesService outgoing_PackagesService,
            ITrades_History_SecuritiesService trades_History_SecuritiesService,
            IStock_CurrencyService stock_CurrencyService, IThsTasksService thsTasksService,
            IConfiguration configuration)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _operDaysService = operDaysService;
            _trades_History_SecuritiesService = trades_History_SecuritiesService;
            _orders_History_SecuritiesService = orders_History_SecuritiesService;
            _trades_History_CurrenciesService = trades_History_CurrenciesService;
            _incoming_PackagesService = incoming_PackagesService;
            _stock_securityService = stock_securityService;
            _outgoing_PackagesService = outgoing_PackagesService;
            _stock_CurrencyService = stock_CurrencyService;
            _schedule_TasksService = schedule_TasksService;
            _thsTasksService = thsTasksService;
            _configuration = configuration;
        }


        public async void DoTPlusN(EntityOperationResult<ths_tasks> entityOperationResult, Guid user_guid)
        {
            if (entityOperationResult.IsSuccess)
            {
                var entity = entityOperationResult.Entity;
                var job_id = BackgroundJob.Schedule(() => DoTradeHistorySecurityPlan(entity, user_guid), entity.date);
                entity.job_id = job_id;
                await _thsTasksService.Update(entity, user_guid);
            }
        }

        public async Task<EntityOperationResult<trades_history_securities>> DoTradeHistorySecurityPlan(ths_tasks ths_task, Guid user_guid)
        {

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var ths_Task = unitOfWork.ths_tasks.Get(ths_task.id);
                if (ths_Task != null)
                {
                    if (!ths_Task.is_done)
                    {
                        var trade_history_security = JsonConvert.DeserializeObject<trades_history_securities>(ths_Task.object_str);
                        if (trade_history_security != null)
                        {
                            var entityOperationResult = await _trades_History_SecuritiesService.Create(trade_history_security, user_guid);
                            if (entityOperationResult.IsSuccess)
                            {
                                ths_task.is_done = true;
                                await _thsTasksService.Update(ths_task, user_guid);
                            }
                            return entityOperationResult;
                        }
                        else return EntityOperationResult<trades_history_securities>
                          .Failure()
                          .AddError($"Объект равен нулю");
                    }
                    else return EntityOperationResult<trades_history_securities>
                           .Failure()
                           .AddError($"Задача уже обработана");
                }
                else return EntityOperationResult<trades_history_securities>
                           .Failure()
                           .AddError($"Задача с таким идентификатором не обнаружена");
            }
        }

        public void DoScheduleByType(int operationType, Guid user_guid, int hour, int minute)
        {
            TimeZoneInfo timeZoneInfo;
            try
            {
                timeZoneInfo = TimeZoneInfo.Local;
            }
            catch (TimeZoneNotFoundException)
            {
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Asia/Bishkek");
            }

            switch ((OperationTypes)operationType)
            {
                case OperationTypes.OPEN_OPERATION_DAY:
                    RecurringJob.AddOrUpdate(() => OpenOperDay(DateTime.Now, user_guid), Cron.Daily(hour, minute), timeZoneInfo);
                    break;
                case OperationTypes.CLOSE_OPERATION_DAY:
                    RecurringJob.AddOrUpdate(() => CloseOperDay(DateTime.Now, user_guid), Cron.Daily(hour, minute), timeZoneInfo);
                    break;
                case OperationTypes.IMPORT_STANDART_SECURITY:
                    RecurringJob.AddOrUpdate(() => OpenSecuritySessionBySector((int)SectorValues.STANDART_SECURITY, user_guid), Cron.Daily(hour, minute), timeZoneInfo);
                    break;
                case OperationTypes.EXPORT_STANDART_SECURITY:
                    RecurringJob.AddOrUpdate(() => CloseSecuritySessionBySector((int)SectorValues.STANDART_SECURITY, user_guid), Cron.Daily(hour, minute), timeZoneInfo);
                    break;
                case OperationTypes.IMPORT_GOV_SECURITY_NB_LOCATION:
                    RecurringJob.AddOrUpdate(() => OpenSecuritySessionBySector((int)SectorValues.GOV_SECURITY_NB_LOCATION, user_guid), Cron.Daily(hour, minute), timeZoneInfo);
                    break;
                case OperationTypes.EXPORT_GOV_SECURITY_NB_LOCATION:
                    RecurringJob.AddOrUpdate(() => CloseSecuritySessionBySector((int)SectorValues.GOV_SECURITY_NB_LOCATION, user_guid), Cron.Daily(hour, minute), timeZoneInfo);
                    break;
                case OperationTypes.IMPORT_GOV_SECURITY_KSE_LOCATION:
                    RecurringJob.AddOrUpdate(() => OpenSecuritySessionBySector((int)SectorValues.GOV_SECURITY_KSE_LOCATION, user_guid), Cron.Daily(hour, minute), timeZoneInfo);
                    break;
                case OperationTypes.EXPORT_GOV_SECURITY_KSE_LOCATION:
                    RecurringJob.AddOrUpdate(() => CloseSecuritySessionBySector((int)SectorValues.GOV_SECURITY_KSE_LOCATION, user_guid), Cron.Daily(hour, minute), timeZoneInfo);
                    break;
                case OperationTypes.IMPORT_CURRENCY:
                    OpenCurrencySession(user_guid);
                    break;
                case OperationTypes.EXPORT_CURRENCY:
                    CloseCurrencySession(user_guid);
                    break;
            }
        }

        public async Task<List<EntityOperationResult<oper_days>>> OpenOperDay(DateTime oper_date, Guid user_guid)
        {
            List<EntityOperationResult<oper_days>> entityOperationResults = new List<EntityOperationResult<oper_days>>();
            try
            {
                var oper_time = DateTime.Now;
                oper_days oper_Days = new oper_days { open = oper_time, date = oper_time.Date };

                var entityOperationResult = await _operDaysService.Open(oper_Days, user_guid);
                entityOperationResults.Add(entityOperationResult);
                return entityOperationResults;
            }
            catch (Exception ex)
            {
                var entityOperationResult = EntityOperationResult<oper_days>.Failure().AddError(ex.ToString());
                entityOperationResults.Add(entityOperationResult);
                return entityOperationResults;
            }
        }


        public async Task<List<EntityOperationResult<oper_days>>> CloseOperDay([FromBody] DateTime oper_date, Guid user_guid)
        {
            List<EntityOperationResult<oper_days>> entityOperationResults = new List<EntityOperationResult<oper_days>>();
            oper_days oper_day = new oper_days();
            try
            {
                var oper_time = DateTime.Now;
                oper_days oper_Days = new oper_days { close = oper_time, date = oper_time.Date };

                var entityOperationResult = await _operDaysService.Close(oper_Days, user_guid);
                entityOperationResults.Add(entityOperationResult);
                return entityOperationResults;
            }
            catch (Exception ex)
            {
                var entityOperationResult = EntityOperationResult<oper_days>.Failure().AddError(ex.ToString());
                entityOperationResults.Add(entityOperationResult);
                return entityOperationResults;
            }
        }


        public async Task<List<incoming_packages>> OpenSecuritySessionBySector(int sector_id, Guid user_guid)
        {
            List<incoming_packages> importedPackages = new List<incoming_packages>();
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var incoming_packages = unitOfWork.incoming_packages.GetRawBySector(sector_id);
                    foreach (var incoming_package in incoming_packages)
                    {
                        var imported_package = await ImportSecurityPackage(incoming_package, user_guid);
                        importedPackages.Add(imported_package);
                    }
                }
                return importedPackages;
            }
            catch (Exception ex)
            {
                return importedPackages;
            }
        }


        public async Task<List<incoming_packages>> OpenCurrencySession(Guid user_guid)
        {
            List<incoming_packages> importedPackages = new List<incoming_packages>();
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var incoming_packages = unitOfWork.incoming_packages.GetRaw();
                    foreach (var incoming_package in incoming_packages)
                    {
                        var imported_package = await ImportCurrencyPackage(incoming_package, user_guid);
                        importedPackages.Add(imported_package);
                    }
                }
                return importedPackages;
            }
            catch (Exception ex)
            {
                return importedPackages;
            }
        }




        public async Task<outgoing_packages> CloseSecuritySessionBySector(int sector_id, Guid user_guid)
        {
            var securityItems = await ExportSecurity(sector_id, user_guid);
            var outgoing_package = new outgoing_packages();
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                outgoing_package = unitOfWork.outgoing_packages.GetByDateAndSector(DateTime.Now.Date, sector_id);
                if (outgoing_package != null)
                {
                    outgoing_package.pos_str = JsonConvert.SerializeObject(securityItems);
                    outgoing_package.pos_result = await SendPostQuery(securityItems.ToArray());
                    _outgoing_PackagesService.Update(outgoing_package, user_guid);
                }
                else
                {
                    outgoing_package = new outgoing_packages();
                    outgoing_package.sector_id = sector_id;
                    outgoing_package.pos_str = JsonConvert.SerializeObject(securityItems);
                    outgoing_package.pos_result = await SendPostQuery(securityItems.ToArray());
                    _outgoing_PackagesService.Create(outgoing_package, user_guid);

                }
            }
            return outgoing_package;
        }

        public async Task<string> SendPostQuery(object[] objectArray)
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


        public async void CloseCurrencySession(Guid user_guid)
        {
            var currencyItems = await ExportCurrency(user_guid);
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var outgoing_Package = unitOfWork.outgoing_packages.GetByDateAndSector(DateTime.Now.Date, (int)SectorValues.STANDART_CURRENCY);
                if (outgoing_Package != null)
                {
                    outgoing_Package.pos_str = JsonConvert.SerializeObject(currencyItems);
                    outgoing_Package.pos_result = await SendPostQuery(currencyItems.ToArray());
                    _outgoing_PackagesService.Update(outgoing_Package, user_guid);
                }
                else
                {
                    var outgoing_package = new outgoing_packages();
                    outgoing_package.sector_id = (int)SectorValues.STANDART_CURRENCY;
                    outgoing_Package.pos_str = JsonConvert.SerializeObject(currencyItems);
                    outgoing_Package.pos_result = await SendPostQuery(currencyItems.ToArray());
                    _outgoing_PackagesService.Create(outgoing_Package, user_guid);
                }
            }
        }

        private async Task<incoming_packages> ImportSecurityPackage(incoming_packages incoming_package, Guid user_guid)
        {
            List<EntityOperationResult<trades_history_securities>> trade_History_SecuritiesOperationResults = new List<EntityOperationResult<trades_history_securities>>();
            List<EntityOperationResult<orders_history_securities>> orders_History_SecuritiesOperationResults = new List<EntityOperationResult<orders_history_securities>>();


            try
            {
                var trade_history_security = JsonConvert.DeserializeObject<trades_history_securities>(incoming_package.trd_str);
                var trd_EntityOperationResult = await _trades_History_SecuritiesService.Create(trade_history_security, user_guid);
                if (trd_EntityOperationResult.IsSuccess)
                {
                    trade_History_SecuritiesOperationResults.Add(trd_EntityOperationResult);
                }
                incoming_package.trd_transfer_result = JsonConvert.SerializeObject(trade_History_SecuritiesOperationResults);

                var orders_history_security = JsonConvert.DeserializeObject<orders_history_securities>(incoming_package.ord_str);
                var ord_EntityOperationResult = await _orders_History_SecuritiesService.CreateOrders_History_Security(orders_history_security, user_guid);

                if (ord_EntityOperationResult.IsSuccess)
                {
                    orders_History_SecuritiesOperationResults.Add(ord_EntityOperationResult);
                }
                incoming_package.trd_transfer_result = JsonConvert.SerializeObject(trade_History_SecuritiesOperationResults);

                await _incoming_PackagesService.Update(incoming_package, user_guid);

                return incoming_package;
            }
            catch (Exception ex)
            {
                throw new Exception(JsonConvert.SerializeObject(ex));
            }
        }

        private async Task<incoming_packages> ImportCurrencyPackage(incoming_packages incoming_package, Guid user_guid)
        {
            List<EntityOperationResult<trades_history_currencies>> trades_History_CurrenciesOperationResults = new List<EntityOperationResult<trades_history_currencies>>();

            try
            {
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




        private async Task<List<SecurityItem>> ExportSecurity(int sector_id, Guid user_guid)
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
                    stock_Securities = unitOfWork.stock_security.GetList();
                    foreach (var stock_security in stock_Securities)
                    {
                        var trade_account = trade_accounts.Where(x => x.id == stock_security.account).FirstOrDefault();
                        var security = securities.Where(x => x.id == stock_security.security).FirstOrDefault();
                        if (trade_account != null && security != null)
                        {
                            var quantity = stock_security.quantity;
                            stock_security.quantity_stop = stock_security.quantity_stop + quantity;
                            stock_security.quantity = 0;
                            SecurityItem stockInstrumentDTO = new SecurityItem
                            {
                                account_number = trade_account.accnumber,
                                security_code = security.code,
                                quantity = stock_security.quantity,
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





        private async Task<List<CurrencyItem>> ExportCurrency(Guid user_guid)
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
                    stock_Currencies = unitOfWork.stock_currency.GetList().Where(x => x.quantity > 0).ToList();
                    foreach (var stock_currency in stock_Currencies)
                    {
                        var trade_account = trade_accounts.Where(x => x.id == stock_currency.account).FirstOrDefault();
                        var currency = currencies.Where(x => x.id == stock_currency.currency).FirstOrDefault();
                        if (trade_account != null && currency != null)
                        {
                            var quantity = stock_currency.quantity;
                            stock_currency.quantity_stop = stock_currency.quantity_stop + quantity;
                            stock_currency.quantity = 0;
                            CurrencyItem currencyItem = new CurrencyItem
                            {
                                account_number = trade_account.accnumber,
                                currency_code = currency.code,
                                quantity = stock_currency.quantity,
                                // quantity_stop = stock_currency.quantity_stop
                            };
                            currencyItems.Add(currencyItem);
                            var entity = await _stock_CurrencyService.Update(stock_currency, user_guid);
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
    }
}
