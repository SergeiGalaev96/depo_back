using Depository.Core.Models;
using Depository.Core.Models.DTO;
using Depository.DAL;
using Depository.DAL.DbContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Depository.Controllers
{
    public class ChargingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ChargingController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        public ChargingController(ILogger<ChargingController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
        }

       
        //[HttpPost]
        //public List<report_model1_DTO> GetReport_1(DateTime dt_from, DateTime dt_to, int registrar_id, int service_group_id, int service_type_id)
        //{
        //    IEnumerable<service_groups> service_groups;
        //    IEnumerable<service_types> service_types;
        //    IEnumerable<instructions> instructions;
        //    IEnumerable<issuers> issuers;
        //    List<report_model1_DTO> report_model1_DTOList = new List<report_model1_DTO>();
        //    using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
        //    {
        //        service_types = unitOfWork.service_types.GetList();
        //        issuers = unitOfWork.issuers.GetListByRegistrar(registrar_id);
               
        //            var t=unitOfWork.instructions.GetByRegistrar(dt_from, dt_to, issuers);
        //        instructions = unitOfWork.instructions.GetList();
        //        var report_01_DTOList = instructions.GroupBy(g => new { g.security, g.issuer })
        //                                            .Select(gr =>
        //                                                new report_deponent_model
        //                                                {
        //                                                    issuer_id = gr.Key.issuer.Value,
        //                                                    security_id = gr.Key.security.Value
        //                                                }
        //                                            );          
                                                   
        //        foreach (var report_01_DTO in report_01_DTOList)
        //        {
        //            var issuer = issuers.Where(x => x.id == report_01_DTO.issuer_id).FirstOrDefault();
        //            var security = unitOfWork.securities.Get(report_01_DTO.security_id);
        //                if (issuer != null && security != null)
        //                {
        //                    report_model1_DTO report_model1_DTO = new report_model1_DTO
        //                    {
        //                        issuer_name = issuer.name,
        //                        security_code = security.code,
        //                    };
                   
        //                    foreach (var instruction in instructions)
        //                    {
        //                        var service_type = service_types.Where(x => x.instruction_type == instruction.type).FirstOrDefault();
        //                        if (service_types!=null)
        //                        {
        //                            report1 report1 = new report1();
        //                            report1.instruction_number = instruction.id;
        //                            report1.instruction_type_id = instruction.type.Value;
        //                            report1.createdDate = instruction.created_at;
        //                            report1.quantity = instruction.quantity.Value;
        //                            report1.comission = service_type.index.Value;
        //                            report_model1_DTO.report1List.Add(report1);
        //                        }
        //                    }
        //                    report_model1_DTOList.Add(report_model1_DTO);
        //                }
        //        }
        //        return report_model1_DTOList;
        //    }
        //}

        
    }
}
