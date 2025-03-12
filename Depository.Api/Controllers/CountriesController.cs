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
   
    public class CountriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CountriesController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly ICountriesService _countriesService;
        public CountriesController(ILogger<CountriesController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, ICountriesService countriesService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _countriesService = countriesService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<countries> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] countries country,  Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<countries>> entityOperationResults = new List<EntityOperationResult<countries>>();
                var entityOperationResult = await _countriesService.CreateCountry(country, user_guid);
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
                    var countries = unitOfWork.countries.GetList();
                    return Ok(countries);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetCountries");
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get(int? id)
        {
           
            try
            {
                if (id == null) return BadRequest();
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var country = unitOfWork.accounts.Get(id);
                    return Ok(country);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetCountry");
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] countries country,  Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<countries>> entityOperationResults = new List<EntityOperationResult<countries>>();
                var entityOperationResult = await _countriesService.UpdateCountry(country, user_guid);
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
        public async Task<ActionResult> Delete([FromBody] int? id,  Guid user_guid)
        {
            try 
            { 
                if (id == null)  return BadRequest();
                List<EntityOperationResult<countries>> entityOperationResults = new List<EntityOperationResult<countries>>();
                var entityOperationResult = await _countriesService.DeleteCountry(id, user_guid);
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
