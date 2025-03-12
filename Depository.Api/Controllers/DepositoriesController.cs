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
    public class DepositoriesController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<DepositoriesController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IDepositoriesService _depositariesService;
        public DepositoriesController(ILogger<DepositoriesController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IDepositoriesService depositariesService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _depositariesService = depositariesService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<depositories> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] depositories depositary, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<depositories>> entityOperationResults = new List<EntityOperationResult<depositories>>();
                var entityOperationResult = await _depositariesService.CreateDepositary(depositary, user_guid);
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
        public async Task<IActionResult> Get(int? id)
        {
            try
            {
                if (id == null) return BadRequest();
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var depository = unitOfWork.depositories.Get(id);
                    return Ok(depository);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetDepositary");
                return Ok(ex.ToString());
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] depositories depositary, Guid user_guid)
        {
            try
            { 
                List<EntityOperationResult<depositories>> entityOperationResults = new List<EntityOperationResult<depositories>>();
                var entityOperationResult = await _depositariesService.UpdateDepositary(depositary, user_guid);
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
                if (id == null)  return BadRequest();
                List<EntityOperationResult<depositories>> entityOperationResults = new List<EntityOperationResult<depositories>>();
                var entityOperationResult = await _depositariesService.DeleteDepositary(id, user_guid);
                entityOperationResults.Add(entityOperationResult);
                WriteToLogEntityOperationResult(entityOperationResult, "Delete");
                return Ok(entityOperationResults);
            }
            catch(Exception ex)
            {
                WriteToLogException(ex, "Delete");
                return Ok(ex.ToString());
            }
        }


        [HttpGet]
        public async Task<IActionResult> Gets()
        {
            List<depositoriesDTO> depositorsDTOList = new List<depositoriesDTO>();
            try
            {

                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var currencies = unitOfWork.currencies.GetList();
                    var cities = unitOfWork.cities.GetList();
                    var regions = unitOfWork.regions.GetList();
                    var countries = unitOfWork.countries.GetList();
                    var banks = unitOfWork.banks.GetList();
                    var depositories = unitOfWork.depositories.GetList();
                    foreach (var depository in depositories)
                    {
                        depositoriesDTO depositoriesDTO = new depositoriesDTO();
                        depositoriesDTO.name = depository.partners.name;
                        depositoriesDTO.id = depository.id;
                        depositoriesDTO.partner = depository.partners.id;
                        depositoriesDTO.address = depository.partners.address != null ? depository.partners.address : "";
                        depositoriesDTO.bank_account = depository.partners.bank_account != null ? depository.partners.bank_account : "";
                        depositoriesDTO.gov_reg = depository.partners.gov_reg != null ? depository.partners.gov_reg : "";
                        depositoriesDTO.capital = depository.partners.capital != null ? depository.partners.capital : null;
                        depositoriesDTO.currency = depository.partners.currency != null ? currencies.Where(x => x.id == depository.partners.currency).FirstOrDefault().name : "";
                        depositoriesDTO.city = depository.partners.city != null ? depository.partners.city : null;
                        depositoriesDTO.region = depository.partners.region != null ? depository.partners.region : null;
                        depositoriesDTO.country = depository.partners.country != null ? depository.partners.country : null;
                        depositoriesDTO.post_address = depository.partners.post_address != null ? depository.partners.post_address : "";
                        depositoriesDTO.phone = depository.partners.phone != null ? depository.partners.phone : "";
                        depositoriesDTO.fax = depository.partners.fax != null ? depository.partners.fax : "";
                        depositoriesDTO.email = depository.partners.email != null ? depository.partners.email : "";
                        depositoriesDTO.bank = depository.partners.bank != null ? banks.Where(x => x.id == depository.partners.bank).FirstOrDefault().name : null;
                        depositoriesDTO.inn = depository.partners.inn != null ? depository.partners.inn : "";
                        depositoriesDTO.rni = depository.partners.rni != null ? depository.partners.rni : "";
                        depositoriesDTO.cliring_account = depository.partners.cliring_account != null ? depository.partners.cliring_account : "";
                        depositoriesDTO.storage_account = depository.partners.storage_account != null ? depository.partners.storage_account : "";
                        depositoriesDTO.cliring_bank = depository.partners.cliring_bank != null ? banks.Where(x => x.id == depository.partners.cliring_bank).FirstOrDefault().name : null;
                        depositoriesDTO.storage_bank = depository.partners.storage_bank != null ? banks.Where(x => x.id == depository.partners.storage_bank).FirstOrDefault().name : null;
                        depositoriesDTO.security_reg = depository.partners.security_reg != null ? depository.partners.security_reg : "";
                        depositorsDTOList.Add(depositoriesDTO);
                    }
                    return Ok(depositorsDTOList);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetDepositors");
                return BadRequest();
            }
        }

    }
}
