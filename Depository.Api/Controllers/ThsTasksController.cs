
using Depository.Core;
using Depository.Core.Models;
using Depository.DAL;
using Depository.DAL.DbContext;
using Depository.Domain.Services;
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
    public class ThsTasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ThsTasksController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IThsTasksService _thsTasksService;
        public ThsTasksController(ILogger<ThsTasksController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IThsTasksService thsTasksService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _thsTasksService = thsTasksService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<partners> entityOperationResult, string action)
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
                    var ths_Tasks = unitOfWork.ths_tasks.GetList();
                    return Ok(ths_Tasks);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Gets");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }

    }
}
