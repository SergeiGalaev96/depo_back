using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models
{
    public class instructions_gik : Entity
    {
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public bool? deleted { get; set; }
        public int type { get; set; }
        public int? status { get; set; }
        public string? created_user { get; set; }
        public string? executed_user { get; set; }
        public DateTime? execution_time { get; set; }
        public string? canceled_user { get; set; }
        public string? cancelation_reason { get; set; }
        public DateTime? cancelation_time { get; set; }
        public string? issuer_full_name { get; set; }
        public string? issuer_short_name { get; set; }
        public string? gik_security_name { get; set; }
        public DateTime? agreement_date { get; set; }
        public string? agreement_number { get; set; }
        public double? interest_rate { get; set; }
        public string? obligation { get; set; }
        public double? debt_amount { get; set; }
        public string? state_registration_number { get; set; }
        public DateTime? state_registration_date { get; set; }
        public string? state_registration_authority_name { get; set; }
        public string? real_estate_name { get; set; }
        public bool? building_is_finished { get; set; }
        public string? building_address { get; set; }
        public string? building_purpose { get; set; }
        public double? building_area { get; set; }
        public DateTime? pricing_date { get; set; }
        public double? independent_appraiser_price { get; set; }
        public string? mortgage_registration_number { get; set; }
        public DateTime? mortgage_registration_date { get; set; }
        public string? authority_registered_mortgage_name { get; set; }
        public DateTime? mortgage_issue_date { get; set; }
        public string? authority_issued_mortgage_name { get; set; }
        public DateTime? issuer_acquisition_date { get; set; }
        public string? payment_deadline { get; set; }
        public string? debt_repayment_plan { get; set; }
        public double? monthly_payment_amount { get; set; }
        public string? admitted_to_trading { get; set; }
        public string? property_type { get; set; }
        public string? replacement_basis { get; set; }
        public DateTime? replacement_basis_occurrence_date { get; set; }
        public string? excluding_property_basis { get; set; }
        public DateTime? excluding_basis_occurrence_date { get; set; }
        public string? attached_supporting_documents { get; set; }
        public string? issuer_authorized_manager { get; set; }
        //public string issuer_name { get; set; }
        //public string inn { get; set; }
        //public string main_gov_reg_num { get; set; }
        //public string issuer_gov_reg_date { get; set; }
        //public string gov_reg_authority { get; set; }
        //public string mortgage_securities { get; set; }
        //public string mortgage_securities_reg_date { get; set; }
        //public string borrower_full_name { get; set; }
        //public string bank { get; set; }
        //public string loan_agreement_number { get; set; }
        //public string loan_agreement_date { get; set; }
        //public string credit_amount { get; set; }
        //public string credit_balance { get; set; }
        //public string credit_term { get; set; }
        //public string interest_rate { get; set; }
        //public string monthly_contribution_amount { get; set; }
        //public string overdue_principal_amount { get; set; }
        //public string overdue_principal_days { get; set; }
        //public string delinquencies_number_on_principal_amount { get; set; }
        //public string overdue_interest_amount { get; set; }
        //public string overdue_interest_days { get; set; }
        //public string interest_arrears_number { get; set; }
        //public string ltv { get; set; }
        //public string rppu { get; set; }
        //public string mortgage_gov_reg_authority { get; set; }
        //public string independent_evaluation { get; set; }
        //public string mortgage_loan_reg_number { get; set; }
        //public string mortgage_loan_reg_date { get; set; }
        //public string mortgage_loan_reg_authority { get; set; }
        //public string obligation_degree_performance_extinguished_od { get; set; }
        //public string obligation_degree_performance_interest_payments { get; set; }
        //public string not_fulfilled_interest_amount_od { get; set; }
        //public string loan_coverage_mortgage_security { get; set; }
        //public string mortgage_coverage_amount { get; set; }
        //public string independent_evaluation_date { get; set; }
        //public string pledge_type { get; set; }
        //public string pledge_address { get; set; }
        //public string gov_reg_mortgage_umber { get; set; }
        //public string gov_reg_mortgage_date { get; set; }
        //public string total_area { get; set; }
        //public string living_area { get; set; }
        //public string land_area { get; set; }
        //public string securities_type { get; set; }
        //public string securities_nominale { get; set; }
        //public string securities_issuing_gov_reg_date { get; set; }
        //public string securities_issuing_gov_reg_number { get; set; }

        //public string securities_maturity_date { get; set; }
        //public string securities_amount { get; set; }
        //public string securities_value { get; set; }
        //public string depository_name { get; set; }
        //public string depository_location { get; set; }
        //public string depository_license { get; set; }
        //public string depository_licence_issue_date { get; set; }
        //public string immovable_property_name { get; set; }
        //public string immovable_property_location { get; set; }

        //public string immovable_property_purpose { get; set; }
        //public string immovable_property_total_area { get; set; }
        //public string immovable_property_living_ares { get; set; }
        //public string immovable_property_land_area { get; set; }
        //public string immovable_property_cost { get; set; }
        //public string immovable_property_cost_determine_date { get; set; }
        //public string gov_reg_ownership_date { get; set; }
        //public string gov_reg_ownership_number { get; set; }
        //public string gov_reg_ownership_authority { get; set; }
        //public string currency { get; set; }
        //public string term_deposit_funds_amount { get; set; }
        //public string term_deposit_org_name { get; set; }
        //public string term_deposit_org_location { get; set; }
        //public string term_deposit_end_date { get; set; }

    }
}
