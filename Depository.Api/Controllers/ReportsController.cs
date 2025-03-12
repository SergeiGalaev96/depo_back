using Depository.Core.Models;
using Depository.Core.Models.DTO;
using Depository.DAL;
using Depository.DAL.DbContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Api.Controllers
{

    [Route("api/[controller]/[action]")]

    public class ReportsController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReportsController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        public ReportsController(ILogger<ReportsController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
        }




        [HttpGet]
        public async Task<IActionResult> GetDeptReport(List<int> depositors, DateTime dtFrom, DateTime dtTo)
        {
            List<depositor_dept> depositor_depts = new List<depositor_dept>();
            foreach (var depositor in depositors)
            {
                depositor_dept depositor_dept = new depositor_dept();
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var _depositor = unitOfWork.depositors.Get(depositor);
                    if (_depositor == null) continue;
                    var partner = unitOfWork.partners.Get(_depositor.partner);
                    if (partner == null) continue;
                    else depositor_dept.depositor_name = partner.name;
                    var service_types = unitOfWork.service_types.GetList();
                    var depts_all = unitOfWork.charge_for_cd_services.GetDeptByDepositor(depositor, dtFrom);
                    var payments_all = unitOfWork.payments_for_cd_services.GetPaymentsByDepositor(depositor, dtFrom);



                    var dept_difference = depts_all - payments_all;
                    depositor_dept.dept_before = dept_difference.Value;
                    var service_type_depts = unitOfWork.charge_for_cd_services.GetServiceTypeDeptByDepositor(depositor, dtFrom, dtTo);
                    foreach (var service_type_dept in service_type_depts)
                    {
                        var service_type_name = service_types.Where(x => x.id == service_type_dept.service_type_id).FirstOrDefault().name;
                        service_type_dept.service_type_name = service_type_name;
                        depositor_dept.service_type_depts.Add(service_type_dept);
                    }
                    depositor_dept.payments_between_period = unitOfWork.payments_for_cd_services.GetPaymentsBetweenPeriod(depositor, dtFrom, dtTo);
                    depositor_dept.without_vat_total = depositor_dept.service_type_depts.Sum(x => x.without_vat);
                    depositor_dept.total = depositor_dept.service_type_depts.Sum(x => x.total);
                    depositor_dept.transit_total = depositor_dept.service_type_depts.Sum(x => x.transit);
                    depositor_dept.amount_total_all = depositor_dept.service_type_depts.Sum(x => x.amount_total);
                    depositor_dept.dept_after = depositor_dept.dept_before + depositor_dept.amount_total_all - depositor_dept.payments_between_period;
                    depositor_depts.Add(depositor_dept);
                }
            }
            return Ok(depositor_depts);
        }


        [HttpPost]
        public async Task<byte[]> Retranslator([FromBody]object bodyValue, string headerValue)
        {
            var url = _configuration.GetValue<string>("Links:JasperReportRetranslatorGetTempValueLink");
            var url2 = _configuration.GetValue<string>("Links:JasperReportRetranslatorGetBlobValueLink");
            var client = new HttpClient();
            client.DefaultRequestHeaders
               .Accept
               .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + headerValue);
            StringContent content = new StringContent(JsonConvert.SerializeObject(bodyValue), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            
            string result = await response.Content.ReadAsStringAsync();
            var reportExecutionModel=JsonConvert.DeserializeObject<report_execution_model>(result);
            url2 = url2.Replace("requestId", reportExecutionModel.requestId).Replace("exportId", reportExecutionModel.exports[0].id);
            var response2 = await client.GetByteArrayAsync(url2);
            client.Dispose();
            return response2;
        }



        [HttpPost]
        public async Task<byte[]> Retranslator2([FromBody] string bodyValue, string headerValue)
        {
            var url = _configuration.GetValue<string>("Links:JasperReportRetranslatorGetTempValueLink");
            var url2 = _configuration.GetValue<string>("Links:JasperReportRetranslatorGetBlobValueLink");
            var client = new HttpClient();
            client.DefaultRequestHeaders
               .Accept
               .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + headerValue);
            StringContent content = new StringContent(JsonConvert.SerializeObject(bodyValue), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            string result = await response.Content.ReadAsStringAsync();
            var reportExecutionModel = JsonConvert.DeserializeObject<report_execution_model>(result);
            url2 = url2.Replace("requestId", reportExecutionModel.requestId).Replace("exportId", reportExecutionModel.exports[0].id);
            var response2 = await client.GetByteArrayAsync(url2);
            client.Dispose();
            return response2;
        }
    }
}
