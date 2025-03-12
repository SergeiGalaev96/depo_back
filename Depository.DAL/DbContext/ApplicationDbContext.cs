using System;
using System.Collections.Generic;
using System.Text;
using Depository.Core.Models;
using Depository.Core.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace Depository.DAL.DbContext
{
    public class ApplicationDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<banks> banks { get; set; }
        public DbSet<exchange_rates> exchange_rates { get; set; }
        public DbSet<currencies> currencies { get; set; }
        public DbSet<metadata> metadata { get; set; }
        public DbSet<accounts> accounts { get; set; }
        public DbSet<countries> countries { get; set; }
        public DbSet<depositors> depositors { get; set; }
        public DbSet<exchanges> exchanges { get; set; }
        public DbSet<issuers> issuers { get; set; }
        public DbSet<localities> localities { get; set; }
        public DbSet<partner_contacts> partner_contacts { get; set; }
        public DbSet<partners> partners { get; set; }
        public DbSet<user_roles> user_roles { get; set; }
        public DbSet<history> history { get; set; }
        public DbSet<users> users { get; set; }
        public DbSet<account_types> account_types { get; set; }
        public DbSet<depositories> depositories { get; set; }
        public DbSet<instructions> instructions { get; set; }
        public DbSet<instruction_types> instruction_types { get; set; }
        public DbSet<securities> securities { get; set; }
        public DbSet<trading_systems> trading_systems { get; set; }
        public DbSet<corr_depositories> corr_depositories { get; set; }
        public DbSet<security_types> security_types { get; set; }
        public DbSet<registrars> registrars { get; set; }
        public DbSet<directory> directory { get; set; }
        public DbSet<account_members> account_members { get; set; }
        public DbSet<account_statuses> account_statuses { get; set; }
        public DbSet<instruction_account_relations> instruction_account_relations { get; set; }
        public DbSet<accounting_entry> accounting_entry { get; set; }
        public DbSet<transactions> transactions { get; set; }
        public DbSet<cities> cities { get; set; }
        public DbSet<regions> regions { get; set; }
        public DbSet<stock_security> stock_security { get; set; }
        public DbSet<stock_currency> stock_currency { get; set; }
        public DbSet<partner_types> partner_types { get; set; }
        public DbSet<service_groups> service_groups { get; set; }
        public DbSet<service_types> service_types { get; set; }
        public DbSet<tariffs_cd> tariffs_cd { get; set; }
        public DbSet<tariffs_registrars> tariffs_registrars { get; set; }
        public DbSet<coefficient_depositors> coefficient_depositors { get; set; }
        public DbSet<tariffs_corr_depository> tariffs_corr_depository { get; set; }
        public DbSet<charge_for_cd_services> charge_for_cd_services { get; set; }
        public DbSet<payments_for_cd_services> payments_for_cd_services { get; set; }
        public DbSet<transit_charge_for_cd_services> transit_charge_for_cd_services { get; set; }
        public DbSet<transit_payments_for_cd_services> transit_payments_for_cd_services { get; set; }
        public DbSet<payer_types> payer_types { get; set; }
        public DbSet<recipient_types> recipient_types { get; set; }
        public DbSet<legal_statuses> legal_statuses { get; set; }
        public DbSet<accounts_gik> accounts_gik { get; set; }
        public DbSet<questinaries_gik> questinaries_gik { get; set; }
        public DbSet<report_types> report_types { get; set; }
        public DbSet<imported_files_gik> imported_files_gik { get; set; }
        public DbSet<account_managers> account_managers { get; set; }
        public DbSet<contracts> contracts { get; set; }
        public DbSet<trades> trades { get; set; }
        public DbSet<orders_history_currencies> orders_history_currencies { get; set; }
        public DbSet<orders_history_securities> orders_history_securities { get; set; }
        public DbSet<trades_history_currencies> trades_history_currencies { get; set; }
        public DbSet<trades_history_securities> trades_history_securities { get; set; }
        public DbSet<history> histories { get; set; }
        public DbSet<oper_days> oper_days { get; set; }
        public DbSet<instruction_statuses> instruction_statuses { get; set; }
        public DbSet<it_bases> it_bases { get; set; }
        public DbSet<instruction_reports> instruction_reports { get; set; }
        public DbSet<instruction_types_user_permissions> instruction_types_user_permissions { get; set; }
        public DbSet<transfer_orders> transfer_orders { get; set; }
        public DbSet<mortgage_securities> mortgage_securities { get; set; }
        public DbSet<instructions_gik> instructions_gik { get; set; }
        public DbSet<transfer_order_statuses> transfer_order_statuses { get; set; }
        public DbSet<mail_statuses> mail_statuses { get; set; }
        public DbSet<mails> mails { get; set; }
        public DbSet<mail_distributions> mail_distributions {get;set;}
        public DbSet<new_security_applications> new_security_applications { get; set; }
        public DbSet<new_security_application_types> new_security_application_types { get; set; }
        public DbSet<trades_history_statuses> trades_history_statuses { get; set; }
        public DbSet<accounting_report_types> accounting_report_types { get; set; }
        public DbSet<excerts> excerts { get; set; }
        public DbSet<security_balance> security_balance { get; set; }
        public DbSet<mail_types> mail_types { get; set; }
        public DbSet<security_issue_form_types> security_issue_form_types { get; set; }
        public DbSet<sectors> sectors { get; set; }

        public DbSet<ths_tasks> ths_tasks { get; set; }
        public DbSet<schedule_tasks> schedule_tasks { get; set; }
        public DbSet<task_types> task_types { get; set; }
        public DbSet<incoming_packages> incoming_packages { get; set; }
        public DbSet<outgoing_packages> outgoing_packages { get; set; }
        public DbSet<tax_types> tax_types { get; set; }
        public DbSet<limits> limits { get; set; }
        public DbSet<security_location> security_location { get; set; }
        public DbSet<gov_securities_payments> gov_securities_payments { get; set; }
        public DbSet<vats> vats { get; set; }
        public DbSet<sales_taxes> sales_taxes { get; set; }
        public DbSet<payment_types> payment_types { get; set; }
        public DbSet<accounting_entry_stop_types> accounting_entry_stop_types { get; set; }
        public DbSet<stock_security_view> stock_security_view { get; set; }
        public DbSet<stock_currency_view> stock_currency_view { get; set; }
        public DbSet<instruction_registrar_reports> instruction_registrar_reports { get; set; }
        public DbSet<instructions_gik_statuses> instructions_gik_statuses { get; set; }
        public DbSet<instruction_types_gik> instruction_types_gik { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<instructions>()
                .HasOne(i => i.issuers)
                .WithMany(i => i.instructions)
                .HasForeignKey(f => f.issuer);
            modelBuilder
                .Entity<instructions>()
                .HasOne(i => i.depositors)
                .WithMany(d => d.instructions)
                .HasForeignKey(f => f.depositor);
           modelBuilder
                .Entity<mail_distributions>()
                .HasOne(m=>m.mails)
                .WithMany(d=>d.mail_distributions)
                .HasForeignKey(f => f.mail_id);
            modelBuilder
                .Entity<depositors>()
                .HasOne(d => d.partners)
                .WithMany(p => p.depositors)
                .HasForeignKey(f => f.partner);
            modelBuilder
               .Entity<instructions>()
               .HasOne(i => i.securities)
               .WithMany(s => s.instructions)
               .HasForeignKey(f => f.security);
            modelBuilder
                .Entity<instructions>()
                .HasOne(i => i.instruction_types)
                .WithMany(t => t.instructions)
                .HasForeignKey(f => f.type);
            modelBuilder
                .Entity<instructions>()
                .HasOne(i => i.currencies)
                .WithMany(c => c.instructions)
                .HasForeignKey(f => f.currency);
            modelBuilder
                .Entity<securities>()
                .HasOne(s => s.security_types)
                .WithMany(t => t.securities)
                .HasForeignKey(f => f.security_type);
            modelBuilder
                .Entity<securities>()
                .HasOne(s => s.issuers)
                .WithMany(i => i.securities)
                .HasForeignKey(f => f.issuer);
            modelBuilder
                .Entity<partner_contacts>()
                .HasOne(p => p.partners)
                .WithMany(c => c.partner_contacts)
                .HasForeignKey(f => f.partner);
            modelBuilder
                .Entity<registrars>()
                .HasOne(r => r.partners)
                .WithMany(p => p.registrars)
                .HasForeignKey(f => f.partner);
            modelBuilder
                .Entity<depositories>()
                .HasOne(d => d.partners)
                .WithMany(p => p.depositories)
                .HasForeignKey(f => f.partner);
            modelBuilder
                .Entity<corr_depositories>()
                .HasOne(d => d.partners)
                .WithMany(p => p.corr_depositories)
                .HasForeignKey(f => f.partner);
            modelBuilder
                .Entity<issuers>()
                .HasOne(i => i.registrars)
                .WithMany(r => r.issuers)
                .HasForeignKey(f => f.registrar);
            modelBuilder
                .Entity<accounts>()
                .HasOne(a => a.partners)
                .WithMany(p => p.accounts)
                .HasForeignKey(f => f.partner);
                
        }
    }
}
