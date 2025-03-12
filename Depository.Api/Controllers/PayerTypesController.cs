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
    public class PayerTypesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PayerTypesController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IPayer_TypesService _payer_TypesService;

        public PayerTypesController(ILogger<PayerTypesController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IPayer_TypesService payer_TypesService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _payer_TypesService = payer_TypesService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<payer_types> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] payer_types payer_type, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<payer_types>> entityOperationResults = new List<EntityOperationResult<payer_types>>();
                var entityOperationResult = await _payer_TypesService.CreatePayer_Type(payer_type, user_guid);
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
                    var payer_types = unitOfWork.payer_types.GetList();
                    return Ok(payer_types);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Getpayer_types");
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
                    var payer_type = unitOfWork.payer_types.Get(id);
                    return Ok(payer_type);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Getrecipient_type");
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] payer_types payer_type, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<payer_types>> entityOperationResults = new List<EntityOperationResult<payer_types>>();
                var entityOperationResult = await _payer_TypesService.UpdatePayer_Type(payer_type, user_guid);
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
                List<EntityOperationResult<payer_types>> entityOperationResults = new List<EntityOperationResult<payer_types>>();
                var entityOperationResult = await _payer_TypesService.DeletePayer_Type(id, user_guid);
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
