using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Builder;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Depository.DAL.DbContext;
using Depository.DAL;
using Microsoft.EntityFrameworkCore;

using Depository.Domain.Services;
using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
using Depository.Api.MessageBroker;
using Depository.Mail.Services;
using Depository.Mail.Models;
using Rotativa.AspNetCore;
using System.Reflection;
using System.IO;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Http;

namespace Depository.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment _env;
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connection, hangfireConnection;
            if (!_env.IsDevelopment())
            {
                connection = Configuration.GetConnectionString("LocalConnection");
                hangfireConnection = Configuration.GetConnectionString("LocalHangfireConnection");
            }
            else
            {
                //connection = Configuration.GetConnectionString("DefaultConnection");
                connection = Configuration.GetConnectionString("LocalConnection");
                hangfireConnection = Configuration.GetConnectionString("LocalHangfireConnection");
            }
            var frontendRabbitSection = Configuration.GetSection("FrontendRabbitConnection");

            IServiceCollection serviceCollection = services.AddHangfire(x => x.UsePostgreSqlStorage(hangfireConnection));
            services.AddHangfireServer();

            services.AddSingleton<IConfiguration>(Configuration);
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connection));
            services.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql(connection);
            services.AddSingleton<IApplicationDbContextFactory>(
                sp => new ApplicationDbContextFactory(optionsBuilder.Options));
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            services.AddSwaggerGen(c =>
            c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true)
            );


            services.AddAutoMapper(typeof(Startup));

            services.AddHttpClient();

            services.AddScoped<IExchange_RatesService, Exchange_RatesService>();
            services.AddScoped<IMetadataService, MetadataService>();
            services.AddScoped<IAccountsService, AccountService>();
            services.AddScoped<IBanksService, BanksService>();
            services.AddScoped<ICountriesService, CountriesService>();
            services.AddScoped<IDepositorsService, DepositorsService>();
            services.AddScoped<IExchangesService, ExchangeService>();
            services.AddScoped<IIssuersService, IssuersService>();
            services.AddScoped<ILocalitiesService, LocalitiesService>();
            services.AddScoped<IPartnersService, PartnersService>();
            services.AddScoped<IPartner_ContactsService, Partner_ContactsService>();
            services.AddScoped<IUser_RolesService, User_RolesService>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IAccount_TypesService, Account_TypesService>();
            services.AddScoped<IDepositoriesService, DepositoriesService>();
            services.AddScoped<IInstructionsService, InstructionsService>();
            services.AddScoped<IInstruction_TypesService, Instruction_TypesService>();
            services.AddScoped<ISecuritiesService, SecuritiesService>();
            services.AddScoped<ITrading_SystemsService, Trading_SystemsService>();
            services.AddScoped<ICorr_DepositoriesService, Corr_DepositoriesService>();
            services.AddScoped<IInstruction_Account_RelationsService, Instruction_Account_RelationsService>();
            services.AddScoped<IRegistrarsService, RegistrarsService>();
            services.AddScoped<IAccount_MembersService, Account_MembersService>();
            services.AddScoped<IAccount_StatusesService, Account_StatusesService>();
            services.AddScoped<IAccounting_EntryService, Accounting_EntryService>();
            services.AddScoped<ICurrenciesService, CurrencyService>();
            services.AddScoped<ISecurity_TypesService, Security_TypesService>();
            services.AddScoped<ICitiesService, CitiesService>();
            services.AddScoped<IPartner_TypesService, Partner_TypesService>();
            services.AddScoped<IService_GroupsService, Service_GroupsService>();
            services.AddScoped<IService_TypesService, Service_TypesService>();
            services.AddScoped<ITariffs_cdService, Tariffs_cdService>();
            services.AddScoped<ITariffs_RegistrarsService, Tariffs_RegistrarsService>();
            services.AddScoped<ICoefficient_DepositorsService, Coefficient_DepositorsService>();
            services.AddScoped<ITariffs_Corr_DepositoryService, Tariffs_Corr_DepositoryService>();
            services.AddScoped<ICharge_For_Cd_ServicesService, Charge_For_Cd_ServicesService>();
            services.AddScoped<IPayments_For_Cd_ServicesService, Payments_For_Cd_ServicesService>();
            services.AddScoped<ITransit_Charge_For_Cd_ServicesService, Transit_Charge_For_Cd_ServicesService>();
            services.AddScoped<ITransit_Payments_For_Cd_ServicesService, Transit_Payments_For_Cd_ServicesService>();
            services.AddScoped<IPayer_TypesService, Payer_TypesService>();
            services.AddScoped<IRecipient_TypesService, Recipient_TypesService>();
            services.AddScoped<ILegal_StatusesService, Legal_StatusesService>();
            services.AddScoped<IAccounts_GikService, Accounts_GikService>();
            services.AddScoped<IQuestinaries_GikService, Questinaries_GikService>();
            services.AddScoped<IReport_TypesService, Report_TypesService>();
            services.AddScoped<IImported_Files_GikService, Imported_Files_GikService>();
            services.AddScoped<IAccount_ManagersService, Account_ManagersService>();
            services.AddScoped<ITradesService, TradesService>();
            services.AddScoped<IContractsService, ContractService>();
            services.AddScoped<IOrders_History_SecuritiesService, Orders_History_SecuritiesService>();
            services.AddScoped<IOrders_History_CurrenciesService, Orders_History_CurrenciesService>();
            services.AddScoped<ITrades_History_SecuritiesService, Trades_History_SecuritiesService>();
            services.AddScoped<ITrades_History_CurrenciesService, Trades_History_CurrenciesService>();
            services.AddScoped<IStock_SecurityService, Stock_SecurityService>();
            services.AddScoped<IStock_CurrencyService, Stock_CurrencyService>();
            services.AddScoped<IOper_DaysService, Oper_DaysService>();
            services.AddScoped<IInstruction_StatusesService, Instruction_StatusesService>();
            services.AddScoped<IIt_BasesService, It_BasesService>();
            services.AddScoped<IInstruction_ReportsService, Instruction_ReportsService>();
            services.AddScoped<IInstruction_Types_User_PermissionsService, Instruction_Types_User_PermissionsService>();
            services.AddScoped<ITransfer_OrdersService, Transfer_OrdersService>();
            services.AddScoped<IMortgage_SecuritiesService, Mortgage_SecuritiesService>();
            services.AddScoped<IInstructions_gikService, Instructions_gikService>();
            services.AddScoped<ITransfer_order_statusesService, Transfer_order_statusesService>();
            services.AddScoped<IMailsService, MailsService>();
            services.AddScoped<IMail_statusesService, Mail_statusesService>();
            services.AddScoped<IMail_distributionsService, Mail_distributionsService>();
            services.AddScoped<INew_Security_ApplicationsService, New_Security_ApplicationsService>();
            services.AddScoped<INew_Security_Application_TypesService, New_Security_Application_TypesService>();
            services.AddScoped<IExcertsService, ExcertsService>();
            services.AddScoped<IMail_TypesService, Mail_TypesService>();
            services.AddScoped<ISecurity_Issue_Form_TypesService, Security_Issue_Form_TypesService>();
            services.AddScoped<IIncoming_PackagesService, Incoming_PackagesService>();
            services.AddScoped<ISchedule_TasksService, Schedule_TasksService>();
            services.AddScoped<IOutgoing_PackagesService, Outgoing_PackagesService>();
            services.AddScoped<ITax_TypesService, Tax_TypesService>();
            services.AddScoped<ITask_TypesService, Task_TypesService>();
            services.AddScoped<IThsTasksService, ThsTasksService>();
            services.AddScoped<ILimitsService, LimitsService>();
            services.AddScoped<ISecurityLocationsService, SecurityLocationsService>();
            services.AddScoped<IGovSecurityPaymentsService, GovSecurityPaymentsService>();
            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            services.AddTransient<IMailService, MailService>();
            services.AddScoped<ISalesTaxesService, SalesTaxesService>();
            services.AddScoped<IVatsService, VatsService>();
            services.AddScoped<IPaymentTypesService, PaymentTypesService>();
            services.AddScoped<IAccountingEntryStopTypesService, AccountingEntryStopTypesService>();
            services.AddScoped<IInstructionRegistrarReportsService, InstructionRegistrarReportsService>();
            services.AddScoped<IInstructions_gikService, Instructions_gikService>();
            services.AddScoped<IInstructionGikStatusesService, InstructionGikStatusesService>();
            services.AddScoped<IInstructionTypesGikService, InstructionTypesGikService>();

            services.AddControllers().AddNewtonsoftJson(options =>

                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            services.AddControllers();
            services.AddControllersWithViews(options =>
            {
                options.AllowEmptyInputInBodyModelBinding = true;
            });
            services.AddMvcCore();
        }

        private readonly string swaggerBasePath = "back";

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{

            app.UseDeveloperExceptionPage();
            //}
            app.UsePathBase("/back");
            app.UseRouting();

            // app.UseHttpsRedirection();
            app.UseSwagger();



            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("./v1/swagger.json", "Depository API V1");

            });



            app.UseRouting();

            app.UseAuthorization();
            app.UseHangfireDashboard("/dashboard");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=instructions}/{action=Privacy}");
            });

            RotativaConfiguration.Setup(env.WebRootPath, "Rotativa");
        }
    }
}

