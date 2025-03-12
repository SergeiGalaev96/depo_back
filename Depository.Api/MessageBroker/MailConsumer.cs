using Depository.Mail.Models;
using Depository.Mail.Services;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Depository.Api.MessageBroker
{
   
    public class MailConsumer:IConsumer<MailRequest>
    {
        private readonly ILogger<MailConsumer> _logger;
        private readonly IMailService _mailService;
        private readonly IConfiguration _configuration;

        public MailConsumer(ILogger<MailConsumer> logger, IMailService mailService, IConfiguration configuration)
        {
            _logger = logger;
            _mailService = mailService;
            _configuration = configuration;
        }

        public Task Consume(ConsumeContext<MailRequest> context)
        {
            try
            {
                 _mailService.SendEmailAsync(context.Message);
                return Task.CompletedTask;
            }

            catch (Exception ex)
            {
                _logger.LogError("MailConsumer: " + ex.Message + ":" + ex.InnerException);
                throw;
            }
        }
    }
}
