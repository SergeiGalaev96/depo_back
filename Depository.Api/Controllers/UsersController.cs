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
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UsersController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IUsersService _usersService;
        public UsersController(ILogger<UsersController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IUsersService usersService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _usersService = usersService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<users> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] users user,  Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<users>> entityOperationResults = new List<EntityOperationResult<users>>();
                var entityOperationResult = await _usersService.CreateUser(user, user_guid);
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
        public async Task<IActionResult> Gets(Guid user_guid)
        {
            List<users> users = new List<users>();
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var user = unitOfWork.users.GetByUserId(user_guid);
                    if (user != null)
                    {
                        users = unitOfWork.users.GetList();
                        
                    }
                }
                return Ok(users);
            }
            catch (Exception ex)
            {

                WriteToLogException(ex, "Create");
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid user_guid)
        {
            if (user_guid == Guid.Empty)
            {
                return BadRequest();
            }
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var user = unitOfWork.users.GetByUserId(user_guid);
                    return Ok(user);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Create");
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] users user,  Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<users>> entityOperationResults = new List<EntityOperationResult<users>>();
                var entityOperationResult = await _usersService.UpdateUser(user, user_guid);
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
        public async Task<ActionResult> Delete([FromBody] Guid user_id_to_delete,  Guid user_guid)
        {
            try
            { 
                if (user_id_to_delete==Guid.Empty) return BadRequest();
                List<EntityOperationResult<users>> entityOperationResults = new List<EntityOperationResult<users>>();
                var entityOperationResult = await _usersService.DeleteUser(user_id_to_delete, user_guid);
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
