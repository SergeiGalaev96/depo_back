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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Depository.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RegistrarsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RegistrarsController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IRegistrarsService _registrarsService;



        public RegistrarsController(ILogger<RegistrarsController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IRegistrarsService registrarsService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _registrarsService = registrarsService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<registrars> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] registrars registrar, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<registrars>> entityOperationResults = new List<EntityOperationResult<registrars>>();
                var entityOperationResult = await _registrarsService.CreateRegistrar(registrar, user_guid);
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
            List<registrarsDTO> registrarsDTOList = new List<registrarsDTO>();
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var currencies = unitOfWork.currencies.GetList();
                    var cities = unitOfWork.cities.GetList();
                    var regions = unitOfWork.regions.GetList();
                    var countries = unitOfWork.countries.GetList();
                    var banks = unitOfWork.banks.GetList();
                    var registrars = unitOfWork.registrars.GetList();
                    foreach(var registrar in registrars)
                    {
                        registrarsDTO registrarsDTO = new registrarsDTO();
                        registrarsDTO.name = registrar.partners.name;
                        registrarsDTO.id = registrar.id;
                        registrarsDTO.partner= registrar.partners.id;
                        registrarsDTO.address = registrar.partners.address != null ? registrar.partners.address : "";
                        registrarsDTO.bank_account = registrar.partners.bank_account != null ? registrar.partners.bank_account : "";
                        registrarsDTO.gov_reg = registrar.partners.gov_reg != null ? registrar.partners.gov_reg : "";
                        registrarsDTO.capital = registrar.partners.capital != null ? registrar.partners.capital : null;
                        registrarsDTO.currency = registrar.partners.currency != null ? currencies.Where(x => x.id == registrar.partners.currency).FirstOrDefault().name : "";
                        registrarsDTO.city = registrar.partners.city != null ? registrar.partners.city : null;
                       // registrarsDTO.region = registrar.partners.region != null ? registrar.partners.region : null;
                        registrarsDTO.country = registrar.partners.country != null ? registrar.partners.country : null;
                        registrarsDTO.post_address = registrar.partners.post_address != null ? registrar.partners.post_address : "";
                        registrarsDTO.phone = registrar.partners.phone != null ? registrar.partners.phone : "";
                        registrarsDTO.fax = registrar.partners.fax != null ? registrar.partners.fax : "";
                        registrarsDTO.email = registrar.partners.email != null ? registrar.partners.email : "";
                        var bank = banks.Where(x => x.id == registrar.partners.bank).FirstOrDefault();
                        var bank_name = bank != null ? bank.name : "";
                        registrarsDTO.bank = bank_name;
                        registrarsDTO.inn = registrar.partners.inn != null ? registrar.partners.inn : "";
                        registrarsDTO.rni = registrar.partners.rni != null ? registrar.partners.rni : "";
                        registrarsDTO.cliring_account = registrar.partners.cliring_account != null ? registrar.partners.cliring_account : "";
                        registrarsDTO.storage_account = registrar.partners.storage_account != null ? registrar.partners.storage_account : "";
                        registrarsDTO.cliring_bank = registrar.partners.cliring_bank != null ? banks.Where(x => x.id == registrar.partners.cliring_bank).FirstOrDefault().name : null;
                        registrarsDTO.storage_bank = registrar.partners.storage_bank != null ? banks.Where(x => x.id == registrar.partners.storage_bank).FirstOrDefault().name : null;
                        registrarsDTO.security_reg = registrar.partners.security_reg != null ? registrar.partners.security_reg : "";
                        registrarsDTOList.Add(new registrarsDTO { id = registrar.id, name = registrar.partners.name, partner = registrar.partners.id });
                    }
                    return Ok(registrarsDTOList);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetRegistrars");
                return Ok(JsonConvert.SerializeObject(ex));
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
                    var registrar = unitOfWork.registrars.Get(id);
                    return Ok(registrar);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetRegistrar");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }


        [HttpPost]
        public async Task<ActionResult> Update([FromBody] registrars registrar, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<registrars>> entityOperationResults = new List<EntityOperationResult<registrars>>();
                var entityOperationResult = await _registrarsService.UpdateRegistrar(registrar, user_guid);
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
        public async Task<ActionResult> Delete([FromBody] int? id,  Guid user_guid)
        {

            try
            {
                if (id == null) return BadRequest();
                List<EntityOperationResult<registrars>> entityOperationResults = new List<EntityOperationResult<registrars>>();
                var entityOperationResult = await _registrarsService.DeleteRegistrar(id, user_guid);
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
