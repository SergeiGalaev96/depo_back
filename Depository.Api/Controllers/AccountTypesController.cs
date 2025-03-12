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
    public class AccountTypesController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountTypesController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IAccount_TypesService _account_typesService;
        public AccountTypesController(ILogger<AccountTypesController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IAccount_TypesService account_typesService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _account_typesService = account_typesService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<account_types> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] account_types account_type, Guid user_guid)
        {
            try
            { 
                List<EntityOperationResult<account_types>> entityOperationResults = new List<EntityOperationResult<account_types>>();
                var entityOperationResult = await _account_typesService.CreateAccount_Type(account_type, user_guid);
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
                    var account_types = unitOfWork.account_types.GetList();
                    return Ok(account_types);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetAccountTypes");
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
                    var account_type = unitOfWork.account_types.Get(id);
                    return Ok(account_type);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetAccountType");
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] account_types account_type, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<account_types>> entityOperationResults = new List<EntityOperationResult<account_types>>();
                var entityOperationResult = await _account_typesService.UpdateAccount_Type(account_type, user_guid);
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
                List<EntityOperationResult<account_types>> entityOperationResults = new List<EntityOperationResult<account_types>>();
                var entityOperationResult = await _account_typesService.DeleteAccount_Type(id, user_guid);
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
