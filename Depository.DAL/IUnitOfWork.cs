using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.Core.Models.DTO;
using Depository.DAL.DbContext.Repositories;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Depository.DAL
{
    public interface IUnitOfWork : IDisposable
    {
        IBanksRepository banks { get; }
        IExchange_RatesRepository exchange_rates { get; }
        ICurrenciesRepository currencies { get; }
        IMetadataRepository metadata { get; }
        IAccountsRepository accounts { get; }
        ICountriesRepository countries { get; }
        IDepositorsRepository depositors { get; }
        IExchangesRepository exchanges { get; }
        IIssuersRepository issuers { get; }
        ILocalitiesRepository localities { get; }
        IPartner_ContactsRepository partner_contacts { get; }
        IPartnersRepository partners { get; }
        IUser_RolesRepository user_roles { get; }
        IUsersRepository users { get; }
        IAccount_TypesRepository account_types { get; }
        IDepositoriesRepository depositories { get; }
        IInstructionsRepository instructions { get; }
        IInstruction_TypesRepository instruction_types { get; }
        ISecuritiesRepository securities { get; }
        ITrading_SystemsRepository trading_systems { get; }
        ICorr_DepositoriesRepository corr_depositories { get; }
        IInstruction_Account_RelationsRepository instruction_account_relations { get; }
        ISecurity_TypesRepository security_types { get; }
        IRegistrarsRepository registrars { get; }
        IDirectoryRepository directory { get; }
        IAccount_MembersRepository account_members { get; }
        IAccount_StatusesRepository account_statuses { get; }
        IAccounting_EntryRepository accounting_entry { get; }
        ITransactionsRepository transactions { get; }
        ICitiesRepository cities { get; }
        IRegionsRepository regions { get; }
        IStock_SecurityRepository stock_security { get; }
        IStock_CurrencyRepository stock_currency { get; }
        IPartner_TypesRepository partner_types { get; }
        IService_GroupsRepository service_groups { get; }
        IService_TypesRepository service_types { get; }
        ITariffs_cdRepository tariffs_cd { get; }
        ITariffs_registrarsRepository tariffs_registrars { get; }
        ICoefficient_DepositorsRepository coefficient_depositors { get; }
        ITariffs_Corr_Depository tariffs_corr_depository { get; }
        ICharge_For_Cd_ServicesRepository charge_for_cd_services { get; }
        IPayments_For_Cd_ServicesRepository payments_for_cd_services { get; }
        ITransit_Charge_For_Cd_ServicesRepository transit_charge_for_cd_services { get; }
        ITransit_Payments_For_Cd_ServicesRepository transit_payments_for_cd_services { get; }
        IPayer_TypesRepository payer_types { get; }
        IRecipient_TypesRepository recipient_types { get; }
        ILegal_StatusesRepository legal_statuses { get; }
        IAccounts_GikRepository accounts_gik { get; }
        IQuestinaries_GikRepository questinaries_gik { get; }
        IReport_TypesRepository report_types { get; }
        IImported_Files_GikRepository imported_files_gik { get; }
        IAccount_ManagersRepository account_managers { get; }
        ITradesRepository trades { get; }
        IOrders_History_SecuritiesRepository orders_history_securities { get; }
        IOrders_History_CurrenciesRepository orders_history_currencies { get; }

        IContractsRepository contracts { get; }
        ITrades_History_SecuritiesRepository trades_history_securities { get; }
        ITrades_History_CurrenciesRepository trades_history_currencies { get; }
        IHistoryRepository histories { get; }
        IOper_DaysRepository oper_days { get; }
        IInstruction_StatusesRepository instruction_statuses { get; }
        IIt_BasesRepository it_bases { get; }
        IInstruction_ReportsRepository instruction_reports { get; }
        IInstruction_Types_User_PermissionsRepository instruction_types_user_permissions { get; }
        ITransfer_ordersRepository transfer_orders { get; }
        IMortgage_SecuritiesRepository mortgage_securities { get; }
        IInstructions_gikRepository instructions_gik { get; }
        ITransfer_order_statusesRepository transfer_order_statuses { get; }
        IMailsRepository mails { get; }

        IMail_statusesRepository mail_statuses { get; }
        IMail_distributionsRepository mail_distributions { get; }
        INew_Security_ApplicationsRepository new_security_applications { get; }
        INew_Security_Application_TypesRepository new_security_application_types { get; }
        ITrades_History_StatusesRepository trades_history_statuses { get; }
        IAccounting_Report_TypesRepository accounting_report_types { get; }
        IExcertsRepository excerts { get; }
        ISecurity_BalanceRepository security_balance { get; }
        IMail_TypesRepository mail_types { get; }
        ISecurity_Issue_Form_TypesRepository security_issue_form_types  { get;}
         IThs_tasksRepository ths_tasks { get; }
        ISectorsRepository sectors { get; }
        ISchedule_TasksRepository schedule_tasks { get; }
        ITask_TypesRepository task_types { get; }
        IIncoming_PackagesRepository incoming_packages { get; }
        IOutgoing_PackagesRepository outgoing_packages { get; }
        ITax_TypesRepository tax_types { get; }
        ILimitsRepository limits { get; }
        ISecurityLocationRepository security_location { get; }
        IGovSecurityPaymentsRepository gov_securities_payments { get; }
        IVatsRepository vats { get; }
        ISalesTaxesRepository sales_taxes { get; }
        IPaymentTypesRepository payment_types { get; }
        IAccountingEntryStopTypesRepository accounting_entry_stop_types { get; }
        IStockSecurityViewRepository stock_security_views { get; }
        IStockCurrencyViewRepository stock_currency_views { get; }
        IInstructionRegistrarReportsRepository instruction_registrar_reports { get; }
        IInstructionGikStatusesRepository instruction_gik_statuses { get; }
        IInstructionTypesGikRepository instruction_types_gik { get; }
        Task<int> CompleteAsync();
        void BeginTransaction();

        void RollbackTransaction();
        void CommitTransaction();

        IRepository<TEntity> GetRepository<TEntity>() where TEntity : Entity;
        Task<DataTable> ExecuteStoredProc(string storedProcName, Dictionary<string, object> procParams);
    }
}
