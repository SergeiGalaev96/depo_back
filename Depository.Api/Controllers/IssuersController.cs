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
  
    public class IssuersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IssuersController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IIssuersService _issuersService;
        private readonly IStock_SecurityService _stock_SecurityService;
        public IssuersController(ILogger<IssuersController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IIssuersService issuersService, IStock_SecurityService stock_SecurityService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _issuersService = issuersService;
            _stock_SecurityService = stock_SecurityService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<issuers> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] issuers issuer,  Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<issuers>> entityOperationResults = new List<EntityOperationResult<issuers>>();
                var entityOperationResult = await _issuersService.CreateIssuer(issuer, user_guid);
                entityOperationResults.Add(entityOperationResult);
                WriteToLogEntityOperationResult(entityOperationResult, "Create");
                return Ok(entityOperationResults);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Create");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }

        [HttpGet]
        public async Task<IActionResult> BlockByIssuer(int issuer_id, Guid user_guid)
        {
            List<EntityOperationResult<stock_security>> entityOperationResultList = new List<EntityOperationResult<stock_security>>();
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var issuer = unitOfWork.issuers.Get(issuer_id);
                    if (issuer!=null)
                    {
                        issuer.blocked = true;
                        issuer.block_date = DateTime.Now;
                        await _issuersService.UpdateIssuer(issuer, user_guid);
                    }
                    var securities = unitOfWork.securities.GetByIssuer(issuer_id);
                    foreach (var security in securities)
                    {
                        var stock_Securities = unitOfWork.stock_security.GetListBySecurity(security.id);
                        foreach (var stock_security in stock_Securities)
                        {
                            var quantity = stock_security.quantity;
                            stock_security.quantity_stop = quantity;
                            stock_security.quantity = 0;
                            var entity = await _stock_SecurityService.Update(stock_security, user_guid);
                            entityOperationResultList.Add(entity);
                        }
                    }
                }
            }




            catch (Exception ex)
            {
                entityOperationResultList.Add(EntityOperationResult<stock_security>
                            .Failure()
                            .AddError(JsonConvert.SerializeObject(ex)));

            }
            return Ok(entityOperationResultList);
        }

        [HttpGet]
        public async Task<IActionResult> UnBlockByIssuer(int issuer_id, Guid user_guid)
        {
            List<EntityOperationResult<stock_security>> entityOperationResultList = new List<EntityOperationResult<stock_security>>();
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var issuer = unitOfWork.issuers.Get(issuer_id);
                    if (issuer != null)
                    {
                        issuer.blocked = false;
                        issuer.block_date = null;
                        await _issuersService.UpdateIssuer(issuer, user_guid);
                    }
                    var securities = unitOfWork.securities.GetByIssuer(issuer_id);
                    foreach (var security in securities)
                    {
                        var stock_Securities = unitOfWork.stock_security.GetListBySecurity(security.id);
                        foreach (var stock_security in stock_Securities)
                        {
                            var quantity_stop = stock_security.quantity_stop;
                            stock_security.quantity_stop = 0;
                            stock_security.quantity = quantity_stop;
                            var entity = await _stock_SecurityService.Update(stock_security, user_guid);
                            entityOperationResultList.Add(entity);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                entityOperationResultList.Add(EntityOperationResult<stock_security>
                            .Failure()
                            .AddError(JsonConvert.SerializeObject(ex)));

            }
            return Ok(entityOperationResultList);
        }


        [HttpGet]
        public async Task<IActionResult> Gets()
        {
            List<issuersDTO> isssuersDTOList = new List<issuersDTO>();
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var currencies = unitOfWork.currencies.GetList();
                    var issuers = unitOfWork.issuers.GetList();
                    //var cities = unitOfWork.cities.GetList();
                    //var regions = unitOfWork.regions.GetList();
                    //var countries = unitOfWork.countries.GetList();
                    //var banks = unitOfWork.banks.GetList();
                    //foreach (var isssuer in issuers)
                    //{
                    //    issuersDTO issuersDTO = new issuersDTO();
                    //    issuersDTO.id = isssuer.id;
                    //    issuersDTO.name = isssuer.name;
                    //    issuersDTO.address = isssuer.address != null ? isssuer.address : "";
                    //    issuersDTO.bank_account = isssuer.account!=null ? isssuer.account : "";
                    //    issuersDTO.gov_reg = isssuer.gov_registration!=null ? isssuer.gov_registration: "";
                    //    issuersDTO.capital = isssuer.capital!=null ? isssuer.capital:null;
                    //    issuersDTO.city = isssuer.city != null ? isssuer.city  : null;
                    //    issuersDTO.post_address = isssuer.post_address;
                    //    issuersDTO.phone = isssuer.phone;
                    //    issuersDTO.fax = isssuer.fax;
                    //    issuersDTO.email = isssuer.email;
                    //    issuersDTO.b = isssuer.bank != null ? banks.Where(x => x.id == isssuer.bank).FirstOrDefault().name : null;
                    //    issuersDTO.inn = isssuer.inn!=null ? isssuer.inn:"";
                    //    issuersDTO.security_reg = isssuer.registration;
                    //    isssuersDTOList.Add(issuersDTO);
                    //}
                    return Ok(issuers);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetIssuers");
                return Ok(JsonConvert.SerializeObject(ex));
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
                    var issuer = unitOfWork.issuers.Get(id);
                    return Ok(issuer);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetIssuer");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] issuers issuer,  Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<issuers>> entityOperationResults = new List<EntityOperationResult<issuers>>();
                var entityOperationResult = await _issuersService.UpdateIssuer(issuer, user_guid);
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
                List<EntityOperationResult<issuers>> entityOperationResults = new List<EntityOperationResult<issuers>>();
                var entityOperationResult = await _issuersService.DeleteIssuer(id, user_guid);
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
