using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using Depository.DAL.DbContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Depository.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    public class TradesHistoryStatusesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TradesHistoryStatusesController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;

        public TradesHistoryStatusesController(ILogger<TradesHistoryStatusesController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<accounts> entityOperationResult, string action)
        {
            if (entityOperationResult.IsSuccess)
                _logger.LogInformation(action + " : " + JsonConvert.SerializeObject(entityOperationResult));
            else _logger.LogError(action + " : " + JsonConvert.SerializeObject(entityOperationResult));
        }

        private async Task WriteToLogException(Exception exception, string action)
        {
            _logger.LogError(action + " : " + exception.ToString());
        }

       
        [HttpGet]
        public async Task<IActionResult> Gets()
        {
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var accounts = unitOfWork.trades_history_statuses.GetList();
                    return Ok(accounts);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Gets");
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
                    var account = unitOfWork.trades_history_statuses.Get(id);
                    return Ok(account);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Get");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }

       

    }

}

