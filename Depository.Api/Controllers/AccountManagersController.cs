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
    public class AccountManagersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountManagersController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IAccount_ManagersService _account_ManagersService;
        public AccountManagersController(ILogger<AccountManagersController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IAccount_ManagersService account_ManagersService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _account_ManagersService = account_ManagersService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<account_managers> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] account_managers account_manager, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<account_managers>> entityOperationResults = new List<EntityOperationResult<account_managers>>();
                var entityOperationResult = await _account_ManagersService.CreateAccount_Manager(account_manager, user_guid);
                entityOperationResults.Add(entityOperationResult);
                WriteToLogEntityOperationResult(entityOperationResult, "Create");
                return Ok(entityOperationResults);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Create");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Gets()
        {
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var account_managers = unitOfWork.account_managers.GetList();
                    return Ok(account_managers);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Getsaccount_managers");
                return Ok(JsonConvert.SerializeObject(ex));
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
                    var account_manager = unitOfWork.account_managers.Get(id);
                    return Ok(account_manager);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Getaccount_manager");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] account_managers account_manager, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<account_managers>> entityOperationResults = new List<EntityOperationResult<account_managers>>();
                var entityOperationResult = await _account_ManagersService.UpdateAccount_Manager(account_manager, user_guid);
                entityOperationResults.Add(entityOperationResult);
                WriteToLogEntityOperationResult(entityOperationResult, "Update");
                return Ok(entityOperationResults);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Update");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }

        [HttpPost]
        public async Task<ActionResult> Delete([FromBody] int? id, Guid user_guid)
        {
            try
            {
                if (id == null) return BadRequest();
                List<EntityOperationResult<account_managers>> entityOperationResults = new List<EntityOperationResult<account_managers>>();
                var entityOperationResult = await _account_ManagersService.DeleteAccount_Manager(id, user_guid);
                entityOperationResults.Add(entityOperationResult);
                WriteToLogEntityOperationResult(entityOperationResult, "Delete");
                return Ok(entityOperationResults);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Delete");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }
    }
}
