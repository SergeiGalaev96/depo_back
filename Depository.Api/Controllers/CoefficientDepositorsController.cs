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
    public class CoefficientDepositorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CoefficientDepositorsController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly ICoefficient_DepositorsService _coefficient_depositorsService;

        public CoefficientDepositorsController(ILogger<CoefficientDepositorsController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, ICoefficient_DepositorsService coefficient_depositorsService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _coefficient_depositorsService = coefficient_depositorsService;
        }


        private async Task WriteToLogEntityOperationResult(EntityOperationResult<coefficient_depositors> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] coefficient_depositors coefficient_depositor, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<coefficient_depositors>> entityOperationResults = new List<EntityOperationResult<coefficient_depositors>>();
                var entityOperationResult = await _coefficient_depositorsService.CreateCoefficient_Depositor(coefficient_depositor, user_guid);
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
                    var coefficient_depositors = unitOfWork.coefficient_depositors.GetList();
                    return Ok(coefficient_depositors);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetCoefficient_depositors");
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
                    var coefficient_depositor = unitOfWork.coefficient_depositors.Get(id);
                    return Ok(coefficient_depositor);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetCoefficient_depositor");
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] coefficient_depositors coefficient_depositor, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<coefficient_depositors>> entityOperationResults = new List<EntityOperationResult<coefficient_depositors>>();
                var entityOperationResult = await _coefficient_depositorsService.UpdateCoefficient_Depositor(coefficient_depositor, user_guid);
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
                List<EntityOperationResult<coefficient_depositors>> entityOperationResults = new List<EntityOperationResult<coefficient_depositors>>();
                var entityOperationResult = await _coefficient_depositorsService.DeleteCoefficient_Depositor(id, user_guid);
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

   

