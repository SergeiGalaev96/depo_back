
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Depository.TopshelfService
{
    public class InstructionConsumer : IConsumer<object>
    {
        private readonly ILogger<InstructionConsumer> _logger;

        public InstructionConsumer(ILogger<InstructionConsumer> logger)
        {
            _logger = logger;
        }
        public Task Consume(ConsumeContext<object> consumeContext)
        {
            _logger.LogInformation($"value : { JsonConvert.SerializeObject(consumeContext.Message)}");
            return Task.CompletedTask;
        }
    }
}
