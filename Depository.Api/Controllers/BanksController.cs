using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Depository.DAL;
using Depository.DAL.DbContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Depository.Core.Models;
using Newtonsoft.Json;
using Depository.Domain.Services;
using Depository.Core;
using Microsoft.Extensions.Logging;

namespace Depository.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    
    public class BanksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BanksController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IBanksService _banksService;
        public BanksController(ILogger<BanksController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IBanksService banksService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _banksService = banksService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<banks> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] banks bank, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<banks>> entityOperationResults = new List<EntityOperationResult<banks>>();
                var entityOperationResult = await _banksService.CreateBank(bank, user_guid);
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
                    var banks = unitOfWork.banks.GetList();
                    return Ok(banks);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetBanks");
                return Ok(ex);
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
                    var bank =  unitOfWork.banks.Get(id);
                    return  Ok(bank);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetBank");
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] banks bank,  Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<banks>> entityOperationResults = new List<EntityOperationResult<banks>>();
                var entityOperationResult = await _banksService.UpdateBank(bank, user_guid);
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
        public async Task<ActionResult> Delete([FromBody] int id, Guid user_guid)
        {
            try
            {
                if (id == null)  return BadRequest();
                List<EntityOperationResult<banks>> entityOperationResults = new List<EntityOperationResult<banks>>();
                var entityOperationResult = await _banksService.DeleteBank(id, user_guid);
                entityOperationResults.Add(entityOperationResult);
                WriteToLogEntityOperationResult(entityOperationResult, "Update");
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
