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
    
    public class PartnerContactsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PartnerContactsController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IPartner_ContactsService _partner_contactsService;
        public PartnerContactsController(ILogger<PartnerContactsController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IPartner_ContactsService partner_contactsService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _partner_contactsService = partner_contactsService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<partner_contacts> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] partner_contacts partner_contact,  Guid user_guid)
        {
            try
            { 
            List<EntityOperationResult<partner_contacts>> entityOperationResults = new List<EntityOperationResult<partner_contacts>>();
            var entityOperationResult = await _partner_contactsService.CreatePartner_Contact(partner_contact, user_guid);
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
                    var partner_contacts = unitOfWork.partner_contacts.GetList();
                    return Ok(partner_contacts);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetPartnerContacts");
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
                    var partner_contact = unitOfWork.partner_contacts.Get(id);
                    return Ok(partner_contact);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetPartnerContact");
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] partner_contacts partner_contact,  Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<partner_contacts>> entityOperationResults = new List<EntityOperationResult<partner_contacts>>();
                var entityOperationResult = await _partner_contactsService.UpdatePartner_Contact(partner_contact, user_guid);
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
                List<EntityOperationResult<partner_contacts>> entityOperationResults = new List<EntityOperationResult<partner_contacts>>();
                var entityOperationResult = await _partner_contactsService.DeletePartner_Contact(id, user_guid);
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
