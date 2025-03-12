using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MassTransit;
using Depository.MassTransit.ViewModel;
using Depository.MassTransitQueue;
using Depository.Core;
using Depository.Core.Models;

namespace Depository.MassTransit.Controllers
{
    [Route("api/[controller]/[action]")]
    public class InstructionController : Controller
    {
        readonly IPublishEndpoint _publishEndpoint;
        public InstructionController(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }


        [HttpGet]
        public async Task<IActionResult> PassActionResult(string entityOperationResultWithUserDTO)
        {
            await _publishEndpoint.Publish<ValueEntered>(new
            {
                Value = entityOperationResultWithUserDTO
            });
            return Ok();
        }
    }
}
