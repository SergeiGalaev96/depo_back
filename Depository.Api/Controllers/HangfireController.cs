using Depository.Api.Extentions.Enums;
using Depository.Api.Utilities;
using Depository.Core;
using Depository.Core.Models;
using Depository.Core.Models.DTO;
using Depository.DAL;
using Depository.Domain.Services;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    public class HangfireController : Controller
    {
        private readonly IThsTasksService _thsTasksService;
        private readonly ISchedule_TasksService schedule_TasksService;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IOper_DaysService _operDaysService;
        private readonly ITrades_History_SecuritiesService _trades_History_SecuritiesService;
        private readonly IOrders_History_SecuritiesService _orders_History_SecuritiesService;
        private readonly ITrades_History_CurrenciesService _trades_History_CurrenciesService;
        private readonly IIncoming_PackagesService _incoming_PackagesService;
        private readonly IStock_SecurityService _stock_securityService;
        private readonly IStock_CurrencyService _stock_CurrencyService;
        private readonly IOutgoing_PackagesService _outgoing_PackagesService;
        private readonly ISchedule_TasksService _schedule_TasksService;

        private readonly IConfiguration _configuration;
        private readonly ServiceTaskExecutor serviceTaskExecutor;

        public HangfireController(IOrders_History_SecuritiesService orders_History_SecuritiesService,
            ITrades_History_CurrenciesService trades_History_CurrenciesService,
            IUnitOfWorkFactory unitOfWorkFactory, ISchedule_TasksService schedule_TasksService,
            IOper_DaysService operDaysService, IIncoming_PackagesService incoming_PackagesService,
            IStock_SecurityService stock_securityService, IOutgoing_PackagesService outgoing_PackagesService,
            ITrades_History_SecuritiesService trades_History_SecuritiesService,
            IStock_CurrencyService stock_CurrencyService, IThsTasksService thsTasksService, IConfiguration configuration
           )
        {
            _thsTasksService = thsTasksService;
            _schedule_TasksService = schedule_TasksService;
            _unitOfWorkFactory = unitOfWorkFactory;
            _operDaysService = operDaysService;
            _trades_History_SecuritiesService = trades_History_SecuritiesService;
            _orders_History_SecuritiesService = orders_History_SecuritiesService;
            _trades_History_CurrenciesService = trades_History_CurrenciesService;
            _incoming_PackagesService = incoming_PackagesService;
            _stock_securityService = stock_securityService;
            _outgoing_PackagesService = outgoing_PackagesService;
            _stock_CurrencyService = stock_CurrencyService;
            _configuration = configuration;
             serviceTaskExecutor = new ServiceTaskExecutor
                          (
                          _orders_History_SecuritiesService,
                          _trades_History_CurrenciesService,
                          _unitOfWorkFactory,
                          _schedule_TasksService,
                          _operDaysService,
                          _incoming_PackagesService,
                          _stock_securityService,
                          _outgoing_PackagesService,
                          _trades_History_SecuritiesService,
                          _stock_CurrencyService,
                          _thsTasksService, _configuration
                          );
        }

        /// <summary>
        ///  t+n функция
        /// </summary>
        /// <param name="ths_tasks"></param>
        /// <param name="user_guid"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> DoScheduleTPlusN([FromBody] List<ths_tasks> ths_tasks, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<ths_tasks>> entityOperationResults = new List<EntityOperationResult<ths_tasks>>();
                foreach (var ths_task in ths_tasks)
                {
                    if ((ths_task.id != null) && (ths_task.id > 0))
                    {
                        if (!String.IsNullOrEmpty(ths_task.job_id))
                        { 
                            BackgroundJob.Delete(ths_task.job_id);
                        }
                        var entityOperationResult = await _thsTasksService.Update(ths_task, user_guid);

                        serviceTaskExecutor.DoTPlusN(entityOperationResult, user_guid);
                        entityOperationResults.Add(entityOperationResult);
                    }
                    else
                    {
                        var entityOperationResult = await _thsTasksService.Create(ths_task, user_guid);
                        serviceTaskExecutor.DoTPlusN(entityOperationResult, user_guid);
                        entityOperationResults.Add(entityOperationResult);
                    }
                }

                return Ok(entityOperationResults);
            }
            catch (Exception ex)
            {
                return Ok(ex.ToString());
            }

        }

       

        [HttpPost]
        public async Task<ActionResult> DoScheduleTask([FromBody] schedule_tasks schedule_Task, Guid user_guid)
        {
            List<EntityOperationResult<schedule_tasks>> entityOperationResults = new List<EntityOperationResult<schedule_tasks>>();
            if (schedule_Task == null)
            {
                var entityOperationResult = EntityOperationResult<schedule_tasks>
                           .Failure()
                           .AddError($"Объект равен нулю");
                entityOperationResults.Add(entityOperationResult);
                return Ok(entityOperationResults);
            }
            try
            {

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                    var schedule = unitOfWork.schedule_tasks.GetList().Where(x => x.task_type_id == schedule_Task.task_type_id).FirstOrDefault();
                    if (schedule != null) schedule_Task.id = schedule.id;
            }
            if (schedule_Task.id>0)
            {
                var entityOperationResult =await _schedule_TasksService.Update(schedule_Task, user_guid);
                if (entityOperationResult.IsSuccess)
                {
                    serviceTaskExecutor.DoScheduleByType(schedule_Task.task_type_id, user_guid, schedule_Task.hh, schedule_Task.mm);

                }
                    entityOperationResults.Add(entityOperationResult);
                return Ok(entityOperationResults);
            }
            else if (schedule_Task.hh != null && schedule_Task.mm != null)
            {

                var entityOperationResult =await _schedule_TasksService.Create(schedule_Task, user_guid);
                if (entityOperationResult.IsSuccess)
                {
                        serviceTaskExecutor.DoScheduleByType(schedule_Task.task_type_id, user_guid, schedule_Task.hh, schedule_Task.mm);

                    }
                    entityOperationResults.Add(entityOperationResult);
                    return Ok(entityOperationResults);
            }
            else
            {
                    var entityOperationResult =  EntityOperationResult<schedule_tasks>.Failure()
                           .AddError($"Время указано неверно");
                    entityOperationResults.Add(entityOperationResult);
                    return Ok(entityOperationResults);
            }
            }
            catch (Exception ex)
            {
                var entityOperationResult = EntityOperationResult<schedule_tasks>
                           .Failure()
                           .AddError(JsonConvert.SerializeObject(ex));
                entityOperationResults.Add(entityOperationResult);
                return Ok(entityOperationResults);
            }


        }

        
      

       

       


       


       



    }
}
