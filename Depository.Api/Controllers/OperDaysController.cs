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
    [ApiController]
    public class OperDaysController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OperDaysController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IOper_DaysService _operDaysService;
        public OperDaysController(ILogger<OperDaysController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IOper_DaysService operDaysService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _operDaysService = operDaysService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<oper_days> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] oper_days oper_day, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<oper_days>> entityOperationResults = new List<EntityOperationResult<oper_days>>();
                var entityOperationResult = await _operDaysService.Create(oper_day, user_guid);
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

        [HttpPost]
        public async Task<ActionResult> Open([FromBody] DateTime oper_date, Guid user_guid )
        {
            try
            {
                oper_days oper_Days = new oper_days { open= oper_date.Date, date= oper_date.Date };
                
                List<EntityOperationResult<oper_days>> entityOperationResults = new List<EntityOperationResult<oper_days>>();
                var entityOperationResult = await _operDaysService.Open(oper_Days, user_guid);
                entityOperationResults.Add(entityOperationResult);
                WriteToLogEntityOperationResult(entityOperationResult, "Open");
                return Ok(entityOperationResults);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Open");
                return Ok(ex.ToString());
            }
        }

        [HttpPost]
        public async Task<ActionResult> Close([FromBody] DateTime oper_date, Guid user_guid)
        {
            oper_days oper_day = new oper_days();
            try
            {
                oper_days oper_Days = new oper_days { open = oper_date, date = oper_date };
                List<EntityOperationResult<oper_days>> entityOperationResults = new List<EntityOperationResult<oper_days>>();
                var entityOperationResult = await _operDaysService.Close(oper_Days, user_guid);
                entityOperationResults.Add(entityOperationResult);
                WriteToLogEntityOperationResult(entityOperationResult, "Close");
                return Ok(entityOperationResults);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Close");
                return Ok(ex.ToString());
            }
        }



        [HttpGet]
        public async Task<IActionResult> Gets()
        {
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var oper_Days = unitOfWork.oper_days.GetList();
                    return Ok(oper_Days);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Getoper_days");
                return Ok(ex.ToString());
            }
        }

      

        [HttpGet]
        public async Task<IActionResult> IsClosed()
        {
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var IsClosed = unitOfWork.oper_days.IsDayClosed(DateTime.Now);
                    return Ok(IsClosed);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "IsClosed");
                return Ok(ex.ToString());
            }
        }

        [HttpGet]
        public async Task<IActionResult> IsOpened()
        {
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var IsOpened = unitOfWork.oper_days.IsDayOpened(DateTime.Now);
                    if (!IsOpened)
                    {
                        var lastClosedDate = unitOfWork.oper_days.GetLastClosedDate();
                        return Ok(new { IsOpened = false, LastClosedDate = lastClosedDate.date });
                    }
                    else
                    {
                        return Ok(new { IsOpened = true, LastClosedDate = "" });
                    }
                    
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "IsClosed");
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
                    var account = unitOfWork.oper_days.Get(id);
                    return Ok(account);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Getoper_day");
                return Ok(ex.ToString());
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] oper_days oper_day, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<oper_days>> entityOperationResults = new List<EntityOperationResult<oper_days>>();
                var entityOperationResult = await _operDaysService.Update(oper_day, user_guid);
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
                List<EntityOperationResult<oper_days>> entityOperationResults = new List<EntityOperationResult<oper_days>>();
                var entityOperationResult = await _operDaysService.Delete(id, user_guid);
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
