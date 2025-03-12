using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Depository.Core;
using Depository.Core.IRepositories;
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
    public class TariffsCorrDepositoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TariffsCorrDepositoryController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly ITariffs_Corr_DepositoryService _tariffs_corr_depositoryService;
        public TariffsCorrDepositoryController(ILogger<TariffsCorrDepositoryController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, ITariffs_Corr_DepositoryService tariffs_corr_depositoryService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _tariffs_corr_depositoryService = tariffs_corr_depositoryService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<tariffs_corr_depository> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] tariffs_corr_depository tariff_corr_depository, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<tariffs_corr_depository>> entityOperationResults = new List<EntityOperationResult<tariffs_corr_depository>>();
                var entityOperationResult = await _tariffs_corr_depositoryService.CreateTariff_Corr_Depository(tariff_corr_depository, user_guid);
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
                    var tariffs_corr_depository = unitOfWork.tariffs_corr_depository.GetList();
                    return Ok(tariffs_corr_depository);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetsTariffs_corr_depository");
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
                    var tariff_corr_depository = unitOfWork.tariffs_corr_depository.Get(id);
                    return Ok(tariff_corr_depository);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetTariff_corr_depository");
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] tariffs_corr_depository tariff_corr_depository, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<tariffs_corr_depository>> entityOperationResults = new List<EntityOperationResult<tariffs_corr_depository>>();
                var entityOperationResult = await _tariffs_corr_depositoryService.UpdateTariff_Corr_Depository(tariff_corr_depository, user_guid);
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
                List<EntityOperationResult<tariffs_corr_depository>> entityOperationResults = new List<EntityOperationResult<tariffs_corr_depository>>();
                var entityOperationResult = await _tariffs_corr_depositoryService.DeleteTariff_Corr_Depository(id, user_guid);
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
