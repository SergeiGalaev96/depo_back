using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Depository.Mail.Models;
using Depository.Mail.Services;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Depository.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    public class MailController : ControllerBase
    {
       // private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<MailController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMailService _mailService;

        public MailController(IMailService mailService, /*IPublishEndpoint publishEndpoint,*/ ILogger<MailController> logger, IConfiguration configuration)
        {
           // _publishEndpoint = publishEndpoint;
            _logger = logger;
            _configuration = configuration;
            _mailService = mailService;
        }

        [HttpPost]
        public async Task<ActionResult> SendMail([FromForm] MailRequest mailRequest)
        {
            try
            {
                var result=_mailService.SendEmailAsync(mailRequest);
               // await _publishEndpoint.Publish<MailRequest>(mailRequest);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ex);
            }

        }
    }
}
