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
  
    public class DepositorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DepositorsController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IDepositorsService _depositorsService;
        public DepositorsController(ILogger<DepositorsController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IDepositorsService depositorsService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _depositorsService = depositorsService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<depositors> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] depositors depositor,  Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<depositors>> entityOperationResults = new List<EntityOperationResult<depositors>>();
                var entityOperationResult = await _depositorsService.CreateDepositor(depositor, user_guid);
                entityOperationResults.Add(entityOperationResult);
                WriteToLogEntityOperationResult(entityOperationResult, "Create");
                return Ok(entityOperationResults);
            }
            catch(Exception ex)
            {
                WriteToLogException(ex, "Create");
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Gets()
        {
            List<depositorsDTO> depositorsDTOList = new List<depositorsDTO>();
            try
            {
               
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var currencies = unitOfWork.currencies.GetList();
                    var cities = unitOfWork.cities.GetList();
                    var regions = unitOfWork.regions.GetList();
                    var countries = unitOfWork.countries.GetList();
                    var banks = unitOfWork.banks.GetList();
                    var depositors = unitOfWork.depositors.GetList();
                    foreach(var depositor in depositors)
                    {
                        depositorsDTO depositorsDTO = new depositorsDTO();
                        depositorsDTO.name = depositor.partners.name;
                        depositorsDTO.id = depositor.id;
                        depositorsDTO.partner = depositor.partners.id;
                        depositorsDTO.address = depositor.partners.address != null ? depositor.partners.address : "";
                        depositorsDTO.bank_account = depositor.partners.bank_account != null ? depositor.partners.bank_account : "";
                        depositorsDTO.gov_reg = depositor.partners.gov_reg != null ? depositor.partners.gov_reg : "";
                        depositorsDTO.capital = depositor.partners.capital != null ? depositor.partners.capital : null;
                        depositorsDTO.currency = depositor.partners.currency != null ? currencies.Where(x => x.id == depositor.partners.currency).FirstOrDefault().name : "";
                        depositorsDTO.city = depositor.partners.city != null ? depositor.partners.city : null;
                        depositorsDTO.region = depositor.partners.region != null ? depositor.partners.region : null;
                        depositorsDTO.country = depositor.partners.country != null ? depositor.partners.country : null;
                        depositorsDTO.post_address = depositor.partners.post_address != null ? depositor.partners.post_address : "";
                        depositorsDTO.phone = depositor.partners.phone != null ? depositor.partners.phone : "";
                        depositorsDTO.fax = depositor.partners.fax != null ? depositor.partners.fax : "";
                        depositorsDTO.email = depositor.partners.email != null ? depositor.partners.email : "";
                        depositorsDTO.bank = depositor.partners.bank != null ? banks.Where(x => x.id == depositor.partners.bank).FirstOrDefault().name : null;
                        depositorsDTO.inn = depositor.partners.inn != null ? depositor.partners.inn : "";
                        depositorsDTO.rni = depositor.partners.rni != null ? depositor.partners.rni : "";
                        depositorsDTO.cliring_account = depositor.partners.cliring_account != null ? depositor.partners.cliring_account : "";
                        depositorsDTO.storage_account = depositor.partners.storage_account != null ? depositor.partners.storage_account : "";
                        depositorsDTO.cliring_bank = depositor.partners.cliring_bank != null ? banks.Where(x => x.id == depositor.partners.cliring_bank).FirstOrDefault().name : null;
                        depositorsDTO.storage_bank = depositor.partners.storage_bank != null ? banks.Where(x => x.id == depositor.partners.storage_bank).FirstOrDefault().name : null;
                        depositorsDTO.security_reg = depositor.partners.security_reg != null ? depositor.partners.security_reg : "";
                       // depositorsDTO.vat = depositor.vat;
                       // depositorsDTO.sales_tax = depositor.sales_tax;
                        depositorsDTO.npf=depositor.npf;
                        depositorsDTO.clr_service = depositor.clr_service;
                        depositorsDTOList.Add(depositorsDTO);
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

        [HttpGet]
        public async Task<IActionResult> Get(int? id)
        {
            
            try
            {
                if (id == null) return BadRequest();
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var depositor = unitOfWork.depositors.Get(id);
                    return Ok(depositor);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetDepositor");
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] depositors depositor,  Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<depositors>> entityOperationResults = new List<EntityOperationResult<depositors>>();
                var entityOperationResult = await _depositorsService.UpdateDepositor(depositor, user_guid);
                entityOperationResults.Add(entityOperationResult);
                WriteToLogEntityOperationResult(entityOperationResult, "Update");
                return Ok(entityOperationResults);
            }
            catch(Exception ex)
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
                    if (id == null) return BadRequest();
                    List<EntityOperationResult<depositors>> entityOperationResults = new List<EntityOperationResult<depositors>>();
                    var entityOperationResult = await _depositorsService.DeleteDepositor(id, user_guid);
                    entityOperationResults.Add(entityOperationResult);
                    WriteToLogEntityOperationResult(entityOperationResult, "Delete");
                    return Ok(entityOperationResults);
                }
            catch(Exception ex)
            {

                WriteToLogException(ex, "Delete");
                return BadRequest();
            }
        }
    }
}
