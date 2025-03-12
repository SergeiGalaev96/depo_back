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
    public class PartnerTypesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PartnerTypesController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IPartner_TypesService _partner_typesService;

        public PartnerTypesController(ILogger<PartnerTypesController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IPartner_TypesService partner_typesService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _partner_typesService = partner_typesService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<partner_types> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] partner_types partner_type, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<partner_types>> entityOperationResults = new List<EntityOperationResult<partner_types>>();
                var entityOperationResult = await _partner_typesService.CreatePartnerType(partner_type, user_guid);
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
                    var partner_types = unitOfWork.partner_types.GetList();
                    return Ok(partner_types);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetPartnerTypes");
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
                    var partner_types = unitOfWork.partner_types.Get(id);
                    return Ok(partner_types);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetPartnerType");
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] partner_types partner_type, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<partner_types>> entityOperationResults = new List<EntityOperationResult<partner_types>>();
                var entityOperationResult = await _partner_typesService.CreatePartnerType(partner_type, user_guid);
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
                List<EntityOperationResult<partner_types>> entityOperationResults = new List<EntityOperationResult<partner_types>>();
                var entityOperationResult = await _partner_typesService.DeletePartnerType(id, user_guid);
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

