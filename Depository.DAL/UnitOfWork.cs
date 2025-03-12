using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.Core.Models.DTO;
using Depository.DAL.DbContext;
using Depository.DAL.DbContext.Repositories;
using Depository.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using Npgsql.Schema;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Depository.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly ConcurrentDictionary<Type, object> _repositories;

        private IDbContextTransaction _transaction;

        private bool _disposed;

        public IBanksRepository banks { get; }
        public IExchange_RatesRepository exchange_rates { get; }
        public ICurrenciesRepository currencies { get; }
        public IMetadataRepository metadata { get; }
        public IAccountsRepository accounts { get; }
        public ICountriesRepository countries { get; }
        public IDepositorsRepository depositors { get; }
        public IExchangesRepository exchanges { get; }
        public IIssuersRepository issuers { get; }
        public ILocalitiesRepository localities { get; }
        public IPartner_ContactsRepository partner_contacts { get; }
        public IPartnersRepository partners { get; }
        public IUser_RolesRepository user_roles { get; }
        public IUsersRepository users { get; }
        public IAccount_TypesRepository account_types { get; }
        public IDepositoriesRepository depositories { get; }
        public IInstructionsRepository instructions { get; }
        public IInstruction_TypesRepository instruction_types { get; }
        public ISecuritiesRepository securities { get; }
        public ITrading_SystemsRepository trading_systems { get; }
        public ICorr_DepositoriesRepository corr_depositories { get; }
        public IInstruction_Account_RelationsRepository instruction_account_relations { get; }
        public ISecurity_TypesRepository security_types { get; }
        public IRegistrarsRepository registrars { get; }
        public IDirectoryRepository directory { get; }
        public IAccount_MembersRepository account_members { get; }
        public IAccount_StatusesRepository account_statuses { get; }
        public IAccounting_EntryRepository accounting_entry { get; }
        public ITransactionsRepository transactions { get; }
        public ICitiesRepository cities { get; }
        public IRegionsRepository regions { get; }
        public IStock_SecurityRepository  stock_security {get;}
        public IStock_CurrencyRepository stock_currency { get; }
        public  IPartner_TypesRepository partner_types { get; }
        public IService_GroupsRepository service_groups { get; }
        public IService_TypesRepository service_types { get; }
        public ITariffs_cdRepository tariffs_cd { get; }
        public ITariffs_registrarsRepository tariffs_registrars { get; }
        public ICoefficient_DepositorsRepository coefficient_depositors { get; }
        public ITariffs_Corr_Depository tariffs_corr_depository { get; }
        public ICharge_For_Cd_ServicesRepository charge_for_cd_services { get; }
        public IPayments_For_Cd_ServicesRepository payments_for_cd_services { get; }
        public ITransit_Charge_For_Cd_ServicesRepository transit_charge_for_cd_services { get; }
        public ITransit_Payments_For_Cd_ServicesRepository transit_payments_for_cd_services { get; }
        public IPayer_TypesRepository payer_types { get; }
        public IRecipient_TypesRepository recipient_types { get; }
        public ILegal_StatusesRepository legal_statuses { get; }
        public IAccounts_GikRepository accounts_gik { get; }
        public IQuestinaries_GikRepository questinaries_gik { get; }
        public IReport_TypesRepository report_types { get; }
        public IImported_Files_GikRepository imported_files_gik { get; }
        public IAccount_ManagersRepository account_managers { get; }
        public ITradesRepository trades { get; }
        public IContractsRepository contracts { get; }
        public IOrders_History_SecuritiesRepository orders_history_securities { get; }
        public IOrders_History_CurrenciesRepository orders_history_currencies { get; }
        public ITrades_History_CurrenciesRepository trades_history_currencies { get; }
        public ITrades_History_SecuritiesRepository trades_history_securities { get; }
        public IHistoryRepository histories { get; }
        public IOper_DaysRepository oper_days { get; }
        public IInstruction_StatusesRepository instruction_statuses { get; }
        public IIt_BasesRepository it_bases { get; }
        public IInstruction_ReportsRepository instruction_reports { get; }
        public IInstruction_Types_User_PermissionsRepository instruction_types_user_permissions { get; }
        public ITransfer_ordersRepository transfer_orders { get; }
        public IMortgage_SecuritiesRepository mortgage_securities { get; }
        public IInstructions_gikRepository instructions_gik { get; }
        public ITransfer_order_statusesRepository transfer_order_statuses { get; }
        public IMailsRepository mails { get; }
        public IMail_statusesRepository mail_statuses { get; }
        public IMail_distributionsRepository mail_distributions { get; }
        public INew_Security_ApplicationsRepository new_security_applications { get; }
        public INew_Security_Application_TypesRepository new_security_application_types { get; }
        public ITrades_History_StatusesRepository trades_history_statuses { get; }
        public IAccounting_Report_TypesRepository accounting_report_types { get; }
        public IExcertsRepository excerts { get; }
        public ISecurity_BalanceRepository security_balance { get; }
        public IMail_TypesRepository mail_types { get; }
        public ISecurity_Issue_Form_TypesRepository security_issue_form_types { get;  }
        public IThs_tasksRepository ths_tasks { get; }
        public ISectorsRepository sectors { get; }
        public ISchedule_TasksRepository schedule_tasks { get; }
        public ITask_TypesRepository task_types { get; }
        public IIncoming_PackagesRepository incoming_packages { get; }
        public IOutgoing_PackagesRepository outgoing_packages { get; }
        public ITax_TypesRepository tax_types { get; }
        public ILimitsRepository limits { get; }
        public ISecurityLocationRepository security_location { get; }
        public IGovSecurityPaymentsRepository gov_securities_payments { get; }
        public IVatsRepository vats { get; }
        public ISalesTaxesRepository sales_taxes { get; }
        public IPaymentTypesRepository payment_types { get; }
        public IAccountingEntryStopTypesRepository accounting_entry_stop_types { get; }
        public IStockSecurityViewRepository stock_security_views { get;  }
        public IStockCurrencyViewRepository stock_currency_views { get; }
        public IInstructionRegistrarReportsRepository instruction_registrar_reports { get; }
        public IInstructionGikStatusesRepository instruction_gik_statuses { get; }
        public IInstructionTypesGikRepository instruction_types_gik { get; }
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            _repositories = new ConcurrentDictionary<Type, object>();
            banks = new BanksRepository(context);
            exchange_rates = new Exchange_RatesRepository(context);
            currencies = new CurrenciesRepository(context);
            metadata = new MetadataRepository(context);
            accounts = new AccountsRepository(context);
            countries = new CountriesRepository(context);
            depositors = new DepositorsRepository(context);
            exchanges = new ExchangesRepository(context);
            issuers = new IssuersRepository(context);
            localities = new LocalitiesRepository(context);
            partners = new PartnersRepository(context);
            partner_contacts = new Partner_ContactsRepository(context);
            user_roles = new User_RolesRepository(context);
            users = new UsersRepository(context);
            account_types = new Account_TypesRepository(context);
            depositories = new DepositoriesRepository(context);
            instructions = new InstructionsRepository(context);
            instruction_types = new Instruction_TypesRepository(context);
            securities = new SecuritiesRepository(context);
            trading_systems = new Trading_SystemsRepository(context);
            corr_depositories = new Corr_DepositoriesRepository(context);
            instruction_account_relations = new Instruction_Account_RelationsRepository(context);
            security_types = new Security_TypesRepository(context);
            registrars = new RegistrarsRepository(context);
            directory = new DirectoryRepository(context);
            account_members = new Account_MembersRepository(context);
            account_statuses = new Account_StatusesRepository(context);
            accounting_entry = new Accounting_EntryRepository(context);
            transactions = new TransactionsRepository(context);
            cities = new CitiesRepository(context);
            regions = new RegionsRepository(context);
            stock_security = new Stock_SecurityRepository(context);
            stock_currency = new Stock_CurrencyRepository(context);
            partner_types = new Partner_TypesRepository(context);
            service_groups = new Service_GroupsRepository(context);
            service_types = new Service_TypesRepository(context);
            tariffs_cd = new Tariffs_cdRepository(context);
            tariffs_registrars = new Tariffs_registrarsRepository(context);
            coefficient_depositors = new Coefficient_DepositorsRepository(context);
            tariffs_corr_depository = new Tariffs_Corr_Depository(context);
            charge_for_cd_services = new Charge_For_Cd_ServicesRepository(context);
            payments_for_cd_services = new Payments_For_Cd_ServicesRepository(context);
            transit_charge_for_cd_services = new Transit_Charge_For_Cd_ServicesRepository(context);
            transit_payments_for_cd_services = new Transit_Payments_For_Cd_ServicesRepository(context);
            payer_types = new Payer_TypesRepository(context);
            recipient_types = new Recipient_TypesRepository(context);
            legal_statuses = new Legal_StatusesRepository(context);
            accounts_gik = new Accounts_GikRepository(context);
            questinaries_gik = new Questinaries_GikRepository(context);
            report_types = new Report_TypesRepository(context);
            imported_files_gik = new Imported_Files_GikRepository(context);
            account_managers = new Account_ManagersRepository(context);
            trades = new TradesRepository(context);
            contracts = new ContractsRepository(context);
            orders_history_securities = new Orders_History_SecuritiesRepository(context);
            orders_history_currencies = new Orders_History_CurrenciesRepository(context);
            trades_history_currencies = new Trades_History_CurrenciesRepository(context);
            trades_history_securities = new Trades_History_SecuritiesRepository(context);
            histories = new HistoryRepository(context);
            oper_days = new Oper_DaysRepository(context);
            instruction_statuses = new Instruction_StatusesRepository(context);
            it_bases = new It_BasesRepository(context);
            instruction_reports = new Instruction_ReportsRepository(context);
            instruction_types_user_permissions = new Instruction_Types_User_PermissionsRepository(context);
            transfer_orders = new Transfer_ordersRepository(context);
            mortgage_securities = new Mortgage_SecuritiesRepository(context);
            instructions_gik = new Instructions_gikRepository(context);
            transfer_order_statuses = new Transfer_order_statusesRepository(context);
            mails = new MailsRepository(context);
            mail_statuses = new Mail_statusesRepository(context);
            mail_distributions = new Mail_distributionsRepository(context);
            new_security_applications = new New_Security_ApplicationsRepository(context);
            new_security_application_types = new New_Security_Application_TypesRepository(context);
            trades_history_statuses = new Trades_History_StatusesRepository(context);
            accounting_report_types = new Accounting_Report_TypesRepository(context);
            excerts = new ExcertsRepository(context);
            security_balance = new Security_BalanceRepository(context);
            mail_types = new Mail_TypesRepository(context);
            security_issue_form_types = new Security_Issue_Form_TypesRepository(context);
            ths_tasks = new Ths_tasksRepository(context);
            sectors = new SectorsRepository(context);
            task_types = new Task_TypesRepository(context);
            schedule_tasks = new Schedule_TasksRepository(context);
            incoming_packages = new Incoming_PackagesRepository(context);
            outgoing_packages = new Outgoing_PackagesRepository(context);
            tax_types = new Tax_TypesRepository(context);
            limits = new LimitsRepository(context);
            security_location = new SecurityLocationRepository(context);
            gov_securities_payments = new GovSecurityPaymentsRepository(context);
            vats = new VatsRepository(context);
            sales_taxes = new SalesTaxesRepository(context);
            payment_types = new PaymentTypesRepository(context);
            accounting_entry_stop_types=new AccountingEntryStopTypesRepository(context);
            stock_security_views=new StockSecurityViewRepository(context);
            stock_currency_views=new StockCurrencyViewRepository(context);
            instruction_registrar_reports=new InstructionRegistrarReportsRepository(context);
            instruction_gik_statuses = new InstructionGikStatusesRepository(context);
            instruction_types_gik = new InstructionTypesGikRepository(context);
    }

        public Task<int> CompleteAsync()
        {
            return _context.SaveChangesAsync();
        }

        //public void Refresh(object entity)
        //{
        //    _context.Entry(entity).Reload();
        //}

        public void BeginTransaction()
        {
            _transaction = _context.Database.BeginTransaction();
        }


        public void CommitTransaction()
        {
            if (_transaction == null) return;

            _transaction.Commit();
            _transaction.Dispose();

            _transaction = null;
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : Entity
        {
            return _repositories.GetOrAdd(typeof(TEntity), (object)new Repository<TEntity>(_context)) as IRepository<TEntity>;
        }

        public void RollbackTransaction()
        {
            if (_transaction == null) return;

            _transaction.Rollback();
            _transaction.Dispose();

            _transaction = null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                _context.Dispose();

            _disposed = true;
        }


        ~UnitOfWork()
        {
            Dispose(false);
        }


        private static List<T> DataReaderMapToList<T>(IDataReader dr)
        {
            List<T> list = new List<T>();
            T obj = default(T);
            while (dr.Read())
            {
                obj = Activator.CreateInstance<T>();
                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                {
                    if (!object.Equals(dr[prop.Name], DBNull.Value))
                    {
                        prop.SetValue(obj, dr[prop.Name], null);
                    }
                }
                list.Add(obj);
            }
            return list;
        }

        public async Task<DataTable> ExecuteStoredProc(string storedProcName, Dictionary<string, object> procParams)
        {
            //NpgsqlConnection conn = (NpgsqlConnection) _context.Database.GetDbConnection();
            try
            {
                var connectionString = _context.Database.GetDbConnection().ConnectionString;

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand(storedProcName, connection))
                    {
                        foreach (KeyValuePair<string, object> procParam in procParams)
                        {
                            DbParameter param = command.CreateParameter();
                            param.ParameterName = procParam.Key;
                            param.Value = procParam.Value;
                            command.Parameters.Add(param);
                        }
                        var dt = new DataTable();


                        using (var reader = command.ExecuteReader())
                        {

                            dt.Load(reader);

                            reader.Dispose();
                        }
                        return dt;
                    }
                   
                }
               
            }
            
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message, e.InnerException);
            }
            finally
            {
            }

            return null; // default state
        }
    }
}
