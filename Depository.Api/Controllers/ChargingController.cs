using Depository.Core.Models.DTO;
using Depository.Core.Models;
using Depository.DAL.DbContext;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Depository.DAL;
using Microsoft.Extensions.Configuration;

namespace Depository.Api.Controllers
{
    [Route("api/[controller]/[action]")]

    public class ChargingController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<ChargingController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        const string TRADING_ACCOUNT_SERVICE_TYPE_NAME = "Учет ЦБ на торговых счетах";
        const string NON_TRADING_ACCOUNT_SERVICE_TYPE_NAME = "Учет ЦБ на неторговых счетах";
        public ChargingController(ILogger<ChargingController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
        }

    
        private List<general_depositor_model> GetSecurityBalanceModels(DateTime dt_from, DateTime dt_to, int depositor_id, directories_report2 directories_report)
        {
            List<general_depositor_model> general_depositor_models = new List<general_depositor_model>();
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var depositor = unitOfWork.depositors.Get(depositor_id);
                var accounts = unitOfWork.accounts.GetByPartner(depositor.partner);
                var trading_accounts = accounts.Where(x => x.istrading_account.Value.Equals(true)).ToList();
                var non_trading_accounts= accounts.Where(x => !x.istrading_account.Value.Equals(true)).ToList();
                var security_balances_trading = unitOfWork.security_balance.GetByParams(dt_from, dt_to, trading_accounts);
                var security_balances_non_trading = unitOfWork.security_balance.GetByParams(dt_from, dt_to, non_trading_accounts);
                var trading_account_service_type = unitOfWork.service_types.GetByName(TRADING_ACCOUNT_SERVICE_TYPE_NAME);
                var non_trading_account_service_type = unitOfWork.service_types.GetByName(NON_TRADING_ACCOUNT_SERVICE_TYPE_NAME);
                var general_depositor_models1 = GetDepositorModels_SecurityAccounting(dt_from, dt_to, security_balances_trading, trading_account_service_type.id, trading_account_service_type.service_group, directories_report);
                general_depositor_models.AddRange(general_depositor_models1);
                var general_depositor_models2 = GetDepositorModels_SecurityAccounting(dt_from, dt_to, security_balances_non_trading, non_trading_account_service_type.id, non_trading_account_service_type.service_group, directories_report);
                general_depositor_models.AddRange(general_depositor_models2);
            }
            return general_depositor_models;
        }

        private List<general_depositor_model> GetDepositorModels_SecurityAccounting(DateTime dt_from, DateTime dt_to, List<security_balance> security_balances,   int service_type_id, int service_group_id, directories_report2 directories_report)
        {
            List<general_depositor_model> general_depositor_models = new List<general_depositor_model>();
            var security_balance_groups = security_balances
                            .GroupBy(x => new { x.security, x.account })
                            .Select(g => new
                            {
                                SecurityId = g.Key.security,
                                AccountId = g.Key.account
                            }).ToList();

            foreach(var security_balance_group in security_balance_groups)
            {
                var security_balance_list = security_balances.Where(x => x.account == security_balance_group.AccountId && x.security == security_balance_group.SecurityId );
                foreach (var sec_balance in security_balance_list)
                {
                    general_depositor_model general_depositor_model = new general_depositor_model();
                    var security = directories_report.securities.Where(x => x.id == sec_balance.security).FirstOrDefault();
                    if (security == null) continue;
                    var tariff_central_depository = directories_report.cd_tariffs.Where(x => x.service_type == service_type_id && x.date_start <= sec_balance.date_begin && sec_balance.date_begin <= x.date_end).FirstOrDefault();

                    var tariff = tariff_central_depository != null ? tariff_central_depository.tariff_percent.Value / 100 : 0;
                    if (tariff_central_depository != null)
                    { 
                        tariff_central_depository.date_end = tariff_central_depository.date_end == DateTime.MinValue ? DateTime.Now.Date : tariff_central_depository.date_end;
                    }
                    var days_in_period = tariff_central_depository != null ? (tariff_central_depository.date_end.Value - tariff_central_depository.date_start.Value).Days : 1;
                    var day_tariff = (sec_balance.quantity * security.nominal.Value * tariff) / days_in_period;
                    if (day_tariff == null) continue;
                    general_depositor_model.id = 0;
                    general_depositor_model.currency_id = 0;
                    general_depositor_model.date = sec_balance.date_begin;
                    var issuer = directories_report.issuers.Where(x=>x.id==security.issuer).FirstOrDefault();
                    general_depositor_model.issuer_id = issuer!=null? issuer.id:0;
                    general_depositor_model.quantity = sec_balance.quantity;
                    general_depositor_model.security_id = sec_balance.security;
                    general_depositor_model.service_type_id = service_type_id;
                    general_depositor_model.service_group_id = service_group_id;
                    general_depositor_model.service_main = day_tariff;
                    general_depositor_model.service_transit = 0;
                    general_depositor_model.service_total = day_tariff;
                    general_depositor_model.depositor_id = directories_report.depositor.id;
                    general_depositor_model.security_name = security.code;
                    general_depositor_model.depositor_name = directories_report.partner.name;
                    general_depositor_model.issuer_name = issuer.name;
                    var security_type = directories_report.security_types.Where(x => x.id == security.security_type).FirstOrDefault();
                    general_depositor_model.security_type_name = security_type!=null? security_type.name:"";
                    var service_group = directories_report.service_groups.Where(x => x.id == service_group_id).FirstOrDefault();
                    general_depositor_model.service_group_name = service_group != null ? service_group.name : "";
                    var service_type= directories_report.service_types.Where(x => x.id == service_type_id).FirstOrDefault();
                    general_depositor_model.service_type_name = service_type != null ? service_type.name : "";
                    general_depositor_models.Add(general_depositor_model);
                }
            }
            return general_depositor_models;
        }


        //Отчет по инструментам.
        [HttpGet]
        public List<general_depositor_model> GetReport_2(DateTime dt_from, DateTime dt_to, int depositor_id)
        {
            var directories_report = new directories_report2();
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                directories_report.issuers = unitOfWork.issuers.GetList();
                directories_report.securities = unitOfWork.securities.GetList();
                directories_report.security_types = unitOfWork.security_types.GetList();
                directories_report.service_types = unitOfWork.service_types.GetList();
                directories_report.service_groups = unitOfWork.service_groups.GetList();
                directories_report.cd_tariffs = unitOfWork.tariffs_cd.GetList();
                directories_report.coefficient_depositors = unitOfWork.coefficient_depositors.GetList();
                directories_report.tariff_registrars = unitOfWork.tariffs_registrars.GetList();
                directories_report.partners = unitOfWork.partners.GetList();
                directories_report.depositor = unitOfWork.depositors.Get(depositor_id);
                directories_report.currencies = unitOfWork.currencies.GetList();
                if (directories_report.depositor == null) throw new Exception("Выбранный депонент не найден в справочнике");
                var partner = unitOfWork.partners.Get(directories_report.depositor.partner);
                directories_report.partner = partner;
                if (directories_report.depositor == null) throw new Exception("Выбранный депонент не найден в справочнике контрагентов");

            }
            var general_depositor_models = GetDepositorModels(dt_from, dt_to, depositor_id, directories_report);
            var general_depositor_models_security_accounting = GetSecurityBalanceModels(dt_from, dt_to, depositor_id, directories_report);
            general_depositor_models.AddRange(general_depositor_models_security_accounting);
            return general_depositor_models;

        }

        private List<general_depositor_model> GetDepositorModels(DateTime dt_from, DateTime dt_to, int depositor_id, directories_report2 directories_report)
        {

            report_depositor_model report_depositor_model = new report_depositor_model();
            List<general_depositor_model> general_depositor_models = new List<general_depositor_model>();
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var instructions = unitOfWork.instructions.GetByDepositor(dt_from, dt_to, depositor_id);
                var coefficient_depositor = directories_report.coefficient_depositors.Where(x => x.partner == directories_report.partner.id).FirstOrDefault();
                var coefficient = coefficient_depositor != null ? coefficient_depositor.coefficient : 1;
                foreach (var instruction in instructions)
                {
                   
                    var service_type = directories_report.service_types.Where(x => x.instruction_type == instruction.type && x.is_urgent==instruction.urgent).FirstOrDefault();
                    if (service_type == null) continue;
                    var tariff_cd = directories_report.cd_tariffs.Where(x =>x.service_type==service_type.id && x.date_start <= instruction.executedDate.Value.Date && (x.date_end >= instruction.executedDate.Value.Date || x.date_end==null)).FirstOrDefault();
                    var tariff_registrar = directories_report.tariff_registrars.Where(x => x.date_start <= instruction.executedDate.Value.Date && (x.date_end >= instruction.executedDate.Value.Date || x.date_end==null)).FirstOrDefault();
                    if (tariff_cd != null)
                    {
                        service_types service_type_cd= tariff_cd!=null ? directories_report.service_types.Where(x => x.id == tariff_cd.service_type && x.instruction_type == instruction.type).FirstOrDefault():null;
                        service_types service_type_registrar = tariff_registrar != null ? directories_report.service_types.Where(x => x.id == tariff_registrar.service_type && x.instruction_type == instruction.type).FirstOrDefault() : null;
                        if (service_type != null)
                        {
                            general_depositor_model general_depositor_model = new general_depositor_model();
                            general_depositor_model.service_type_id = service_type.id;
                            general_depositor_model.security_id = instruction.security;
                            general_depositor_model.currency_id = instruction.currency;
                            general_depositor_model.issuer_id = instruction.issuer;
                            general_depositor_model.id = instruction.id;
                            general_depositor_model.quantity = instruction.quantity;
                            general_depositor_model.date = instruction.executedDate;
                            general_depositor_model.service_group_id = service_type.service_group;
                            general_depositor_model.service_main = service_type_cd != null && tariff_cd.tariff_max!=null ? coefficient * tariff_cd.tariff_max.Value : 0;
                            general_depositor_model.service_transit = service_type_registrar != null ? tariff_registrar.tariff.Value : 0;
                            general_depositor_model.service_total = 0;
                            general_depositor_model.currency_name= instruction.currency!=null ? directories_report.currencies.Where(x=>x.id==instruction.currency.Value).FirstOrDefault().name: "";
                            var security = instruction.security!=null ? directories_report.securities.Where(x => x.id == instruction.security.Value).FirstOrDefault():null;
                            if (security!=null)
                            {
                                var security_type = directories_report.security_types.Where(x => x.id == security.security_type).FirstOrDefault();
                                general_depositor_model.security_name = security.code;
                                general_depositor_model.security_type_name = security_type!=null ? security_type.name:"";
                            }
                            general_depositor_model.service_group_name = directories_report.service_groups.Where(x => x.id == service_type.service_group).FirstOrDefault().name;
                            general_depositor_model.service_type_name = service_type.name;
                            general_depositor_model.issuer_name = instruction.issuer != null ? directories_report.issuers.Where(x => x.id == instruction.issuer.Value).FirstOrDefault().name : "";
                            general_depositor_model.depositor_name = directories_report.partner.name;
                            general_depositor_model.depositor_id = directories_report.depositor.id;
                            general_depositor_models.Add(general_depositor_model);
                        }
                    }
                }
            }
            return general_depositor_models;
        }

        [HttpGet]
        public List<report_service_type_model> GetReport_3(DateTime dt_from, DateTime dt_to, int service_type_id)
        {
            var directories_report = new directories_report3();
            List<report_service_type_model> report_service_type_models = new List<report_service_type_model>();
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                directories_report.service_types = unitOfWork.service_types.GetList();
                directories_report.service_groups = unitOfWork.service_groups.GetList();
                var service_type = directories_report.service_types.Where(x => x.id == service_type_id).FirstOrDefault();
                var service_group= directories_report.service_groups.Where(x => x.id == service_type.service_group).FirstOrDefault();
                if (service_type==null) throw new Exception("Выбранный тип услуг не найден в справочнике");
                if (service_group== null) throw new Exception("Выбранный депонент не найден в справочнике контрагентов");
                directories_report.instructions = unitOfWork.instructions.GetByServiceType(dt_from, dt_to, service_type).Where(x => x.depositor != null).ToList();
                directories_report.cd_tariffs = unitOfWork.tariffs_cd.GetList();
                directories_report.coefficient_depositors = unitOfWork.coefficient_depositors.GetList();
                directories_report.tariff_registrars = unitOfWork.tariffs_registrars.GetList();
                directories_report.securities = unitOfWork.securities.GetList();
                directories_report.partners = unitOfWork.partners.GetList();
                directories_report.depositors = unitOfWork.depositors.GetList();
                directories_report.service_type = service_type;
                directories_report.service_group = service_group;
                

            }
            if (directories_report.service_type.name.Equals(NON_TRADING_ACCOUNT_SERVICE_TYPE_NAME) || directories_report.service_type.name.Equals(TRADING_ACCOUNT_SERVICE_TYPE_NAME))
            {
                report_service_type_models = GetSecurityAccountingModel(dt_from, dt_to, directories_report);
            }
            else
            {
                report_service_type_models=GetStandartServiceTypeModels(dt_from, dt_to, directories_report);
            }
            return report_service_type_models;
        }

        private List<report_service_type_model> GetSecurityAccountingModel(DateTime dt_from, DateTime dt_to,  directories_report3 directories_report)
        {
           
            List<accounts> accounts = new List<accounts>();
            List<report_service_type_model> report_service_type_models = new List<report_service_type_model>();
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                        var accountList = unitOfWork.accounts.GetList();
                        if (directories_report.service_type.name.Equals(TRADING_ACCOUNT_SERVICE_TYPE_NAME))
                        accounts = accountList.Where(x => x.istrading_account.Value.Equals(true)).ToList();
                        else if (directories_report.service_type.name.Equals(NON_TRADING_ACCOUNT_SERVICE_TYPE_NAME))
                         accounts = accounts.Where(x => !x.istrading_account.Value.Equals(true)).ToList();
                        var non_trading_accounts = accounts.Where(x => !x.istrading_account.Value.Equals(true)).ToList();
                        var security_balances= unitOfWork.security_balance.GetByParams(dt_from, dt_to, accounts);
                var general_depositor_models1 = GetServiceTypeModels_SecurityAccounting(dt_from, dt_to, security_balances, directories_report, accounts);
                report_service_type_models.AddRange(general_depositor_models1);
            }
            return report_service_type_models;
        }

        private List<report_service_type_model> GetServiceTypeModels_SecurityAccounting(DateTime dt_from, DateTime dt_to, List<security_balance> security_balances,  directories_report3 directories_report, List<accounts> accounts)
        {
            List<report_service_type_model> report_service_type_models = new List<report_service_type_model>();
            var security_balance_groups = security_balances
                            .GroupBy(x => new { x.security, x.account })
                            .Select(g => new
                            {
                                SecurityId = g.Key.security,
                                AccountId = g.Key.account
                            }).ToList();
            foreach (var security_balance_group in security_balance_groups)
            {
                var security_balance_list = security_balances.Where(x => x.account == security_balance_group.AccountId && x.security == security_balance_group.SecurityId);
                foreach (var sec_balance in security_balance_list)
                {
                    var security = directories_report.securities.Where(x => x.id == sec_balance.security).FirstOrDefault();
                    if (security == null) continue;
                    var account = accounts.Where(x => x.id == sec_balance.account).FirstOrDefault();
                    if (account == null) continue;
                    
                    var partner = directories_report.partners.Where(x => x.id == account.partner).FirstOrDefault();
                    if (partner == null) continue;
                    var depositor = directories_report.depositors.Where(x => x.partner == partner.id).FirstOrDefault();
                    if (depositor == null) continue;

                    var tariff_central_depository = directories_report.cd_tariffs.Where(x => x.service_type == directories_report.service_type.id && x.date_start <= sec_balance.date_begin.Date && sec_balance.date_begin.Date <= x.date_end).FirstOrDefault();
                    var tariff = tariff_central_depository != null ? tariff_central_depository.tariff_percent.Value / 100 : 0;
                    tariff_central_depository.date_end = tariff_central_depository.date_end == null ? DateTime.Now.Date : tariff_central_depository.date_end;
                    var days_in_period = tariff_central_depository != null ? (tariff_central_depository.date_end.Value - tariff_central_depository.date_start.Value).Days : 1;
                    var day_tariff = (sec_balance.quantity * security.nominal.Value * tariff) / days_in_period;
                    if (day_tariff == null) continue;
                    report_service_type_model report_service_type_model = new report_service_type_model();
                    report_service_type_model.depositor_id = depositor.id;
                    report_service_type_model.depositor_name = partner.name;
                    report_service_type_model.service_group_id = directories_report.service_group.id;
                    report_service_type_model.service_type_id = directories_report.service_type.id;
                    report_service_type_model.service_group_name = directories_report.service_group.name;
                    report_service_type_model.service_type_name = directories_report.service_type.name;
                    report_service_type_model.service_main= day_tariff;
                    report_service_type_model.service_transit = 0;
                    report_service_type_model.service_total = day_tariff;
                    report_service_type_models.Add(report_service_type_model);
                }
            }
            return report_service_type_models;
        }


        private List<report_service_type_model> GetStandartServiceTypeModels(DateTime dt_from, DateTime dt_to,directories_report3 directories_report)
        {
            List<report_service_type_model> report_service_type_models = new List<report_service_type_model>();
                      
            List<int> depositors = directories_report.instructions.GroupBy(x => x.depositor).Select(g => g.Key.Value).ToList();
            foreach (var depositor_id in depositors)
            {
                var instructions = directories_report.instructions.Where(x => x.depositor == depositor_id);
                foreach (var instruction in instructions)
                {
                    report_service_type_model report_service_type_model = new report_service_type_model();
                    report_service_type_model.id = instruction.id;
                    var tariff_cd = directories_report.cd_tariffs.Where(x => x.service_type == directories_report.service_type.id && x.date_start <= instruction.executedDate.Value.Date && (x.date_end >= instruction.executedDate.Value.Date || x.date_end==null)).FirstOrDefault();
                    var tariff_registrar = directories_report.tariff_registrars.Where(x => x.date_start <= instruction.executedDate && x.date_end >= instruction.executedDate).FirstOrDefault();
                    var depositor = directories_report.depositors.Where(x => x.id == depositor_id).FirstOrDefault();
                    if (depositor != null)
                    {
                        var coefficient_depositor = directories_report.coefficient_depositors.Where(x => x.partner == depositor.partner).FirstOrDefault();
                        var coefficient = coefficient_depositor != null ? coefficient_depositor.coefficient : 1;
                        service_types service_type_cd = tariff_cd != null ? directories_report.service_types.Where(x => x.id == tariff_cd.service_type && x.instruction_type == instruction.type).FirstOrDefault() : null;
                        service_types service_type_registrar = tariff_registrar != null ? directories_report.service_types.Where(x => x.id == tariff_registrar.service_type && x.instruction_type == instruction.type).FirstOrDefault() : null;
                        report_service_type_model.service_transit = service_type_registrar != null ? tariff_registrar.tariff.Value : 0;
                        report_service_type_model.service_main= service_type_cd != null && tariff_cd.tariff_max != null ? coefficient * tariff_cd.tariff_max.Value : 0;
                        report_service_type_model.service_group_name = directories_report.service_group.name;
                        report_service_type_model.service_type_name = directories_report.service_type.name;
                        report_service_type_model.service_group_id = directories_report.service_group.id;
                        report_service_type_model.service_type_id = directories_report.service_type.id;
                        var partner = directories_report.partners.Where(x => x.id == depositor.partner).FirstOrDefault();
                        report_service_type_model.depositor_name = partner != null ? partner.name : "";
                        report_service_type_model.depositor_id = depositor.id;
                        report_service_type_model.service_total = report_service_type_model.service_main + report_service_type_model.service_transit;
                        report_service_type_models.Add(report_service_type_model);
                    }
                   
                }
            }
            return report_service_type_models;
        }


        [HttpGet]
        public List<report_security_model> GetReport_4(DateTime dt_from, DateTime dt_to)
        {
            List<report_security_model> report_security_models = new List<report_security_model>();
            var directories_report = new directories_report4();
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                directories_report.depositors = unitOfWork.depositors.GetList();
                directories_report.instructions = unitOfWork.instructions.GetByPeriod(dt_from, dt_to);
                directories_report.partners = unitOfWork.partners.GetList();
                directories_report.securities = unitOfWork.securities.GetList();
                directories_report.security_types = unitOfWork.security_types.GetList();
                directories_report.issuers = unitOfWork.issuers.GetList();
                directories_report.service_types = unitOfWork.service_types.GetList();
                directories_report.service_groups = unitOfWork.service_groups.GetList();
                directories_report.tariffs_cds = unitOfWork.tariffs_cd.GetList();
                directories_report.tariffs_registrars = unitOfWork.tariffs_registrars.GetList();
                directories_report.coefficient_depositors = unitOfWork.coefficient_depositors.GetList();
                directories_report.security_balances = unitOfWork.security_balance.GetByPeriod(dt_from, dt_to);
            }
            var report_service_type_models = GetStandartSecurityModels(dt_from, dt_to, directories_report);
            var report_service_type_models2 = GetSecurityModel(dt_from, dt_to, directories_report);
            report_service_type_models.AddRange(report_service_type_models2);
            return report_service_type_models;
        }

        private List<report_security_model> GetSecurityModel(DateTime dt_from, DateTime dt_to, directories_report4 directories_report)
        {
            List<report_security_model> report_security_models = new List<report_security_model>();
            List<accounts> accounts;
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var accountList = unitOfWork.accounts.GetList();
                var trading_accounts = accountList.Where(x => x.istrading_account.Value.Equals(true)).ToList();
                var service_type1 = directories_report.service_types.Where(x => x.name.Equals(TRADING_ACCOUNT_SERVICE_TYPE_NAME)).FirstOrDefault();

                var general_depositor_models1 = GetSecurityModel_SecurityAccounting(dt_from, dt_to, directories_report, trading_accounts, service_type1);
                report_security_models.AddRange(general_depositor_models1);

                var non_trading_accounts = accountList.Where(x => !x.istrading_account.Value.Equals(true)).ToList();
                var service_type2 = directories_report.service_types.Where(x => x.name.Equals(NON_TRADING_ACCOUNT_SERVICE_TYPE_NAME)).FirstOrDefault();

                var general_depositor_models2 = GetSecurityModel_SecurityAccounting(dt_from, dt_to, directories_report, non_trading_accounts, service_type2);
                report_security_models.AddRange(general_depositor_models2);
            }
            return report_security_models;
        }

        private List<report_security_model> GetSecurityModel_SecurityAccounting(DateTime dt_from, DateTime dt_to, directories_report4 directories_report, List<accounts> accounts, service_types service_type)
        {
            List<report_security_model> report_security_models = new List<report_security_model>();
            var security_balance_groups = directories_report.security_balances
                          .GroupBy(x => new { x.security, x.account })
                          .Select(g => new
                          {
                              SecurityId = g.Key.security,
                              AccountId = g.Key.account
                          }).ToList();
            foreach (var security_balance_group in security_balance_groups)
            {
              
                var security_balance_list = directories_report.security_balances.Where(x => x.account == security_balance_group.AccountId && x.security == security_balance_group.SecurityId);
                foreach (var sec_balance in security_balance_list)
                {
                    var security = directories_report.securities.Where(x => x.id == sec_balance.security).FirstOrDefault();
                    if (security == null) continue;
                    var account = accounts.Where(x => x.id == sec_balance.account).FirstOrDefault();
                    if (account == null) continue;

                    var partner = directories_report.partners.Where(x => x.id == account.partner).FirstOrDefault();
                    if (partner == null) continue;
                    var depositor = directories_report.depositors.Where(x => x.partner == partner.id).FirstOrDefault();
                    if (depositor == null) continue;
                    var tariff_central_depository = directories_report.tariffs_cds.Where(x => x.service_type == service_type.id && x.date_start <= sec_balance.date_begin.Date && sec_balance.date_begin.Date <= x.date_end).FirstOrDefault();
                    if (tariff_central_depository == null) continue;
                    var tariff = tariff_central_depository != null ? tariff_central_depository.tariff_percent.Value / 100 : 0;
                    tariff_central_depository.date_end =  tariff_central_depository.date_end.Equals(DateTime.MinValue) ? DateTime.Now.Date : tariff_central_depository.date_end;
                    var days_in_period = tariff_central_depository != null ? (tariff_central_depository.date_end.Value - tariff_central_depository.date_start.Value).Days : 1;
                    var day_tariff = (sec_balance.quantity * security.nominal.Value * tariff) / days_in_period;
                    if (day_tariff == null) continue;
                    var service_group = directories_report.service_groups.Where(x => x.id == service_type.service_group).FirstOrDefault();
                    var issuer = directories_report.issuers.Where(x => x.id == security.issuer).FirstOrDefault();
                    if (issuer == null) continue;
                    var security_type = directories_report.security_types.Where(x => x.id == security.security_type).FirstOrDefault();
                    if (security_type == null) continue;

                    report_security_model report_security_model = new report_security_model();
                    report_security_model.depositor_id= depositor.id;
                    report_security_model.depositor_name = partner.name;
                    report_security_model.service_group_id = service_group.id;
                    report_security_model.service_group_name = service_group.name;
                    report_security_model.service_type_id = service_type.id;
                    report_security_model.service_type_name = service_type.name;
                    report_security_model.service_main = day_tariff;
                    report_security_model.service_transit = 0;
                    report_security_model.service_total = day_tariff;
                    report_security_model.security_name = security.code + ", " + security_type.name + ", " + issuer.name;
                    report_security_models.Add(report_security_model);
                }
            }
            return report_security_models; 
        }
        private List<report_security_model> GetStandartSecurityModels(DateTime dt_from, DateTime dt_to, directories_report4 directories_report)
        {
            List<report_security_model> report_security_models = new List<report_security_model>();
            List<int> depositors = directories_report.instructions.Where(x=>x.depositor!=null || x.depositor2!=null).GroupBy(x => x.depositor!=null ? x.depositor: x.depositor2).Select(g => g.Key.Value).ToList();
            foreach (var depositor_id in depositors)
            {
                report_security_model report_security_model = new report_security_model();
                var depositor = directories_report.depositors.Where(x => x.id == depositor_id).FirstOrDefault();
                if (depositor == null) throw new Exception("Депонент: " + depositor_id.ToString() + " не найден в справочнике");
                var partner = directories_report.partners.Where(x => x.id == depositor.partner).FirstOrDefault();
                if (partner == null) throw new Exception("Контрагент: " + depositor.partner.ToString() + " не найден в справочнике");
                report_security_model.depositor_id = depositor.id;
                report_security_model.depositor_name = partner.name;
                var instructions = directories_report.instructions.Where(x => x.depositor == depositor_id);
                var coefficient_depositor = directories_report.coefficient_depositors.Where(x => x.partner == partner.id).FirstOrDefault();
                var coefficient = coefficient_depositor != null ? coefficient_depositor.coefficient : 1;
                foreach (var instruction in instructions)
                {
                    var security = directories_report.securities.Where(x => x.id == instruction.security).FirstOrDefault();
                    if (security == null) continue;
                    var security_type = directories_report.security_types.Where(x => x.id == security.security_type).FirstOrDefault();
                    if (security_type == null) continue;
                    var issuer = directories_report.issuers.Where(x => x.id == instruction.issuer).FirstOrDefault();
                    if (issuer == null) continue;
                    report_security_model.security_name = security.code + ", " + security_type.name + ", " + issuer.name;
                    var service_type = directories_report.service_types.Where(x => x.instruction_type == instruction.type).FirstOrDefault();
                    if (service_type == null) continue;
                    var service_group = directories_report.service_groups.Where(x => x.id == service_type.service_group).FirstOrDefault();
                    if (service_group == null) continue;
                    var tariff_cd = directories_report.tariffs_cds.Where(x => x.service_type == service_type.id && x.date_start <= instruction.executedDate.Value.Date && (x.date_end >= instruction.executedDate.Value.Date || x.date_end == null)).FirstOrDefault();
                    var tariff_registrar = directories_report.tariffs_registrars.Where(x => x.date_start <= instruction.executedDate.Value.Date && (x.date_end >= instruction.executedDate.Value.Date || x.date_end == null)).FirstOrDefault();
                    if (tariff_cd != null)
                    {
                        service_types service_type_cd = tariff_cd != null ? directories_report.service_types.Where(x => x.id == tariff_cd.service_type && x.instruction_type == instruction.type).FirstOrDefault() : null;
                        service_types service_type_registrar = tariff_registrar != null ? directories_report.service_types.Where(x => x.id == tariff_registrar.service_type && x.instruction_type == instruction.type).FirstOrDefault() : null;
                        if (service_type != null)
                        {
                            report_security_model.service_main = service_type_cd != null && tariff_cd.tariff_max != null ? coefficient * tariff_cd.tariff_max.Value : 0;
                            report_security_model.service_transit = service_type_registrar != null ? tariff_registrar.tariff.Value : 0;
                            report_security_model.service_total = 0;
                            report_security_model.service_type_id = service_type.id;
                            report_security_model.service_type_name = service_type.name;
                            report_security_model.service_group_id = service_type.service_group;
                            report_security_model.service_group_name = service_group.name;
                            report_security_models.Add(report_security_model);
                        }
                    }
                }
                
            }
            return report_security_models;
        }
    }

}
