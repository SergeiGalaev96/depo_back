using Depository.Core.Models;
using Depository.Core;
using Depository.DAL.DbContext;
using Depository.DAL;
using Depository.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;

namespace Depository.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    public class VatsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<VatsController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IVatsService _vatsService;
        public VatsController(ILogger<VatsController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IVatsService vatsService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _vatsService = vatsService;
        }


        private async Task WriteToLogEntityOperationResult(EntityOperationResult<vats> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] vats vat, Guid user_guid)
        {
            try
            {

                List<EntityOperationResult<vats>> entityOperationResults = new List<EntityOperationResult<vats>>();
                var entityOperationResult = await _vatsService.Create(vat, user_guid);
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
                    var vats = unitOfWork.vats.GetList();
                    return Ok(vats);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "gets");
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
                    var vat = unitOfWork.vats.Get(id);
                    return Ok(vat);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "get");
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] vats vat, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<vats>> entityOperationResults = new List<EntityOperationResult<vats>>();
                var entityOperationResult = await _vatsService.Update(vat, user_guid);
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
                List<EntityOperationResult<vats>> entityOperationResults = new List<EntityOperationResult<vats>>();
                var entityOperationResult = await _vatsService.Delete(id, user_guid);
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
