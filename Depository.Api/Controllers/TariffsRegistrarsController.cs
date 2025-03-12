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
    public class TariffsRegistrarsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TariffsRegistrarsController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly ITariffs_RegistrarsService _tariffs_registrarsService;

        public TariffsRegistrarsController(ILogger<TariffsRegistrarsController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, ITariffs_RegistrarsService tariffs_registrarsService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _tariffs_registrarsService = tariffs_registrarsService;
        }


        private async Task WriteToLogEntityOperationResult(EntityOperationResult<tariffs_registrars> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] tariffs_registrars tariff_registrars, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<tariffs_registrars>> entityOperationResults = new List<EntityOperationResult<tariffs_registrars>>();
                var entityOperationResult = await _tariffs_registrarsService.CreateTariff_Registrars(tariff_registrars, user_guid);
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
                    var tariffs_registrars = unitOfWork.tariffs_registrars.GetList();
                    return Ok(tariffs_registrars);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetsTariffs_registrars");
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
                    var tariff_registrars = unitOfWork.tariffs_registrars.Get(id);
                    return Ok(tariff_registrars);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetTariff_registrars");
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] tariffs_registrars tariff_registrars, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<tariffs_registrars>> entityOperationResults = new List<EntityOperationResult<tariffs_registrars>>();
                var entityOperationResult = await _tariffs_registrarsService.UpdateTariff_Registrars(tariff_registrars, user_guid);
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
                List<EntityOperationResult<tariffs_registrars>> entityOperationResults = new List<EntityOperationResult<tariffs_registrars>>();
                var entityOperationResult = await _tariffs_registrarsService.DeleteTariff_Registrars(id, user_guid);
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
