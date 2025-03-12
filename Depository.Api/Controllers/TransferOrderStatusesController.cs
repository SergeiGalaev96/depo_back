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
    public class TransferOrderStatusesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TransferOrderStatusesController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly ITransfer_order_statusesService _transfer_order_statusesService;

        public TransferOrderStatusesController(ILogger<TransferOrderStatusesController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, ITransfer_order_statusesService transfer_order_statusesService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _transfer_order_statusesService = transfer_order_statusesService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<transfer_order_statuses> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] transfer_order_statuses transfer_order_status, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<transfer_order_statuses>> entityOperationResults = new List<EntityOperationResult<transfer_order_statuses>>();
                var entityOperationResult = await _transfer_order_statusesService.Create(transfer_order_status, user_guid);
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
                    var transfer_order_statuses = unitOfWork.transfer_order_statuses.GetList();
                    return Ok(transfer_order_statuses);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetTransfer_order_statuses");
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
                    var transfer_order_status = unitOfWork.transfer_order_statuses.Get(id);
                    return Ok(transfer_order_status);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetTransfer_order_status");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] transfer_order_statuses transfer_order_status, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<transfer_order_statuses>> entityOperationResults = new List<EntityOperationResult<transfer_order_statuses>>();
                var entityOperationResult = await _transfer_order_statusesService.Update(transfer_order_status, user_guid);
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
                List<EntityOperationResult<transfer_order_statuses>> entityOperationResults = new List<EntityOperationResult<transfer_order_statuses>>();
                var entityOperationResult = await _transfer_order_statusesService.Delete(id, user_guid);
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
