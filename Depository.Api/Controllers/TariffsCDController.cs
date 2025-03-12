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
    public class TariffsCDController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TariffsCDController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly ITariffs_cdService _tariffs_cdService;

        public TariffsCDController(ILogger<TariffsCDController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, ITariffs_cdService tariffs_cdService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _tariffs_cdService = tariffs_cdService;
        }


        private async Task WriteToLogEntityOperationResult(EntityOperationResult<tariffs_cd> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] tariffs_cd tariff_cd, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<tariffs_cd>> entityOperationResults = new List<EntityOperationResult<tariffs_cd>>();
                var entityOperationResult = await _tariffs_cdService.CreateTariff_cd(tariff_cd, user_guid);
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
                    var tariffs_cd = unitOfWork.tariffs_cd.GetList();
                    return Ok(tariffs_cd);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetsTariffsCD");
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
                    var tariff_cd = unitOfWork.tariffs_cd.Get(id);
                    return Ok(tariff_cd);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetTariffCD");
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] tariffs_cd tariff_cd, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<tariffs_cd>> entityOperationResults = new List<EntityOperationResult<tariffs_cd>>();
                var entityOperationResult = await _tariffs_cdService.UpdateTariff_cd(tariff_cd, user_guid);
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
                List<EntityOperationResult<tariffs_cd>> entityOperationResults = new List<EntityOperationResult<tariffs_cd>>();
                var entityOperationResult = await _tariffs_cdService.DeleteTariff_cd(id, user_guid);
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
