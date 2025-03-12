using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;

namespace Depository.HangFire.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TasksController : Controller
    {
        private readonly IConfiguration _configuration;
        public TasksController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public IActionResult SetOpenClosingDays(Guid user_guid)
        {
            RecurringJob.AddOrUpdate(() => SetOpen(user_guid), Cron.Daily(9, 0));
            RecurringJob.AddOrUpdate(() => SetClose(user_guid), Cron.Daily(18, 0));
            return Ok("Ok");
        }


        
        public async Task<string> SetOpen(Guid user_guid)
        {
            var apiLink = _configuration.GetValue<string>("Links:OperDayOpenLink")+"?user_guid="+user_guid.ToString();
            using (var client = new HttpClient())
            {
                StringContent data = new StringContent("\""+DateTime.Now.ToString()+"\"", Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiLink, data);
                string result = await response.Content.ReadAsStringAsync();
                return result;
            }
            
        }



        
        public async Task<string> SetClose(Guid user_guid)
        {
            var apiLink = _configuration.GetValue<string>("Links:OperDayCloseLink") + "?user_guid=" + user_guid.ToString();
            using (var client = new HttpClient())
            {
                StringContent data = new StringContent("\""+DateTime.Now.ToString()+ "\"", Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiLink, data);
                string result = await response.Content.ReadAsStringAsync();
                return result;
            }

        }
    }
}
