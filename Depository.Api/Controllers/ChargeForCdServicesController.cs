using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Depository.Core;
using Depository.Core.IRepositories;
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
using Npgsql;
using NpgsqlTypes;

namespace Depository.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ChargeForCdServicesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ChargeForCdServicesController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly ICharge_For_Cd_ServicesService _charge_For_Cd_ServicesService;
        public ChargeForCdServicesController(ILogger<ChargeForCdServicesController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, ICharge_For_Cd_ServicesService charge_For_Cd_ServicesService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _charge_For_Cd_ServicesService = charge_For_Cd_ServicesService;
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<charge_for_cd_services> entityOperationResult, string action)
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
        public async Task<ActionResult> Create([FromBody] charge_for_cd_services[] charge_for_cd_services, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<charge_for_cd_services>> entityOperationResults = new List<EntityOperationResult<charge_for_cd_services>>();
                foreach (var charge_for_cd_service in charge_for_cd_services)
                {

                    var entityOperationResult = await _charge_For_Cd_ServicesService.CreateCharge_For_Cd_Service(charge_for_cd_service, user_guid);
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

        [HttpPost]
        public async Task<IActionResult> AutomaticallyCharge([FromBody]  charge_input_model model, Guid user_guid)
        {
            List<EntityOperationResult<charge_for_cd_services>> entityOperationResults = new List<EntityOperationResult<charge_for_cd_services>>();
            //NpgsqlParameter StartDate = new NpgsqlParameter();

            //StartDate.ParameterName = "@StartDate";
            //StartDate.NpgsqlDbType = NpgsqlDbType.Date;
            //StartDate.Direction = ParameterDirection.Input;
            //StartDate.Value = model.StartDate;

            //NpgsqlParameter EndDate = new NpgsqlParameter();
            //EndDate.ParameterName = "@EndDate";
            //EndDate.NpgsqlDbType = NpgsqlDbType.Date;
            //EndDate.Direction = ParameterDirection.Input;
            //EndDate.Value = model.EndDate;

            //NpgsqlParameter DepositorId = new NpgsqlParameter();
            //DepositorId.ParameterName = "@DepositorId";
            //DepositorId.NpgsqlDbType = NpgsqlDbType.Array | NpgsqlDbType.Integer;
            //DepositorId.Direction = ParameterDirection.Input;
            //DepositorId.Value = model.DepositorId;

            //NpgsqlParameter ServiceTypeId = new NpgsqlParameter();
            //ServiceTypeId.ParameterName = "@ServiceTypeId";
            //ServiceTypeId.NpgsqlDbType = NpgsqlDbType.Array | NpgsqlDbType.Integer;
            //ServiceTypeId.Direction = ParameterDirection.Input;
            //ServiceTypeId.Value = model.DepositorId;

            var @params = new Dictionary<string, object>()
            {
                {"@StartDate", model.StartDate.Date},
                 {"@EndDate", model.EndDate.Date},
                  {"@DepositorId", model.DepositorId},
                   {"@ServiceTypeId", model.ServiceTypeId}
            };

            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var @params2 = new Dictionary<string, object>();
                    var query2 = unitOfWork.ExecuteStoredProc("TRUNCATE TABLE public.charge_for_cd_services", @params2);
                    //NpgsqlParameter[] @params = new NpgsqlParameter[] { StartDate, EndDate, DepositorId, ServiceTypeId };
                    var query = unitOfWork.ExecuteStoredProc("SELECT* FROM public.charge_service (date(@StartDate), date(@EndDate), @DepositorId, @ServiceTypeId)", @params);
                    List<charge_for_cd_services> chargeList = new List<charge_for_cd_services>();
                    var dt = query.Result;
                    chargeList = (from DataRow dr in dt.Rows
                                  select new charge_for_cd_services
                                  {
                                      depositor = Convert.ToInt32(dr["payer_id"]!=DBNull.Value ? dr["payer_id"]:0),
                                      service_type = Convert.ToInt32(dr["service_type_id"]!= DBNull.Value ? dr["service_type_id"]:0),
                                      main_quantity = Convert.ToDouble(dr["main_dep"] != DBNull.Value ? dr["main_dep"] :0),
                                      transit_quantity = Convert.ToDouble(dr["transit_dep"] != DBNull.Value ? dr["transit_dep"] : 0),
                                      currency =1,
                                      date = model.EndDate

                                  }).ToList();
                    foreach(var charge_for_cd_service in chargeList)
                    {
                        var entityOperationResult = await _charge_For_Cd_ServicesService.CreateCharge_For_Cd_Service(charge_for_cd_service, user_guid);
                        entityOperationResults.Add(entityOperationResult);
                    }
                    
                    await unitOfWork.CompleteAsync();
                    return Ok(entityOperationResults);
                }
            }
            catch (Exception ex)
            {
               return Ok(JsonConvert.SerializeObject(ex));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Gets()
        {
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var charge_for_cd_services = unitOfWork.charge_for_cd_services.GetList();
                    return Ok(charge_for_cd_services);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetChargeForCdServices");
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
                    var charge_for_cd_service = unitOfWork.charge_for_cd_services.Get(id);
                    return Ok(charge_for_cd_service);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetCharge_for_cd_service");
                return Ok(ex.ToString());
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] charge_for_cd_services charge_for_cd_service, Guid user_guid)
        {
            try
            {
                List<EntityOperationResult<charge_for_cd_services>> entityOperationResults = new List<EntityOperationResult<charge_for_cd_services>>();
                var entityOperationResult = await _charge_For_Cd_ServicesService.UpdateCharge_For_Cd_Service(charge_for_cd_service, user_guid);
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
                List<EntityOperationResult<charge_for_cd_services>> entityOperationResults = new List<EntityOperationResult<charge_for_cd_services>>();
                var entityOperationResult = await _charge_For_Cd_ServicesService.DeleteCharge_For_Cd_Service(id, user_guid);
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
