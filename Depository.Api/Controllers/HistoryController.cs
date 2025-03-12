using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Depository.Core;
using Depository.Core.Models;
using Depository.Core.Models.DTO;
using Depository.DAL;
using Depository.DAL.DbContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Depository.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HistoryController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private const string STR_PARTNERS = "partners";

        public HistoryController(ILogger<HistoryController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<history> entityOperationResult, string action)
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
        public async Task<IActionResult> GetPartners(int id)
        {
            List<partners> partnerList = new List<partners>();
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {

                var histories = unitOfWork.histories.GetByObjectName("partners", id);
                if (histories != null)
                {
                    foreach(var history in histories)
                    {                      
                        try
                        {
                            var serialized_partner =JsonConvert.DeserializeObject<partners>(history.object_str);
                            partnerList.Add(serialized_partner);
                        }
                       finally
                        {

                        }
                    }
                }
                return Ok(partnerList);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetHistory(string object_name, int id)
        {
            List<object> objectList =new  List<object>();
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {

                var histories = unitOfWork.histories.GetByObjectName(object_name, id);
                if (histories != null)
                {
                    foreach (var history in histories)
                    {
                        try
                        {
                            var serialized_object = JsonConvert.DeserializeObject<object>(history.object_str);
                            objectList.Add(serialized_object);
                        }
                        finally
                        {

                        }
                    }
                }
                return Ok(objectList);
            }
        }
    }
}
