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
    public class TransferOrdersController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<TransferOrdersController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly ITransfer_OrdersService _transfer_OrderService;

        public TransferOrdersController(ILogger<TransferOrdersController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, ITransfer_OrdersService transfer_OrderService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _transfer_OrderService = transfer_OrderService;
        }


        private async Task WriteToLogEntityOperationResult(EntityOperationResult<transfer_orders> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] transfer_orders transfer_order, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<transfer_orders>> entityOperationResults = new List<EntityOperationResult<transfer_orders>>();
                var entityOperationResult = await _transfer_OrderService.Create(transfer_order, user_guid);
                entityOperationResults.Add(entityOperationResult);
                WriteToLogEntityOperationResult(entityOperationResult, "Create");
                return Ok(entityOperationResults);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Create");
                return Ok(ex.ToString());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Gets()
        {
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var transfer_orders = unitOfWork.transfer_orders.GetList();
                    return Ok(transfer_orders);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GeTransferOrders");
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
                    var transfer_order = unitOfWork.transfer_orders.Get(id);
                    return Ok(transfer_order);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GeTransferOrder");
                return Ok(ex.ToString());
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] transfer_orders transfer_order, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<transfer_orders>> entityOperationResults = new List<EntityOperationResult<transfer_orders>>();
                var entityOperationResult = await _transfer_OrderService.Update(transfer_order, user_guid);
                entityOperationResults.Add(entityOperationResult);
                WriteToLogEntityOperationResult(entityOperationResult, "Update");
                return Ok(entityOperationResults);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Update");
                return Ok(ex.ToString());
            }
        }

        [HttpPost]
        public async Task<ActionResult> Delete([FromBody] int? id, Guid user_guid)
        {
            try
            {
                if (id == null) return BadRequest();
                List<EntityOperationResult<transfer_orders>> entityOperationResults = new List<EntityOperationResult<transfer_orders>>();
                var entityOperationResult = await _transfer_OrderService.Delete(id, user_guid);
                entityOperationResults.Add(entityOperationResult);
                WriteToLogEntityOperationResult(entityOperationResult, "Delete");
                return Ok(entityOperationResults);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Delete");
                return Ok(ex.ToString());
            }
        }
    }
}
