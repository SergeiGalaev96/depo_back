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
    public class SalesTaxesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SalesTaxesController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly ISalesTaxesService _salesTaxesService;
        public SalesTaxesController(ILogger<SalesTaxesController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, ISalesTaxesService salesTaxesService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _salesTaxesService = salesTaxesService;
        }


        private async Task WriteToLogEntityOperationResult(EntityOperationResult<sales_taxes> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] sales_taxes sales_tax, Guid user_guid)
        {
            try
            {

                List<EntityOperationResult<sales_taxes>> entityOperationResults = new List<EntityOperationResult<sales_taxes>>();
                var entityOperationResult = await _salesTaxesService.Create(sales_tax, user_guid);
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
                    var user_roles = unitOfWork.sales_taxes.GetList();
                    return Ok(user_roles);
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
                    var user_role = unitOfWork.vats.Get(id);
                    return Ok(user_role);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "get");
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] sales_taxes sales_tax, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<sales_taxes>> entityOperationResults = new List<EntityOperationResult<sales_taxes>>();
                var entityOperationResult = await _salesTaxesService.Update(sales_tax, user_guid);
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
                List<EntityOperationResult<sales_taxes>> entityOperationResults = new List<EntityOperationResult<sales_taxes>>();
                var entityOperationResult = await _salesTaxesService.Delete(id, user_guid);
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
