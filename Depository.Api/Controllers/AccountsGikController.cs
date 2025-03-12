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
    public class AccountsGikController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountsGikController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IAccounts_GikService _accounts_GikService;
        public AccountsGikController(ILogger<AccountsGikController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IAccounts_GikService accounts_GikService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _accounts_GikService = accounts_GikService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<accounts_gik> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] accounts_gik account_gik, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<accounts_gik>> entityOperationResults = new List<EntityOperationResult<accounts_gik>>();
                var entityOperationResult = await _accounts_GikService.CreateAccount_Gik(account_gik, user_guid);
                entityOperationResults.Add(entityOperationResult);
                WriteToLogEntityOperationResult(entityOperationResult, "Create");
                return Ok(entityOperationResults);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Create");
                return Ok(ex.ToString());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Gets()
        {
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var accounts_gik = unitOfWork.accounts_gik.GetList();
                    return Ok(accounts_gik);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetAccounts_gik");
                return Ok(ex.ToString());
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
                    var account_gik = unitOfWork.accounts_gik.Get(id);
                    return Ok(account_gik);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Getaccount_gik");
                return Ok(ex.ToString());
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] accounts_gik account_gik, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<accounts_gik>> entityOperationResults = new List<EntityOperationResult<accounts_gik>>();
                var entityOperationResult = await _accounts_GikService.UpdateAccount_Gik(account_gik, user_guid);
                entityOperationResults.Add(entityOperationResult);
                WriteToLogEntityOperationResult(entityOperationResult, "Update");
                return Ok(entityOperationResults);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Update");
                return Ok(ex.ToString());
            }
        }

        [HttpPost]
        public async Task<ActionResult> Delete([FromBody] int? id, Guid user_guid)
        {
            try
            {
                if (id == null) return BadRequest();
                List<EntityOperationResult<accounts_gik>> entityOperationResults = new List<EntityOperationResult<accounts_gik>>();
                var entityOperationResult = await _accounts_GikService.DeleteAccount_Gik(id, user_guid);
                entityOperationResults.Add(entityOperationResult);
                WriteToLogEntityOperationResult(entityOperationResult, "Delete");
                return Ok(entityOperationResults);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Delete");
                return Ok(ex.ToString());
            }
        }
    }
}
