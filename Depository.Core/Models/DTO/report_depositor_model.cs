using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.Models.DTO
{
    public class general_depositor_model
    {
        public int id { get; set; }
        public DateTime? date { get; set; }
        public double? quantity { get; set; }
        public double service_main { get; set; }
        public double service_transit { get; set; }
        public double service_total { get; set; }

        public int? security_id { get; set; }
        public int? currency_id { get; set; }
        public int? issuer_id { get; set; }
        public int depositor_id { get; set; }

        public int service_type_id { get; set; }

        public int service_group_id { get; set; }
        public string service_group_name { get; set; }
        public string service_type_name { get; set; }
        public string issuer_name { get; set; }
        public string security_name { get; set; }
        public string currency_name { get; set; }
        public string security_type_name { get; set; }
        public string depositor_name { get; set; }
    }


    public class service_model
    {
        public int security_id { get; set; }
        public int service_type_id { get; set; }
        public int service_group_id { get; set; }

        public int id { get; set; }
        public DateTime date { get; set; }
        public double quantity { get; set; }
        public double service_main { get; set; }
        public double service_transit { get; set; }
        public double service_total { get; set; }
    }

    public class security_model
    {
        public int security_id { get; set; }
        public int service_type_id { get; set; }
        public int service_group_id { get; set; }

        public string security_code { get; set; }
        public string security_type_name { get; set; }
        public string issuer_name { get; set; }
        public List<service_model> service_model_list { get; set; }
        public double amount_main { get; set; }
        public double amount_transit { get; set; }
        public double amount_total { get; set; }

        public security_model()
        {
            service_model_list = new List<service_model>();
        }
    }

    public class service_type_model
    {
        public int service_type_id { get; set; }
        public int service_group_id { get; set; }

        public string service_type_name { get; set; }
        public List<security_model> security_model_list { get; set; }

        public double amount_main { get; set; }
        public double amount_transit { get; set; }
        public double amount_total { get; set; }


        public service_type_model()
        {
            security_model_list = new List<security_model>();
        }
    }

    public class service_group
    {
        public int security_id { get; set; }
        public int service_type_id { get; set; }
        public int service_group_id { get; set; }
    }

    public class service_group_model
    {
        public int service_group_id { get; set; }

        public string service_group_name { get; set; }
        public List<service_type_model> service_type_model_list { get; set; }

        public double amount_main { get; set; }
        public double amount_transit { get; set; }
        public double amount_total { get; set; }
        public service_group_model()
        {
            service_type_model_list = new List<service_type_model>();
        }
        
    }
    public class report_depositor_model
    {
        public string depositor_name { get; set; }
        public DateTime dt_from { get; set; }
        public DateTime dt_to { get; set; }
        public List<service_group_model> service_group_model_list { get; set; }
        public double amount_main { get; set; }
        public double amount_transit { get; set; }
        public double amount_total { get; set; }
        public report_depositor_model()
        {
            service_group_model_list = new List<service_group_model>();
        }
    }
}
