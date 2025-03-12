using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Depository.Core;
using Depository.Core.IRepositories;
using Depository.Core.Models;
using Depository.Core.Models.DTO;
using Depository.DAL;
using Depository.DAL.DbContext;
using Depository.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Depository.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    public class MetadataController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MetadataController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IMetadataService _metadataService;

        public MetadataController(ILogger<MetadataController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IMetadataService metadataService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _metadataService = metadataService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<metadata> entityOperationResult, string action)
        {
            if (entityOperationResult.IsSuccess)
                _logger.LogInformation(action + " : " + JsonConvert.SerializeObject(entityOperationResult));
            else _logger.LogError(action + " : " + JsonConvert.SerializeObject(entityOperationResult));
        }

        private async Task WriteToLogException(Exception exception, string action)
        {
            _logger.LogError(action + " : " + exception.ToString());
        }

        // GET api/<MetadataController>/5
        [HttpGet]
        public string GetTestId()
        {
            Guid newGuid = Guid.NewGuid();
            metadata metadata = new metadata { type = 1, defid = newGuid, created_at = DateTime.Now, updated_at = DateTime.Now, data="somedata" };
            return JsonConvert.SerializeObject(metadata);
        }

        [HttpGet]
        public async Task<IActionResult> Gets()
        {
            List<string> dataList = new List<string>();
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var metadatas = unitOfWork.metadata.GetAll();
                    foreach(var metadata in metadatas)
                    {
                        dataList.Add(metadata.data);
                    }
                    return Ok(dataList);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetMetadatas");
                return Ok(ex.ToString());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var metadatas = unitOfWork.metadata.GetAll();
                    return Ok(metadatas);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetMetadatas");
                return Ok(ex.ToString());
            }
        }

        [HttpGet]
        public async Task<string> GetByDefId([FromQuery] Guid defid)
        {
            try
            {
                
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var metadata = unitOfWork.metadata.GetByDefId(defid);
                    return JsonConvert.SerializeObject(metadata);
                }
                
            }

            catch (Exception ex)
            {
                WriteToLogException(ex, "GetByDefId");
                return ex.Message;
            }
        }



        // POST api/<MetadataController>
        [HttpPost]
        public async Task<ActionResult> Insert([FromBody] metadata metadata)
        {
            metadata.created_at = DateTime.Now;
            metadata.updated_at = DateTime.Now;
            List<EntityOperationResult<metadata>> entityOperationResults = new List<EntityOperationResult<metadata>>();
            var entityOperationResult = await _metadataService.CreateMetadata(metadata);
            entityOperationResults.Add(entityOperationResult);
            WriteToLogEntityOperationResult(entityOperationResult, "Create");
            return Ok(entityOperationResults);
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] metadata metadata)
        {
            try
            { 
                metadata.updated_at = DateTime.Now;
                List<EntityOperationResult<metadata>> entityOperationResults = new List<EntityOperationResult<metadata>>();
                var entityOperationResult = await _metadataService.UpdateMetadata(metadata);
                entityOperationResults.Add(entityOperationResult);
                 WriteToLogEntityOperationResult(entityOperationResult, "Update");
                return Ok(entityOperationResults);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Update");
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Delete([FromBody] int id)
        {
            try
            {
                if (id == null)  return BadRequest();
                List<EntityOperationResult<metadata>> entityOperationResults = new List<EntityOperationResult<metadata>>();
                var entityOperationResult = await _metadataService.DeleteMetadata(id);
                entityOperationResults.Add(entityOperationResult);
                WriteToLogEntityOperationResult(entityOperationResult, "Delete");
                return Ok(entityOperationResults);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Delete");
                return BadRequest();
            }
        }

      
    }
}
