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
    public class AccountingReportTypesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountingReportTypesController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;

        public AccountingReportTypesController(ILogger<AccountingReportTypesController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<accounting_report_types> entityOperationResult, string action)
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
                    var accounting_report_types = unitOfWork.accounting_report_types.GetList();
                    return Ok(accounting_report_types);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetAccounting_report_types");
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
                    var accounting_report_type = unitOfWork.accounting_report_types.Get(id);
                    return Ok(accounting_report_type);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetAccounting_report_types");
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }
    }
}
