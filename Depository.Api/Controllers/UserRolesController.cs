﻿using System;
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
    
    public class UserRolesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserRolesController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IUser_RolesService _user_RolesService;
        public UserRolesController(ILogger<UserRolesController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IUser_RolesService user_RolesService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _user_RolesService = user_RolesService;
        }


        private async Task WriteToLogEntityOperationResult(EntityOperationResult<user_roles> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] user_roles user_role,  Guid user_guid)
        {
            try
            {

            List<EntityOperationResult<user_roles>> entityOperationResults = new List<EntityOperationResult<user_roles>>();
            var entityOperationResult = await _user_RolesService.CreateUserRoles(user_role, user_guid);
            entityOperationResults.Add(entityOperationResult);
            WriteToLogEntityOperationResult(entityOperationResult, "Create");
            return Ok(entityOperationResults);

            }
            catch(Exception ex)
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
                    var user_roles =  unitOfWork.user_roles.GetList();
                    return Ok(user_roles);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetUserRoles");
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
                    var user_role = unitOfWork.user_roles.Get(id);
                    return Ok(user_role);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetUserRole");
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] user_roles user_role,  Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<user_roles>> entityOperationResults = new List<EntityOperationResult<user_roles>>();
                var entityOperationResult = await _user_RolesService.UpdateUserRoles(user_role, user_guid);
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
        public async Task<ActionResult> Delete([FromBody] int? id, [FromBody] Guid user_guid)
        {
            try
            {
                if (id == null) return BadRequest();
                List<EntityOperationResult<user_roles>> entityOperationResults = new List<EntityOperationResult<user_roles>>();
                var entityOperationResult = await _user_RolesService.DeleteUserRoles(id, user_guid);
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
