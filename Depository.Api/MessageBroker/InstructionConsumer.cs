using Depository.Core;
using Depository.Core.Models;
using Depository.Core.Models.DTO;
using Depository.Domain.Services;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Depository.Api.MessageBroker
{
    public class InstructionConsumer:IConsumer<InstructionWithUserDTO>
    {
        private readonly ILogger<InstructionConsumer> _logger;
        private readonly IInstructionsService _instructionsService;
        private IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        public InstructionConsumer(IHttpClientFactory httpClientFactory, ILogger<InstructionConsumer> logger, IInstructionsService instructionsService, IConfiguration configuration)
        {
            _logger = logger;
            _instructionsService = instructionsService;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;

        }
        public  Task Consume(ConsumeContext<InstructionWithUserDTO> instructionWithUserDTOContext)
        {
            var user_guid = instructionWithUserDTOContext.Message._user_guid;
            _logger.LogInformation($"Taked value from RabbitMQ : { JsonConvert.SerializeObject(instructionWithUserDTOContext.Message)}");
            var entityOperationResult =  _instructionsService.CreateInstruction(instructionWithUserDTOContext.Message._instruction, instructionWithUserDTOContext.Message._user_guid);
            EntityOperationResultWithUserDTO entityOperationResultWithUserDTO = new EntityOperationResultWithUserDTO(JsonConvert.SerializeObject(entityOperationResult.Result, Formatting.Indented,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                }), user_guid);
            var instructionResultLink = _configuration.GetValue<string>("Links:InstructionResultLink");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(instructionResultLink);
            var parameter = "?entityOperationResultWithUserDTO=" + JsonConvert.SerializeObject(entityOperationResultWithUserDTO);
            string result = client.GetStringAsync(parameter).Result;
            _logger.LogInformation("Query string result from query", result);
            return Task.CompletedTask;
        }

       

    }
}
