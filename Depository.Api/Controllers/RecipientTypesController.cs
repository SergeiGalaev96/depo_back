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
    public class RecipientTypesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RecipientTypesController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IRecipient_TypesService _recipient_TypesService;

        public RecipientTypesController(ILogger<RecipientTypesController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IRecipient_TypesService recipient_TypesService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _recipient_TypesService = recipient_TypesService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<recipient_types> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] recipient_types recipient_type, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<recipient_types>> entityOperationResults = new List<EntityOperationResult<recipient_types>>();
                var entityOperationResult = await _recipient_TypesService.CreateRecipient_Type(recipient_type, user_guid);
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
                    var recipient_types = unitOfWork.recipient_types.GetList();
                    return Ok(recipient_types);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Getrecipient_types");
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
                    var recipient_type = unitOfWork.recipient_types.Get(id);
                    return Ok(recipient_type);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Getrecipient_type");
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] recipient_types recipient_type, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<recipient_types>> entityOperationResults = new List<EntityOperationResult<recipient_types>>();
                var entityOperationResult = await _recipient_TypesService.UpdateRecipient_Type(recipient_type, user_guid);
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
                List<EntityOperationResult<recipient_types>> entityOperationResults = new List<EntityOperationResult<recipient_types>>();
                var entityOperationResult = await _recipient_TypesService.DeleteRecipient_Type(id, user_guid);
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
