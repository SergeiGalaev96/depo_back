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
    public class AccountStatusesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountStatusesController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IAccount_StatusesService _account_statusesService;

        public AccountStatusesController(ILogger<AccountStatusesController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IAccount_StatusesService account_statusesService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _account_statusesService = account_statusesService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<account_statuses> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] account_statuses account_status, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<account_statuses>> entityOperationResults = new List<EntityOperationResult<account_statuses>>();
                var entityOperationResult = await _account_statusesService.CreateAccount_Status(account_status, user_guid);
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
                    var account_statuses = unitOfWork.account_statuses.GetList();
                    return Ok(account_statuses);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetAccountStatuses");
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
                    var account_status = unitOfWork.account_statuses.Get(id);
                    return Ok(account_status);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetAccountStatus");
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] account_statuses account_status, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<account_statuses>> entityOperationResults = new List<EntityOperationResult<account_statuses>>();
                var entityOperationResult = await _account_statusesService.UpdateAccount_Status(account_status, user_guid);
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
                List<EntityOperationResult<account_statuses>> entityOperationResults = new List<EntityOperationResult<account_statuses>>();
                var entityOperationResult = await _account_statusesService.DeleteAccount_Status(id, user_guid);
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
