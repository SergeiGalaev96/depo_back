using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Depository.HangFire.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RateTaskController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public RateTaskController(IConfiguration configuration)
        {
            _configuration = configuration;                
        }

        [HttpGet]
        public IActionResult SetTask()
        {
            RecurringJob.AddOrUpdate(() => GetRates(), Cron.Daily());
            return Ok("Ok");
        }

        
        public async Task<string> GetRates()
        {
            var apiLink = _configuration.GetValue<string>("Links:RateLink");
            string result = "";
            using (var client = new HttpClient())
            {
                result = await client.GetStringAsync(apiLink);
            }
            return result;
        }
    }
}
