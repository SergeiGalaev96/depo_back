using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Depository.MassTransitQueue
{
    public class EventConsumer : IConsumer<ValueEntered>
    {
        private readonly ILogger<EventConsumer> _logger;

        public EventConsumer(ILogger<EventConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<ValueEntered> context)
        {
            _logger.LogInformation($"value : {context.Message.Value}");

            return Task.CompletedTask;
        }
    }
}
