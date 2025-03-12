using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Depository.Core;
using Depository.Core.Models;
using Depository.Core.Models.DTO;
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
    public class MailsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MailsController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IMailsService _mailsService;
        private readonly IMail_distributionsService _mail_distributionsService;
        public MailsController(ILogger<MailsController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IMailsService mailsService, IMail_distributionsService mail_distributionsService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _mailsService = mailsService;
            _mail_distributionsService = mail_distributionsService;

        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<mails> entityOperationResult, string action)
        {
            if (entityOperationResult.IsSuccess)
                _logger.LogInformation(action + " : " + JsonConvert.SerializeObject(entityOperationResult));
            else _logger.LogError(action + " : " + JsonConvert.SerializeObject(entityOperationResult));
        }
        private async Task WriteToLogEntityOperationResult(List<EntityOperationResult<mail_distributions>> entityOperationResults, string action)
        {
                _logger.LogInformation(action + " : " + JsonConvert.SerializeObject(entityOperationResults));
        }

        private async Task WriteToLogException(Exception exception, string action)
        {
            _logger.LogError(action + " : " + exception.ToString());
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] mail_and_distributionsDTO mail_and_distributionsDTO, Guid user_guid)
        {
            try
            {
                var mail = mail_and_distributionsDTO.mails;
                var mail_distributions = mail_and_distributionsDTO.mail_distributions;
                List<EntityOperationResult<mails>> mailEntityOperationResults = new List<EntityOperationResult<mails>>();
                List<EntityOperationResult<mail_distributions>> mail_distributionsEntityOperationResults = new List<EntityOperationResult<mail_distributions>>();
                var mailEntityOperationResult = await _mailsService.Create(mail, user_guid);
                if (mailEntityOperationResult.IsSuccess)
                { 
                    var mail_id = mailEntityOperationResult.Entity.id;
                    foreach(var mail_distribution in mail_distributions)
                    {
                        mail_distribution.mail_id = mail_id;
                        var mail_distributionsEntityOperationResult=await _mail_distributionsService.Create(mail_distribution, user_guid);
                        mail_distributionsEntityOperationResults.Add(mail_distributionsEntityOperationResult);
                    }
                    WriteToLogEntityOperationResult(mail_distributionsEntityOperationResults, "Create");
                }
                mailEntityOperationResults.Add(mailEntityOperationResult);
                WriteToLogEntityOperationResult(mailEntityOperationResult, "Create");
                return Ok(mailEntityOperationResults);
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
                    var mails = unitOfWork.mails.GetList();
                    return Ok(mails);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetMails");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBySender(int user_id_sender)
        {
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var mails = unitOfWork.mails.GetBySender(user_id_sender);
                    return Ok(mails);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetBySender");
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
                    var account = unitOfWork.mails.Get(id);
                    return Ok(account);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Get");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] mails mail, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<mails>> entityOperationResults = new List<EntityOperationResult<mails>>();
                var entityOperationResult = await _mailsService.Update(mail, user_guid);
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
        public async Task<ActionResult> UpdateList([FromBody] List<mails> mails, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<mails>> entityOperationResults = new List<EntityOperationResult<mails>>();
                foreach(var mail in mails)
                { 
                    var entityOperationResult = await _mailsService.Update(mail, user_guid);
                    entityOperationResults.Add(entityOperationResult);
                    WriteToLogEntityOperationResult(entityOperationResult, "Update");
                }
               
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
                List<EntityOperationResult<mails>> entityOperationResults = new List<EntityOperationResult<mails>>();
                var entityOperationResult = await _mailsService.Delete(id, user_guid);
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
