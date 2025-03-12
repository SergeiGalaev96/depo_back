using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using Depository.DAL.DbContext;
using Depository.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Depository.Api.Controllers
{
    [Route("api/[controller]/[action]")]

    public class TradingSystemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TradingSystemsController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly ITrading_SystemsService _trading_SystemsService;


        public TradingSystemsController(ILogger<TradingSystemsController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, ITrading_SystemsService trading_SystemsService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _trading_SystemsService = trading_SystemsService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<trading_systems> entityOperationResult, string action)
        {
            if (entityOperationResult.IsSuccess)
                _logger.LogInformation(action + " : " + JsonConvert.SerializeObject(entityOperationResult));
            else _logger.LogError(action + " : " + JsonConvert.SerializeObject(entityOperationResult));
        }

        private async Task WriteToLogException(Exception exception, string action)
        {
            _logger.LogError(action + " : " + exception.ToString());
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] trading_systems trading_system, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<trading_systems>> entityOperationResults = new List<EntityOperationResult<trading_systems>>();
                var entityOperationResult = await _trading_SystemsService.CreateTrading_System(trading_system, user_guid);
                entityOperationResults.Add(entityOperationResult);
                WriteToLogEntityOperationResult(entityOperationResult, "Create");
                return Ok(entityOperationResults);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Create");
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Gets()
        {
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var accounts = unitOfWork.trading_systems.GetList();
                    return Ok(accounts);
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var account = unitOfWork.trading_systems.Get(id);
                    return Ok(account);
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }



        [HttpPost]
        public async Task<ActionResult> Update([FromBody] trading_systems trading_system, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<trading_systems>> entityOperationResults = new List<EntityOperationResult<trading_systems>>();
                var entityOperationResult = await _trading_SystemsService.UpdateTrading_System(trading_system, user_guid);
                entityOperationResults.Add(entityOperationResult);
                WriteToLogEntityOperationResult(entityOperationResult, "Update");
                return Ok(entityOperationResults);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Update");
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Delete([FromBody] int? id, Guid user_guid)
        {
            try
            {
                if (id == null) return BadRequest();
                List<EntityOperationResult<trading_systems>> entityOperationResults = new List<EntityOperationResult<trading_systems>>();
                var entityOperationResult = await _trading_SystemsService.DeleteTrading_System(id, user_guid);
                entityOperationResults.Add(entityOperationResult);
                WriteToLogEntityOperationResult(entityOperationResult, "Delete");
                return Ok(entityOperationResults);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Delete");
                return BadRequest();
            }
        }
    }
}
