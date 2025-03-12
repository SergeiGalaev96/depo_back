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
using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace Depository.Api.Controllers
{
    [Route("api/[controller]/[action]")]

    public class SecuritiesController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<SecuritiesController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly ISecuritiesService _securitiesService;
        private readonly IStock_SecurityService _stock_SecurityService;
        private readonly IGovSecurityPaymentsService _govSecurityPaymentsService;
        public SecuritiesController(ILogger<SecuritiesController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, ISecuritiesService securitiesService, IStock_SecurityService stock_SecurityService, IGovSecurityPaymentsService govSecurityPaymentsService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _securitiesService = securitiesService;
            _stock_SecurityService = stock_SecurityService;
            _govSecurityPaymentsService = govSecurityPaymentsService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<securities> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] securities security, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<securities>> entityOperationResults = new List<EntityOperationResult<securities>>();
                var entityOperationResult = await _securitiesService.CreateSecurity(security, user_guid);
                if (entityOperationResult.IsSuccess && security.is_gov_security.Equals(true))
                {
                    if (security.reg_date != null && security.reg_date > DateTime.MinValue && security.end_date != null && security.end_date > DateTime.MinValue)
                    {
                        CreateGovSecurityPayments(security, user_guid);
                    }
                }
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


        private void CreateGovSecurityPayments(securities security, Guid user_guid)
        {
            DateTime StartDate = security.reg_date.Value;
            DateTime EndDate = security.end_date.Value;
            int MonthInterval = 6;
            while (StartDate.AddMonths(MonthInterval) <= EndDate)
            {
                StartDate = StartDate.AddMonths(MonthInterval);
                gov_securities_payments gov_securities_payment = new gov_securities_payments
                {
                    date_of_payment = StartDate,
                    security = security.id,
                    percent = security.payment_percent / 2,
                    deleted = false
                };

                var entityOperationResult = _govSecurityPaymentsService.Create(gov_securities_payment, user_guid);

            }
        }



        [HttpGet]
        public async Task<IActionResult> Gets()
        {
            List<securitiesDTO> securitiesDTOList = new List<securitiesDTO>();
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var partners = unitOfWork.partners.GetAll();
                    var securities = unitOfWork.securities.GetList();
                    var issuers = unitOfWork.issuers.GetList();
                    var security_types = unitOfWork.security_types.GetList();
                    var registrars = unitOfWork.registrars.GetList();
                    foreach (var security in securities)
                    {
                        securitiesDTO securitiesDTO = new securitiesDTO();
                        securitiesDTO.id = security.id;
                        securitiesDTO.code = security.code;
                        securitiesDTO.issuer = security.issuer;
                        securitiesDTO.nominal = security.nominal;
                        securitiesDTO.currency = security.currency;
                        securitiesDTO.quantity = security.quantity;
                        securitiesDTO.reg_date = security.reg_date;
                        securitiesDTO.end_date = security.end_date;
                        securitiesDTO.dividends = security.dividends;
                        var registrar = registrars.Where(x => x.id == security.registrar).FirstOrDefault();
                        securitiesDTO.registrar_name = registrar != null ? registrar.name : "";
                        securitiesDTO.registrar = security.registrar;
                        securitiesDTO.isin = security.isin;
                        var issuer = issuers.Where(x => x.id == security.issuer).FirstOrDefault();
                        securitiesDTO.issuer_name = issuer != null ? issuer.name : "";
                        securitiesDTO.security_type = security.security_type;
                        securitiesDTO.security_type_name = security.security_types.name;
                        securitiesDTO.created_at = security.created_at;
                        securitiesDTO.updated_at = security.updated_at;
                        securitiesDTO.is_gov_security = security.is_gov_security;
                        securitiesDTO.gov_part = security.gov_part;
                        securitiesDTO.blocked = security.blocked;
                        securitiesDTO.block_date = security.block_date;
                        securitiesDTO.send_to_trades = security.send_to_trades;
                        securitiesDTO.location = security.location;
                        securitiesDTOList.Add(securitiesDTO);

                    }
                    return Ok(securitiesDTOList);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetSecurities");
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
                    var security = unitOfWork.securities.Get(id);
                    return Ok(security);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetSecurity");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }

        [HttpGet]
        public async Task<IActionResult> BlockBySecurity(int security_id, Guid user_guid)
        {
            List<EntityOperationResult<stock_security>> entityOperationResultList = new List<EntityOperationResult<stock_security>>();
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var security = unitOfWork.securities.Get(security_id);
                    if (security != null)
                    {
                        security.blocked = true;
                        security.block_date = DateTime.Now;
                        await _securitiesService.UpdateSecurity(security, user_guid);
                    }
                    var stock_Securities = unitOfWork.stock_security.GetListBySecurity(security_id);
                    foreach (var stock_security in stock_Securities)
                    {
                        var quantity = stock_security.quantity;
                        stock_security.quantity_stop = stock_security.quantity_stop + quantity;
                        stock_security.quantity = 0;
                        var entity = await _stock_SecurityService.Update(stock_security, user_guid);
                        entityOperationResultList.Add(entity);
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
        public async Task<IActionResult> UnBlockBySecurity(int security_id, Guid user_guid)
        {
            List<EntityOperationResult<stock_security>> entityOperationResultList = new List<EntityOperationResult<stock_security>>();
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var security = unitOfWork.securities.Get(security_id);
                    if (security != null)
                    {
                        security.blocked = false;
                        security.block_date = null;
                        await _securitiesService.UpdateSecurity(security, user_guid);
                    }
                    var stock_Securities = unitOfWork.stock_security.GetListBySecurity(security_id);
                    foreach (var stock_security in stock_Securities)
                    {
                        var quantity_stop = stock_security.quantity_stop;
                        stock_security.quantity_stop = 0;
                        stock_security.quantity = stock_security.quantity + quantity_stop;
                        var entity = await _stock_SecurityService.Update(stock_security, user_guid);
                        entityOperationResultList.Add(entity);
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

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] securities security, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<securities>> entityOperationResults = new List<EntityOperationResult<securities>>();
                var entityOperationResult = await _securitiesService.UpdateSecurity(security, user_guid);
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
        public async Task<ActionResult> Delete([FromBody] int? id, Guid user_guid)
        {
            try
            {
                if (id == null) return BadRequest();
                List<EntityOperationResult<securities>> entityOperationResults = new List<EntityOperationResult<securities>>();
                var entityOperationResult = await _securitiesService.DeleteSecurity(id, user_guid);
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
