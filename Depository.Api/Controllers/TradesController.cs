using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
    public class TradesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TradesController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly ITradesService _tradesService;
        public TradesController(ILogger<TradesController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, ITradesService tradesService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _tradesService = tradesService;
        }


        private async Task WriteToLogEntityOperationResult(EntityOperationResult<trades> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] trades trade, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<trades>> entityOperationResults = new List<EntityOperationResult<trades>>();
                var entityOperationResult = await _tradesService.CreateTrade(trade, user_guid);
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
        public async Task<IActionResult> Gets()
        {
            List<tradesDTO> tradesDTOList = new List<tradesDTO>();
            var config = new MapperConfiguration(cfg => cfg.CreateMap<trades, tradesDTO>());
            // Настройка AutoMapper
            var mapper = new Mapper(config);
            // сопоставление
         
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var partners = unitOfWork.partners.GetList();
                    var depositors = unitOfWork.depositors.GetList();
                    var trades = unitOfWork.trades.GetList();
                    var tradesDTOs = mapper.Map<List<tradesDTO>>(trades);
                    foreach(var tradeDTO in tradesDTOs)
                    {
                        var depositor = depositors.Where(x => x.id == tradeDTO.depositor).FirstOrDefault();
                        if (depositor!=null)
                        {
                            tradeDTO.partner = depositor.partner;
                            tradesDTOList.Add(tradeDTO);
                        }
                    }
                    return Ok(tradesDTOList);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetTrades");
                return Ok(ex.ToString());
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
                    var trade = unitOfWork.trades.Get(id);
                    return Ok(trade);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Gettrade");
                return Ok(ex.ToString());
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] trades trade, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<trades>> entityOperationResults = new List<EntityOperationResult<trades>>();
                var entityOperationResult = await _tradesService.UpdateTrade(trade, user_guid);
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
                if (id == null) return BadRequest();
                List<EntityOperationResult<trades>> entityOperationResults = new List<EntityOperationResult<trades>>();
                var entityOperationResult = await _tradesService.DeleteTrade(id, user_guid);
                entityOperationResults.Add(entityOperationResult);
                WriteToLogEntityOperationResult(entityOperationResult, "Delete");
                return Ok(entityOperationResults);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Delete");
                return Ok(ex.ToString());
            }
        }
    }
}
