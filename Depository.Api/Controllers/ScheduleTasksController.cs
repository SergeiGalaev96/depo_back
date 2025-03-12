using Microsoft.Extensions.Configuration;
using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using Depository.DAL.DbContext;
using Depository.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Depository.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ScheduleTasksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ScheduleTasksController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly ISchedule_TasksService _schedule_TasksService;
        private readonly IOper_DaysService _operDaysService;
        private readonly ITrades_History_SecuritiesService _trades_History_SecuritiesService;
        private readonly IOrders_History_SecuritiesService _orders_History_SecuritiesService;
        private readonly ITrades_History_CurrenciesService _trades_History_CurrenciesService;
        private readonly IIncoming_PackagesService _incoming_PackagesService;
        private readonly IStock_SecurityService _stock_securityService;
        private readonly IStock_CurrencyService _stock_CurrencyService;
        private readonly IOutgoing_PackagesService _outgoing_PackagesService;

        public ScheduleTasksController(ILogger<ScheduleTasksController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, ISchedule_TasksService schedule_TasksService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _schedule_TasksService = schedule_TasksService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<schedule_tasks> entityOperationResult, string action)
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
                    var schedule_tasks = unitOfWork.schedule_tasks.GetList();
                    return Ok(schedule_tasks);
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
                    var schedule_task = unitOfWork.schedule_tasks.Get(id);
                    return Ok(schedule_task);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Get");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] schedule_tasks schedule_task, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<schedule_tasks>> entityOperationResults = new List<EntityOperationResult<schedule_tasks>>();
                var entityOperationResult = await _schedule_TasksService.Update(schedule_task, user_guid);
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


        
    }
}
