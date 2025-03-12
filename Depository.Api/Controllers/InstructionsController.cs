using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Depository.Api.Utilities;
using Depository.Core;
using Depository.Core.Models;
using Depository.Core.Models.DTO;
using Depository.DAL;
using Depository.DAL.DbContext;
using Depository.Domain.Services;
using Depository.Mail.Models;
using Depository.Mail.Services;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RSDN;

namespace Depository.Api.Controllers
{
    [Route("api/[controller]/[action]")]

    public class InstructionsController : Controller
    {
        //private readonly IPublishEndpoint _publishEndpoint;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InstructionsController> _logger;

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        private readonly IInstructionsService _instructionsService;
        private readonly IMailService _mailService;
        public InstructionsController(IMailService mailService, /*IPublishEndpoint publishEndpoint,*/ ILogger<InstructionsController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration, IInstructionsService instructionsService)
        {
            //_publishEndpoint = publishEndpoint;
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
            _instructionsService = instructionsService;
            _mailService = mailService;
        }

        private IActionResult Privacy()
        {
            return View();
        }

        private async Task WriteToLogEntityOperationResult(EntityOperationResult<instructions> entityOperationResult, string action)
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
        public async Task<ActionResult> CreateViaBroker([FromBody] instructions instruction, Guid user_guid)
        {
            try
            {
                if (instruction == null) return BadRequest();
                InstructionWithUserDTO instructionWithUserDTO = new InstructionWithUserDTO(instruction, user_guid);
                //   await _publishEndpoint.Publish<InstructionWithUserDTO>(instructionWithUserDTO);
                return Ok();
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Create");
                return Ok(EntityOperationResult<instructions>
                                    .Failure()
                                    .AddError(ex.ToString()));
            }
        }

        private InstructionTemplateDTO GetInstructionTemplate(int id)
        {
            InstructionTemplateDTO template = new InstructionTemplateDTO();
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var instruction = unitOfWork.instructions.GetViaNestedTables(id);
                var accountTo = unitOfWork.accounts.Get(instruction.accTo);
                var accountFrom = unitOfWork.accounts.Get(instruction.accFrom);
                template.number = instruction.id.ToString("D9");
                template.executed_date = instruction.executedDate;
                template.depositor_name = instruction.depositors.partners.name;
                if (accountTo != null) template.accountTo = accountTo.accnumber; else template.accountTo = "           ";
                if (accountFrom != null) template.accountFrom = accountFrom.accnumber; else template.accountFrom = "           ";
                if (instruction.issuers != null) template.issuer_name = instruction.issuers.name;
                template.base_type_name = instruction.securities.security_types.name;
                template.base_code = instruction.securities.code;
                template.base_count = instruction.quantity;
                template.base_count_in_words = RusCurrency.Str(instruction.quantity.Value, false, "", "", "", "", "", "").Trim();
                template.basis = instruction.basis;
                template.owner_name = instruction.ownerName;
                template.owner_address = instruction.ownerAddress;
                template.owner_document = instruction.ownerDocument;
                template.instruction_type = instruction.type.Value;
                return template;
            }
        }

        [HttpGet]
        public async Task<ActionResult> ShowPdf(int id)
        {
            InstructionTemplateDTO template = new InstructionTemplateDTO();
            template = GetInstructionTemplate(id);

            var actionPdf = new Rotativa.AspNetCore.ViewAsPdf("InstructionType" + template.instruction_type.ToString(), template)
            {
                FileName = id.ToString() + ".pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageHeight = 20,
            };
            var pdfData = await actionPdf.BuildFile(this.ControllerContext);
            MemoryStream workStream = new MemoryStream();
            workStream.Write(pdfData, 0, pdfData.Length);
            workStream.Position = 0;
            return new FileStreamResult(workStream, "application/pdf");
        }
        [HttpGet]
        public ActionResult InstructionType6(InstructionTemplateDTO instructionTemplate)
        {

            return View(instructionTemplate);
        }

        [HttpGet]
        public ActionResult InstructionPdf(InstructionTemplateDTO instructionTemplate)
        {

            return View(instructionTemplate);
        }



        [HttpGet]
        public async Task<byte[]> ShowPdfByInstructionId(int id)
        {
            InstructionTemplateDTO template = new InstructionTemplateDTO();
            template = GetInstructionTemplate(id);

            var actionPdf = new Rotativa.AspNetCore.ViewAsPdf("InstructionPdf", template)
            {
                FileName = "file" + ".pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageHeight = 20,
            };
            var pdfData = await actionPdf.BuildFile(this.ControllerContext);
            return pdfData;
        }

        private async Task<ActionResult> SendPdfToEmail(string ToEMail, string Body, byte[] binaryData, int instruction_id)
        {
            MailRequest mailRequest = new MailRequest();
            mailRequest.Subject = "Depository";
            mailRequest.ToEmail = ToEMail;
            mailRequest.Body = Body;
            var result = _mailService.SendFileToEmailAsync(mailRequest, binaryData, "application/pdf", instruction_id.ToString() + ".pdf");
            return Ok(result);
        }


        private async Task<ActionResult> SendMail(instructions instruction)
        {
            MailRequest mailRequest = new MailRequest();
            try
            {
                List<partner_contacts> partner_contacts = new List<partner_contacts>();
                mailRequest.Subject = "Depository";
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var issuer = unitOfWork.issuers.Get(instruction.issuer);
                    var partner = unitOfWork.partners.GetViaContacts(issuer.id);
                    foreach (var partner_contact in partner.partner_contacts)
                    {
                        mailRequest.ToEmail = partner_contact.mail;
                        mailRequest.Body = "This is a body of message";
                        var pdfData = ShowPdfByInstructionId(instruction.id).Result;
                        using (var ms = new MemoryStream(pdfData))
                        {
                            ms.Position = 0;

                            IFormFile formFile = new FormFile(ms, 0, ms.Length, instruction.id.ToString(), instruction.id.ToString() + ".pdf")
                            {
                                Headers = new HeaderDictionary(),
                                ContentType = "application/pdf",
                                // ContentDisposition = new System.Net.Mime.ContentDisposition { FileName="file", Inline=false }
                            };
                            //  var file = File(pdfData, System.Net.Mime.MediaTypeNames.Application.Octet, "filename.pdf");
                            mailRequest.Attachments.Add(formFile);
                        }

                        //mailRequest.Attachments.Add(pdfData);
                        //    await _publishEndpoint.Publish<MailRequest>(mailRequest);
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return Ok(ex);
            }

        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] instructions instruction, Guid user_guid)
        {
            try
            {
                if (instruction == null) return BadRequest();
                instruction.issuers = null;
                instruction.depositors = null;
                instruction.securities = null;
                InstructionWithUserDTO instructionWithUserDTO = new InstructionWithUserDTO(instruction, user_guid);
                List<EntityOperationResult<instructions>> entityOperationResults = new List<EntityOperationResult<instructions>>();
                var entityOperationResult = await _instructionsService.CreateInstruction(instruction, user_guid);

                if (entityOperationResult.IsSuccess)
                {
                    SendMail(instruction);
                }
                entityOperationResults.Add(entityOperationResult);
                WriteToLogEntityOperationResult(entityOperationResult, "Create");
                return Ok(entityOperationResults);
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "Create");
                return Ok(EntityOperationResult<instructions>
                                    .Failure()
                                    .AddError(ex.ToString()));
            }
        }

        public class MarkAsSignedDTO
        {
            public List<instructions> instructionList { get; set; }
            public string inn { get; set; }
            public string signed_user_full_name { get; set; }
        }




        public class CanceledInstructionDTO
        {
            public int id { get; set; }

        }

        [HttpPost]
        public async Task<ActionResult> CancelList([FromBody] List<CanceledInstructionDTO> cancel_instruction_list, string cancelationReason, Guid user_guid)
        {
            List<EntityOperationResult<instructions>> entityOperationResults = new List<EntityOperationResult<instructions>>();
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var user = unitOfWork.users.GetByUserId(user_guid);
                foreach (var cancel_instruction in cancel_instruction_list)
                {
                    var instruction = unitOfWork.instructions.Get(cancel_instruction.id);

                    if (instruction != null && user != null)
                    {
                        instruction.canceled = true;
                        instruction.canceledDate = DateTime.Now;
                        instruction.cancelationReason = cancelationReason;

                        if (user != null)
                        {
                            instruction.canceledUser = user.firstname + " " + user.lastname;
                        }
                        var entityOperationResult = await _instructionsService.UpdateInstruction(instruction, user_guid);
                        entityOperationResults.Add(entityOperationResult);
                    }
                }
            }
            return Ok(entityOperationResults);
        }


        [HttpPost]
        public async Task<ActionResult> MarkAsSigned([FromBody] MarkAsSignedDTO markAsSigned, Guid user_guid)
        {
            var inn = markAsSigned.inn;
            var signed_user_full_name = markAsSigned.signed_user_full_name;
            var instructionList = markAsSigned.instructionList;
            if (String.IsNullOrEmpty(inn))
            {
                var errorCaption = "ПИН не задан";
                _logger.LogError("MarkAsSigned: " + errorCaption);
                return Ok(EntityOperationResult<instructions>
                           .Failure()
                           .AddError(errorCaption));
            }
            try
            {
                List<EntityOperationResult<instructions>> entityOperationResults = new List<EntityOperationResult<instructions>>();
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var user_with_credentials = unitOfWork.users.GetByUserId(user_guid);
                    if (user_with_credentials == null)
                    {
                        var errorCaption = "Пользователь с таким идентификатором не существует, недостаточно полномочий для выполнения этой операции";
                        _logger.LogError("MarkAsSigned: " + errorCaption);
                        return Ok(EntityOperationResult<instructions>
                            .Failure()
                            .AddError(errorCaption));
                    }
                    foreach (var instructionsItem in instructionList)
                    {
                        var instruction = unitOfWork.instructions.Get(instructionsItem.id);
                        if (instruction != null)
                        {
                            instruction.signed = true;
                            instruction.signed_at = DateTime.Now;
                            instruction.inn = inn;
                            instruction.signed_user_full_name = signed_user_full_name;
                            var entityOperationResult = await _instructionsService.UpdateInstruction(instruction, user_guid);
                            entityOperationResults.Add(entityOperationResult);
                            WriteToLogEntityOperationResult(entityOperationResult, "MarkAsSigned");

                        }
                    }
                    return Ok(entityOperationResults);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "MarkAsOpened");
                return Ok(ex);
            }

        }

        [HttpPost]
        public async Task<ActionResult> MarkAsOpened([FromBody] int id, Guid user_guid)
        {
            if (id == null)
            {
                var errorCaption = "Идентификатор равен нулю";
                _logger.LogError("MarkAsOpened: " + errorCaption);
                return Ok(EntityOperationResult<instructions>
                            .Failure()
                            .AddError(errorCaption));
            }
            try
            {
                List<EntityOperationResult<instructions>> entityOperationResults = new List<EntityOperationResult<instructions>>();
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var instruction = unitOfWork.instructions.Get(id);
                    if (instruction != null)
                    {
                        instruction.opened = true;
                        var entityOperationResult = await _instructionsService.UpdateInstruction(instruction, user_guid);
                        entityOperationResults.Add(entityOperationResult);
                        WriteToLogEntityOperationResult(entityOperationResult, "Update");
                        return Ok(entityOperationResults);
                    }
                    else
                    {
                        var errorCaption = "Объект с таким идентификатором не найден - " + id.ToString();
                        _logger.LogError("MarkAsOpened: " + errorCaption);
                        return Ok(EntityOperationResult<instructions>
                                    .Failure()
                                    .AddError(errorCaption));
                    }
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "MarkAsOpened");
                return Ok(ex);
            }
        }



        [HttpGet]
        public async Task<IActionResult> GetByDepositorAndPeriod(int depositor, DateTime periodStart, DateTime periodEnd)
        {
            List<instructionDTO> instructionDTOList = new List<instructionDTO>();

            if (depositor == null)
            {
                return Ok("Depositor id is null");
            }
            else if (periodStart == null || periodEnd == null)
            {
                return Ok("Period datetime is null");
            }
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var instructions = unitOfWork.instructions.GetByPeriodAndDepositor(depositor, periodStart, periodEnd);
                foreach (var instruction in instructions)
                {
                    var accounts = unitOfWork.accounts.GetList();
                    instructionDTO instructionDTO = new instructionDTO();
                    instructionDTO.number = instruction.id.ToString("D9");
                    instructionDTO.executed_date = instruction.executedDate;
                    instructionDTO.created_at = instruction.created_at;
                    if (instruction.depositor != null)
                    {
                        instructionDTO.depositor_name = instruction.depositors.partners.name;
                        instructionDTO.depositor_id = instruction.depositor;
                    }
                    if (instruction.accTo != null)
                    {
                        instructionDTO.accountTo = accounts.Where(x => x.id == instruction.accTo).FirstOrDefault().accnumber;
                        instructionDTO.accountTo_id = instruction.accTo;
                    }
                    if (instruction.accFrom != null)
                    {
                        instructionDTO.accountFrom = accounts.Where(x => x.id == instruction.accFrom).FirstOrDefault().accnumber;
                        instructionDTO.accountTo_id = instruction.accFrom;
                    }
                    if (instruction.issuer != null)
                    {
                        instructionDTO.issuer_name = instruction.issuers.name;
                        instructionDTO.issuer_id = instruction.issuer;
                    }
                    if (instruction.securities != null)
                    {
                        instructionDTO.base_type_name = (instruction.securities.security_types != null) ? instruction.securities.security_types.name : "";
                        instructionDTO.base_code = instruction.securities.code;
                    }
                    else if (instruction.currencies != null)
                    {
                        instructionDTO.base_type_name = instruction.currencies.name;
                        instructionDTO.base_code = instruction.currencies.code;
                    }
                    instructionDTO.base_count = instruction.quantity;
                    instructionDTO.type_name = instruction.instruction_types.name;
                    instructionDTO.type_id = instruction.id;
                    instructionDTOList.Add(instructionDTO);
                }

            }
            return Ok(instructionDTOList);
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
                    var instruction = unitOfWork.instructions.Get(id);
                    return Ok(instruction);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetInstruction");
                return BadRequest();
            }
        }

        private byte[] DownloadFileFromUrl(string urlFile, string login, string password, int instruction_id)
        {
            using (WebClient client = new WebClient())
            {
                client.Credentials = new NetworkCredential(login, password);
                var url = urlFile + instruction_id.ToString();
                byte[] binaryData = client.DownloadData(url);

                return binaryData;
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] instructions instruction, Guid user_guid)
        {
            const int CENTRAL_DEPOSITORY_STATUS = 0;
            const int CENTRAL_DEPOSITORY_ID = 1;
            const int DEPOSITING_THE_CENTRAL_BANK = 6;
            const int WRITEOFF_THE_CENTRAL_BANK = 25;
            int status_old = 0, status_new = 0;
            int? trans_account, owner = null;
            string acc_number = "", status = "";
            byte[] binaryData = null;
            List<EntityOperationResult<instructions>> entityOperationResults = new List<EntityOperationResult<instructions>>();
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var instruction_old = unitOfWork.instructions.Get(instruction.id);
                if (instruction_old.canceled != null && instruction_old.onExecution != null)

                    if (instruction_old.canceled.Value && instruction_old.onExecution.Value) status_old = 3;

                    else if (instruction_old.executed.Value && instruction_old.onExecution.Value) status_old = 2;
                    else if (instruction_old.onExecution.Value) status_old = 1;
                    else if (instruction_old.filled.Value) status_old = 0;


                if (instruction.canceled.Value && instruction.onExecution.Value) status_new = 3;
                else if (instruction.executed.Value && instruction.onExecution.Value) status_new = 2;
                else if (instruction.onExecution.Value) status_new = 1;
                else if (instruction.filled.Value)
                {
                    status_new = 0;
                    if (instruction.issuer != null && (instruction.type.Value == DEPOSITING_THE_CENTRAL_BANK || instruction.type.Value == WRITEOFF_THE_CENTRAL_BANK))
                    {
                        var login = _configuration.GetValue<string>("JasperSettings:Login");
                        var password = _configuration.GetValue<string>("JasperSettings:Password");
                        var issuer = unitOfWork.issuers.Get(instruction.issuer);
                        if (issuer != null)
                        {
                            var registrar = unitOfWork.registrars.Get(issuer.registrar);
                            if (registrar != null)
                            {
                                var partner = unitOfWork.partners.Get(registrar.partner);
                                if (partner != null && !String.IsNullOrEmpty(partner.email))
                                {
                                    try
                                    {


                                        if (instruction.type.Value == DEPOSITING_THE_CENTRAL_BANK)
                                        {
                                            var enrollmentArrangementsLink = _configuration.GetValue<string>("Links:EnrollmentArrangementsLink");
                                            binaryData = DownloadFileFromUrl(enrollmentArrangementsLink, login, password, instruction.id);
                                        }
                                        else if (instruction.type.Value == WRITEOFF_THE_CENTRAL_BANK)
                                        {
                                            var writeOffArrangementsLink = _configuration.GetValue<string>("Links:WriteOffArrangementsLink");
                                            binaryData = DownloadFileFromUrl(writeOffArrangementsLink, login, password, instruction.id);
                                        }
                                        if (binaryData != null)
                                            SendPdfToEmail(partner.email, "Передаточное распоряжение во вложении", binaryData, instruction.id);
                                    }
                                    catch (Exception ex)
                                    {
                                        WriteToLogException(ex, "DownloadFileFromUrl");
                                    }
                                }
                            }
                        }
                    }
                }



                var accounting_entries = unitOfWork.accounting_entry.GetByTemplate(instruction.type.Value, status_old, status_new);

                try
                {
                    unitOfWork.BeginTransaction();
                    foreach (var entry in accounting_entries)
                    {
                        if (entry.account_type == null) // Конечный счет "-" == финальный счет списания или зачисления
                        {
                            if (entry.from_to == 0 && instruction.accFrom != null) // Поле from_to == 0 операция -, from_to == 1 операция +
                                trans_account = instruction.accFrom; // -
                            else trans_account = instruction.accTo; // +
                        }
                        else
                        {
                            var account_type = unitOfWork.account_types.GetByCode(entry.account_type.Value);
                            status = account_type.astatus;
                            switch (account_type.amember)
                            {
                                case "0":
                                    var corr_depository = unitOfWork.corr_depositories.Get(instruction.corrDepository);
                                    owner = corr_depository.partner;
                                    break;
                                case "1":
                                    if (instruction.depositor != null)
                                    {
                                        var depositor1 = unitOfWork.depositors.Get(instruction.depositor);
                                        owner = depositor1.partner;
                                    }
                                    else
                                    {
                                        var depositor2 = unitOfWork.depositors.Get(instruction.depositor2);
                                        owner = depositor2.partner;
                                    }
                                    break;
                                case "2":
                                    var security = unitOfWork.securities.Get(instruction.security);
                                    //var issuer = unitOfWork.issuers.GetViaNestedTables(security.issuer);01.12.2023
                                    if (security != null)
                                    {
                                        var registrar = unitOfWork.registrars.Get(security.registrar);
                                        if (registrar != null)
                                        {
                                            owner = registrar.partner;
                                        }
                                        else
                                        {
                                            entityOperationResults.Add(EntityOperationResult<instructions>
                                            .Failure()
                                            .AddError($"Регистратор указанной  в ценной бумаге: {security.code} не существует."));
                                            return Ok(entityOperationResults);
                                        }
                                    }
                                    else
                                    {
                                        entityOperationResults.Add(EntityOperationResult<instructions>
                                       .Failure()
                                       .AddError($"Такой ценной бумаги не существует."));
                                        return Ok(entityOperationResults);
                                    }
                                    break;
                                case "3":
                                    status = instruction.tradingSystem.ToString();
                                    var depositor = unitOfWork.depositors.Get(instruction.depositor);
                                    owner = depositor.partner;
                                    break;
                                case "4":
                                    owner = instruction.security;
                                    break;
                                case "5":
                                    var depository = unitOfWork.depositories.Get(CENTRAL_DEPOSITORY_ID);
                                    // var depositor = unitOfWork.depositories.Get(depository.id);
                                    owner = depository.partner;
                                    break;

                            }
                            acc_number = status + '-' + entry.account_type.ToString() + '-' + owner.ToString();

                            var account = unitOfWork.accounts.GetByNumber(acc_number);
                            if (account != null)
                            {
                                trans_account = unitOfWork.accounts.GetByNumber(acc_number).id;
                            }
                            else
                            {
                                entityOperationResults.Add(EntityOperationResult<instructions>
                               .Failure()
                               .AddError($"Счет: " + acc_number + " не существует."));
                                return Ok(entityOperationResults);
                            }
                        }

                        var transaction = new transactions { trans_type = entry.operation, instruction = instruction.id, account = trans_account.Value, stop = entry.stop.ToString(), date = DateTime.Now, created_at = DateTime.Now, updated_at = DateTime.Now };
                        var trans_entity = unitOfWork.transactions.InsertAsyncWithoutHistory(transaction);

                    }

                    var entityOperationResult = await _instructionsService.UpdateInstruction(instruction, user_guid);
                    if (entityOperationResult.IsSuccess) unitOfWork.CommitTransaction();
                    else unitOfWork.RollbackTransaction();
                    entityOperationResults.Add(entityOperationResult);
                    WriteToLogEntityOperationResult(entityOperationResult, "Update");
                    return Ok(entityOperationResults);
                }
                catch (Exception ex)
                {
                    unitOfWork.RollbackTransaction();
                    WriteToLogException(ex, "Update");
                    return Ok(ex);
                }
            }


        }

        [HttpPost]
        public async Task<ActionResult> Delete([FromBody] int? id, Guid user_guid)
        {
            if (id == null)
            {
                return BadRequest();
            }
            else
            {
                try
                {
                    List<EntityOperationResult<instructions>> entityOperationResults = new List<EntityOperationResult<instructions>>();
                    var entityOperationResult = await _instructionsService.DeleteInstruction(id, user_guid);
                    entityOperationResults.Add(entityOperationResult);
                    WriteToLogEntityOperationResult(entityOperationResult, "Delete");
                    return Ok(entityOperationResults);
                }
                catch (Exception ex)
                {
                    WriteToLogException(ex, "Delete");
                    return Ok(EntityOperationResult<instructions>
                                    .Failure()
                                    .AddError(ex.ToString()));
                }

            }
        }


        [HttpGet]
        public async Task<IActionResult> Gets(int page, int size)
        {
            var skip = (page - 1) * size;

            try
            {
                if ((page == null) || (page <= 0))
                {
                    page = 1;
                }
                if ((size == null) || (size <= 0))
                {
                    size = 2000;
                }
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var instructions = unitOfWork.instructions.GetListByPage(skip, size);

                    return Ok(instructions);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetInstructions");
                return Ok(EntityOperationResult<instructions>
                                    .Failure()
                                    .AddError(ex.ToString()));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetInstructionsByDepositorPartner(int page, int size, int depositor_partner)
        {
            var skip = (page - 1) * size;

            try
            {
                if ((page == null) || (page <= 0))
                {
                    page = 1;
                }
                if ((size == null) || (size <= 0))
                {
                    size = 2000;
                }
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var instructions = unitOfWork.instructions.GetInstructionsByDepositorPartner(skip, size, depositor_partner);

                    return Ok(instructions);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetInstructions");
                return Ok(EntityOperationResult<instructions>
                                    .Failure()
                                    .AddError(ex.ToString()));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetInstructionsByRegistrarPartner(int page, int size, int registrar_partner)
        {
            var skip = (page - 1) * size;

            try
            {
                if ((page == null) || (page <= 0))
                {
                    page = 1;
                }
                if ((size == null) || (size <= 0))
                {
                    size = 2000;
                }
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var instructions = unitOfWork.instructions.GetInstructionsByRegistrarPartner(skip, size, registrar_partner);

                    return Ok(instructions);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetInstructions");
                return Ok(EntityOperationResult<instructions>
                                    .Failure()
                                    .AddError(ex.ToString()));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetInstructionsRegistrar(int page, int size)
        {
            var skip = (page - 1) * size;

            try
            {
                if ((page == null) || (page <= 0))
                {
                    page = 1;
                }
                if ((size == null) || (size <= 0))
                {
                    size = 2000;
                }
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var instructions = unitOfWork.instructions.GetInstructionsRegistrar(skip, size);

                    return Ok(instructions);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetInstructions");
                return Ok(EntityOperationResult<instructions>
                                    .Failure()
                                    .AddError(ex.ToString()));
            }
        }



        [HttpGet]
        public async Task<IActionResult> GetInstructionsByDepository(int page, int size)
        {
            var skip = (page - 1) * size;

            try
            {
                if ((page == null) || (page <= 0))
                {
                    page = 1;
                }
                if ((size == null) || (size <= 0))
                {
                    size = 2000;
                }
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var instructions = unitOfWork.instructions.GetInstructionsByDepository(skip, size);

                    return Ok(instructions);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetInstructions");
                return Ok(EntityOperationResult<instructions>
                                    .Failure()
                                    .AddError(ex.ToString()));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetInstructionsByCorrDepositoryPartner(int page, int size, int corr_depository_partner)
        {
            var skip = (page - 1) * size;

            try
            {
                if ((page == null) || (page <= 0))
                {
                    page = 1;
                }
                if ((size == null) || (size <= 0))
                {
                    size = 2000;
                }
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var instructions = unitOfWork.instructions.GetInstructionsByCorrDepositoryPartner(skip, size, corr_depository_partner);

                    return Ok(instructions);
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetInstructions");
                return Ok(EntityOperationResult<instructions>
                                    .Failure()
                                    .AddError(ex.ToString()));
            }
        }


        [HttpPost]
        public async Task<ActionResult> FullTextSearch([FromBody] instructions instruction, int page, int size)
        {
            var skip = (page - 1) * size;
            if (page <= 0 || size <= 0)
            {
                return BadRequest();
            }
            else if (instruction == null)
            {
                try
                {
                    using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                    {
                        var instructions = unitOfWork.instructions.GetList().Skip(skip).Take(size);

                        return Ok(instructions);
                    }
                }
                catch (Exception ex)
                {
                    WriteToLogException(ex, "FullTextSearch");
                    return Ok(EntityOperationResult<instructions>
                                   .Failure()
                                   .AddError(ex.ToString()));
                }
            }
            else
            {
                try
                {

                    using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                    {
                        var instructions = unitOfWork.instructions.FullTextSearch(instruction).Skip(skip).Take(size);

                        return Ok(instructions);
                    }
                }
                catch (Exception ex)
                {
                    WriteToLogException(ex, "FullTextSearch");
                    return Ok(EntityOperationResult<instructions>
                                   .Failure()
                                   .AddError(ex.ToString()));
                }
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetCountNewInstructions()
        {
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var countNewInstructions = unitOfWork.instructions.GetCountNewInstruction();
                    var isUrgentAndNotOpened = unitOfWork.instructions.CheckUrgentInstructions();
                    return Ok(new { Count = countNewInstructions, Urgent = isUrgentAndNotOpened });
                }
            }
            catch (Exception ex)
            {
                WriteToLogException(ex, "GetCountNewInstructions");
                return Ok(EntityOperationResult<instructions>
                                   .Failure()
                                   .AddError(ex.ToString()));
            }
        }
    }
}
