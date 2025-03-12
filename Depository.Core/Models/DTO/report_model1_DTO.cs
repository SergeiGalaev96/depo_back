using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Core.Models.DTO
{
    //Зачисление ЦБ на счет депо с переоформлением в рестре владельцев
    //Re-registration in the register of owners

    public class report_deponent_model
    {
        public string security_code { get; set; }
        public string issuer_name { get; set; }
        public string security_type_name { get; set; }
        public List<report_service_model> reportList { get; set; }
        public report_deponent_model()
        {
            reportList = new List<report_service_model>();
        }
    }
    public class report_service_model
    {
        public int instruction_number { get; set; }
        public DateTime createdDate { get; set; }
        public double quantity { get; set; }
        public double? comission { get; set; }
        public int instruction_type_id { get; set; }
    }
}
