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
    public class ImportedFilesGikController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ImportedFilesGikController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IImported_Files_GikService _imported_files_gikService;
        public ImportedFilesGikController(ILogger<ImportedFilesGikController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IImported_Files_GikService imported_files_gikService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _imported_files_gikService = imported_files_gikService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<imported_files_gik> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] List<imported_files_gik> imported_file_gikList, Guid user_guid)
        {
            List<EntityOperationResult<imported_files_gik>> entityOperationResults = new List<EntityOperationResult<imported_files_gik>>();
            try
            {
                foreach(var imported_file_gik in imported_file_gikList)
                { 
                    var entityOperationResult = await _imported_files_gikService.CreateImported_File_Gik(imported_file_gik, user_guid);
                    entityOperationResults.Add(entityOperationResult);
                    WriteToLogEntityOperationResult(entityOperationResult, "Create");
                }
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
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var imported_files_gik = unitOfWork.imported_files_gik.GetList();
                    return Ok(imported_files_gik);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Getsimported_files_gik");
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
                    var imported_file_gik = unitOfWork.imported_files_gik.Get(id);
                    return Ok(imported_file_gik);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Getimported_file_gik");
                return Ok(ex.ToString());
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] imported_files_gik imported_file_gik, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<imported_files_gik>> entityOperationResults = new List<EntityOperationResult<imported_files_gik>>();
                var entityOperationResult = await _imported_files_gikService.UpdateImported_File_Gik(imported_file_gik, user_guid);
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
                List<EntityOperationResult<imported_files_gik>> entityOperationResults = new List<EntityOperationResult<imported_files_gik>>();
                var entityOperationResult = await _imported_files_gikService.DeleteImported_File_Gik(id, user_guid);
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
