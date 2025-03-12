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
    [ApiController]
    public class MailStatusesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MailStatusesController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IMail_statusesService _mail_statusesService;
        public MailStatusesController(ILogger<MailStatusesController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IMail_statusesService mail_statusesService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _mail_statusesService = mail_statusesService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<mail_statuses> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] mail_statuses mail_status, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<mail_statuses>> entityOperationResults = new List<EntityOperationResult<mail_statuses>>();
                var entityOperationResult = await _mail_statusesService.Create(mail_status, user_guid);
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
                    var mail_statuses = unitOfWork.mail_statuses.GetList();
                    return Ok(mail_statuses);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetMailStatuses");
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
                    var mail_status = unitOfWork.mail_statuses.Get(id);
                    return Ok(mail_status);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetMail_status");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] mail_statuses mail_status, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<mail_statuses>> entityOperationResults = new List<EntityOperationResult<mail_statuses>>();
                var entityOperationResult = await _mail_statusesService.Update(mail_status, user_guid);
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
                List<EntityOperationResult<mail_statuses>> entityOperationResults = new List<EntityOperationResult<mail_statuses>>();
                var entityOperationResult = await _mail_statusesService.Delete(id, user_guid);
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
