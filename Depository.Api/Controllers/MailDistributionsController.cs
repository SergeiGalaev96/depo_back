using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
    public class MailDistributionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MailDistributionsController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IMail_distributionsService _mail_DistributionsService;
        public MailDistributionsController(ILogger<MailDistributionsController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IMail_distributionsService mail_DistributionsService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _mail_DistributionsService = mail_DistributionsService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<mail_distributions> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] mail_distributions mail_distribution, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<mail_distributions>> entityOperationResults = new List<EntityOperationResult<mail_distributions>>();
                var entityOperationResult = await _mail_DistributionsService.Create(mail_distribution, user_guid);
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
        public async Task<IActionResult> GetByRecipient(int user_id_recipient)
        {
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var mail_distributions = unitOfWork.mail_distributions.GetByRecipient(user_id_recipient);
                    var options = new JsonSerializerOptions
                    {
                        IgnoreNullValues = true,
                        WriteIndented = true
                    };
                    return Ok(mail_distributions);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetByRecipient");
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
                    var mail_distributions = unitOfWork.mail_distributions.GetBySender(user_id_sender);
                    return Ok(mail_distributions);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetBySender");
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
                    var mail_distributions = unitOfWork.mail_distributions.GetList();
                    return Ok(mail_distributions);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetMail_distributions");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadDistributions(int user_id_recipient)
        {
            if (user_id_recipient==null )
            {
                return Ok("user_id_recipient value is null");
            }
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var mail_distributions = unitOfWork.mail_distributions.GetUnreadDistributions(user_id_recipient);
                    return Ok(mail_distributions);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetUnreadDistributions");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCountUnreadDistributions(int user_id_recipient)
        {
            if (user_id_recipient == null)
            {
                return Ok("user_id_recipient value is null");
            }
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var count_mail_distributions = unitOfWork.mail_distributions.GetCountUnreadDistributions(user_id_recipient);
                    return Ok( new { value = count_mail_distributions});
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetCountUnreadDistributions");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTrashDistributions(int user_id_recipient)
        {
            if (user_id_recipient == null)
            {
                return Ok("user_id_recipient value is null");
            }
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var mail_distributions = unitOfWork.mail_distributions.GetTrashDistributions(user_id_recipient);
                    return Ok(mail_distributions);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetTrashDistributions");
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
                    var mail_distribution = unitOfWork.mail_distributions.Get(id);
                    return Ok(mail_distribution);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetMail_distribution");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] mail_distributions mail_distribution, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<mail_distributions>> entityOperationResults = new List<EntityOperationResult<mail_distributions>>();
                var entityOperationResult = await _mail_DistributionsService.Update(mail_distribution, user_guid);
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
        public async Task<ActionResult> UpdateList([FromBody] List<mail_distributions> mail_distributions, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<mail_distributions>> entityOperationResults = new List<EntityOperationResult<mail_distributions>>();
                foreach(var mail_distribution in mail_distributions)
                { 
                    var entityOperationResult = await _mail_DistributionsService.Update(mail_distribution, user_guid);
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
                List<EntityOperationResult<mail_distributions>> entityOperationResults = new List<EntityOperationResult<mail_distributions>>();
                var entityOperationResult = await _mail_DistributionsService.Delete(id, user_guid);
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
