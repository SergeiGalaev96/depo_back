using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Depository.Core.Models;
using Depository.Core.Models.DTO;
using Depository.DAL;
using Depository.DAL.DbContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using RSDN;

namespace Depository.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TemplateController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<TemplateController> _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfiguration _configuration;
        public TemplateController(ILogger<TemplateController> logger, ApplicationDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        

        [HttpPost]
        public async Task<IActionResult>AddWatermark(IFormFile formFile, string text)
        {
            PdfDocument newPdfDocument = new PdfDocument();
            MemoryStream stream = new MemoryStream();
            byte[] pdfByteArray;
            using (var pdfStream = new MemoryStream())
            {
                formFile.CopyTo(pdfStream);
                PdfDocument importedDocument = PdfReader.Open(pdfStream, PdfDocumentOpenMode.Modify);
                foreach (var page in importedDocument.Pages)
                {
                    AddFooter(page, text);
                }
                importedDocument.Save(stream);
            }

            byte[] docBytes = stream.ToArray();
            return File(docBytes, "application/pdf");
        }

        [HttpPost]
        public async Task<byte[]> AddWatermark2(IFormFile formFile, string userName)
        {
            PdfDocument newPdfDocument = new PdfDocument();
            MemoryStream stream = new MemoryStream();
            byte[] pdfByteArray;
            // var userName = "aZIZ";
            // MemoryStream pdfStream;
            using (var pdfStream = new MemoryStream())
            {
                formFile.CopyTo(pdfStream);
                PdfDocument importedDocument = PdfReader.Open(pdfStream, PdfDocumentOpenMode.Modify);
                foreach (var page in importedDocument.Pages)
                {
                    // PdfPage pdfPage = page;
                    AddFooter(page, userName);
                    //  newPdfDocument.AddPage(pdfPage);
                }
                importedDocument.Save(stream);
                //  return pdfStream.ToArray();
            }
            //  pdfStream.Write(pdfByteArray, 0, pdfByteArray.Length);
            //  pdfStream.Position = 0;





            //var gfx = XGraphics.FromPdfPage(page);
            //var font = new XFont("OpenSans", 20, XFontStyle.Bold);

            //gfx.DrawString("Hello World!", font, XBrushes.Black, new XRect(20, 20, page.Width, page.Height), XStringFormats.Center);


            byte[] docBytes = stream.ToArray();
            return docBytes;
        }

        public static void AddFooter(PdfPage page, string text)
        {
            XFont font;
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XRect rect = new XRect(new XPoint(), gfx.PageSize);
            XSize xSize = new XSize(gfx.PageSize.Width, 25);
            XRect rect2 = new XRect(new XPoint(), xSize);
            rect.Inflate(-15, -20);
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                font = new XFont("Arial", 12, XFontStyle.Regular);
            }
            else
            {
                font = new XFont("DejaVu Serif", 12, XFontStyle.Bold);
            }
            
            XStringFormat format = new XStringFormat
            {
                Alignment = XStringAlignment.Center,
                LineAlignment = XLineAlignment.Far,
                
            };
            XPen pen = new XPen(XColors.Black, 1);
            gfx.DrawRectangle(pen, rect.X, rect.Height, rect.Width, 25 );
            gfx.DrawString($" {text} ", font, XBrushes.Black, rect, format);
            
        }


        private async Task WriteToLogException(Exception exception, string action)
        {
            _logger.LogError(action + " : " + exception.ToString());
        }

        [HttpGet]
        public async Task<IActionResult> InstructionTemplate_01(int id)
        {
            accounts accountTo = new accounts();
            accounts accountFrom=new accounts();
            depositors depositor=null;

            InstructionTemplateDTO template = new InstructionTemplateDTO();
            try { 
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var depositors = unitOfWork.depositors.GetList();
                var partners = unitOfWork.partners.GetList();
               
                var instruction = unitOfWork.instructions.GetViaNestedTables(id);
                if (instruction==null) return Ok(template);
                if (instruction.accTo != null) accountTo = unitOfWork.accounts.Get(instruction.accTo.Value);
                if (instruction.accFrom != null)  accountFrom = unitOfWork.accounts.Get(instruction.accFrom.Value);
                template.number = instruction.id.ToString("D9");
                template.executed_date = instruction.executedDate;
                template.created_at = instruction.created_at;
                
                if (instruction.depositor != null)
                    {
                        var depositor_id = instruction.depositor;
                        depositor = depositors.Where(x => x.id == depositor_id).FirstOrDefault();

                    }
                    else if (instruction.depositor2!=null)
                    {
                        var depositor_id = instruction.depositor2;
                        depositor = depositors.Where(x => x.id == depositor_id).FirstOrDefault();
                    }

                if (depositor!=null)
                { 
                    if (depositor.partner!=null)
                    { 

                        var partner = partners.Where(x => x.id == depositor.partner).FirstOrDefault();
                        if (partner.bank!=null)
                        {
                            var bank = unitOfWork.banks.Get(partner.bank);
                            if (bank != null)
                            {
                                template.bank_name = bank.name;
                                template.bank_address = bank.address;
                                template.bank_bik = bank.bik;
                            }
                        }
                        template.depositor_name = partner != null ? partner.name : "";
                    }
                }


                template.accountTo = accountTo.accnumber;
                template.accountFrom = accountFrom.accnumber;
                if (instruction.issuers!=null) template.issuer_name = instruction.issuers.name;
                if (instruction.securities != null)
                {
                    template.base_type_name = (instruction.securities.security_types != null) ? instruction.securities.security_types.name : "";
                    template.base_code = instruction.securities.code;
                }
                else if (instruction.currencies != null)
                {
                    template.base_type_name =  instruction.currencies.name;
                    template.base_code = instruction.currencies.code;
                }
                template.base_count = instruction.quantity;
                template.base_count_in_words = RusCurrency.Str(instruction.quantity.Value, false, "", "", "", "", "", "").Trim();
                template.basis = instruction.basis;
                template.owner_name = instruction.ownerName;
                template.owner_address = instruction.ownerAddress;
                template.owner_document = instruction.ownerDocument;
                template.payment_recipient = instruction.payment_recipient;
                template.payment_bank = instruction.payment_bank;
                template.payment_account = instruction.payment_account;
                template.payment_bik = instruction.payment_bik;
                template.payment_purpose = instruction.payment_purpose;
                template.filled_user_name = instruction.filledUser;
                template.executed_user_name = instruction.executedUser;
                template.canceled_user_name = instruction.canceledUser;
                template.signed_user_name = instruction.signed_user_full_name;
                template.canceled = instruction.canceled;
                template.executed = instruction.executed;
                template.onExecution = instruction.onExecution;
                template.onExecutionUser = instruction.onExecutionUser;
                template.cancelationReason = instruction.cancelationReason;
                template.signed_cd_full_name = instruction.signed_cd_full_name;
                template.signed_cd_inn = instruction.signed_cd_inn;
                template.signed_depositor_full_name = instruction.signed_depositor_full_name;
                template.signed_depositor_inn = instruction.signed_depositor_inn;

                return Ok(template);
            }
            }
            catch(Exception ex)
            {
                return Ok(JsonConvert.SerializeObject(ex));
            }
        }


     

        [HttpGet]
        public async Task<IActionResult> InstructionTemplate_02(int id)
        {
            accounts accountTo = new accounts();
            accounts accountFrom = new accounts();

            InstructionTemplateDTO template = new InstructionTemplateDTO();
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var instruction = unitOfWork.instructions.GetViaNestedTables(id);
                if (instruction == null) return Ok(template);
                if (instruction.accTo != null) accountTo = unitOfWork.accounts.Get(instruction.accTo.Value);
                if (instruction.accFrom != null) accountFrom = unitOfWork.accounts.Get(instruction.accFrom.Value);
                template.number = instruction.id.ToString("D9");
                template.executed_date = instruction.executedDate;
                template.created_at = DateTime.Now;
                template.depositor_name = instruction.depositors.partners.name;
                template.accountTo = accountTo.accnumber;
                template.accountFrom = accountFrom.accnumber;
                if (instruction.issuers != null) template.issuer_name = instruction.issuers.name;
                if (instruction.securities != null)
                {
                    template.base_type_name = (instruction.securities.security_types != null) ? instruction.securities.security_types.name : "";
                    template.base_code = instruction.securities.code;
                }
                else if (instruction.currencies != null)
                {
                    template.base_type_name = instruction.currencies.name;
                    template.base_code = instruction.currencies.code;
                }
                template.base_count = instruction.quantity;
                template.base_count_in_words = RusCurrency.Str(instruction.quantity.Value, false, "", "", "", "", "", "").Trim();
                template.basis = instruction.basis;
                template.owner_name = instruction.ownerName;
                template.owner_address = instruction.ownerAddress;
                template.owner_document = instruction.ownerDocument;
                return Ok(template);
            }

        }

        [HttpGet]
        public async Task<IActionResult> InstructionTemplate_03(int id)
        {
            accounts accountTo = new accounts();
            accounts accountFrom = new accounts();

            InstructionTemplateDTO template = new InstructionTemplateDTO();
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var instruction = unitOfWork.instructions.GetViaNestedTables(id);
                if (instruction == null) return Ok(template);
                if (instruction.accTo != null) accountTo = unitOfWork.accounts.Get(instruction.accTo.Value);
                if (instruction.accFrom != null) accountFrom = unitOfWork.accounts.Get(instruction.accFrom.Value);
                template.number = instruction.id.ToString("D9");
                template.executed_date = instruction.executedDate;
                template.created_at = DateTime.Now;
                template.depositor_name = instruction.depositors.partners.name;
                template.accountTo = accountTo.accnumber;
                template.accountFrom = accountFrom.accnumber;
                if (instruction.issuers != null) template.issuer_name = instruction.issuers.name;
                if (instruction.securities != null)
                {
                    template.base_type_name = (instruction.securities.security_types != null) ? instruction.securities.security_types.name : "";
                    template.base_code = instruction.securities.code;
                }
                else if (instruction.currencies != null)
                {
                    template.base_type_name = instruction.currencies.name;
                    template.base_code = instruction.currencies.code;
                }
                template.base_count = instruction.quantity;
                template.base_count_in_words = RusCurrency.Str(instruction.quantity.Value, false, "", "", "", "", "", "").Trim();
                template.basis = instruction.basis;
                template.owner_name = instruction.ownerName;
                template.owner_address = instruction.ownerAddress;
                template.owner_document = instruction.ownerDocument;
                return Ok(template);
            }

        }

        [HttpGet]
        public async Task<IActionResult> InstructionTemplate_04(int id)
        {
            accounts accountTo = new accounts();
            accounts accountFrom = new accounts();

            InstructionTemplateDTO template = new InstructionTemplateDTO();
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var instruction = unitOfWork.instructions.GetViaNestedTables(id);
                if (instruction == null) return Ok(template);
                if (instruction.accTo != null) accountTo = unitOfWork.accounts.Get(instruction.accTo.Value);
                if (instruction.accFrom != null) accountFrom = unitOfWork.accounts.Get(instruction.accFrom.Value);
                template.number = instruction.id.ToString("D9");
                template.executed_date = instruction.executedDate;
                template.created_at = DateTime.Now;
                template.depositor_name = instruction.depositors.partners.name;
                template.accountTo = accountTo.accnumber;
                template.accountFrom = accountFrom.accnumber;
                if (instruction.issuers != null) template.issuer_name = instruction.issuers.name;
                if (instruction.securities != null)
                {
                    template.base_type_name = (instruction.securities.security_types != null) ? instruction.securities.security_types.name : "";
                    template.base_code = instruction.securities.code;
                }
                else if (instruction.currencies != null)
                {
                    template.base_type_name = instruction.currencies.name;
                    template.base_code = instruction.currencies.code;
                }
                template.base_count = instruction.quantity;
                template.base_count_in_words = RusCurrency.Str(instruction.quantity.Value, false, "", "", "", "", "", "").Trim();
                template.basis = instruction.basis;
                template.owner_name = instruction.ownerName;
                template.owner_address = instruction.ownerAddress;
                template.owner_document = instruction.ownerDocument;
                return Ok(template);
            }

        }

        [HttpGet]
        public async Task<IActionResult> InstructionTemplate_05(int id)
        {
            accounts accountTo = new accounts();
            accounts accountFrom = new accounts();

            InstructionTemplateDTO template = new InstructionTemplateDTO();
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var instruction = unitOfWork.instructions.GetViaNestedTables(id);
                if (instruction == null) return Ok(template);
                if (instruction.accTo != null) accountTo = unitOfWork.accounts.Get(instruction.accTo.Value);
                if (instruction.accFrom != null) accountFrom = unitOfWork.accounts.Get(instruction.accFrom.Value);
                template.number = instruction.id.ToString("D9");
                template.executed_date = instruction.executedDate;
                template.created_at = DateTime.Now;
                template.depositor_name = instruction.depositors.partners.name;
                template.accountTo = accountTo.accnumber;
                template.accountFrom = accountFrom.accnumber;
                if (instruction.issuers != null) template.issuer_name = instruction.issuers.name;
                if (instruction.securities != null)
                {
                    template.base_type_name = (instruction.securities.security_types != null) ? instruction.securities.security_types.name : "";
                    template.base_code = instruction.securities.code;
                }
                else if (instruction.currencies != null)
                {
                    template.base_type_name = instruction.currencies.name;
                    template.base_code = instruction.currencies.code;
                }
                template.base_count = instruction.quantity;
                template.base_count_in_words = RusCurrency.Str(instruction.quantity.Value, false, "", "", "", "", "", "").Trim();
                template.basis = instruction.basis;
                template.owner_name = instruction.ownerName;
                template.owner_address = instruction.ownerAddress;
                template.owner_document = instruction.ownerDocument;
                return Ok(template);
            }

        }

        [HttpGet]
        public async Task<IActionResult> InstructionTemplate_06(int id)
        {
            accounts accountTo = new accounts();
            accounts accountFrom = new accounts();

            InstructionTemplateDTO template = new InstructionTemplateDTO();
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var instruction = unitOfWork.instructions.GetViaNestedTables(id);
                if (instruction == null) return Ok(template);
                if (instruction.accTo != null) accountTo = unitOfWork.accounts.Get(instruction.accTo.Value);
                if (instruction.accFrom != null) accountFrom = unitOfWork.accounts.Get(instruction.accFrom.Value);
                template.number = instruction.id.ToString("D9");
                template.executed_date = instruction.executedDate;
                template.created_at = DateTime.Now;
                if (instruction.depositors!=null)
                { 
                    template.depositor_name = instruction.depositors.partners.name;
                }
                template.accountTo = accountTo.accnumber;
                template.accountFrom = accountFrom.accnumber;
                if (instruction.issuers != null) template.issuer_name = instruction.issuers.name;
                if (instruction.securities != null)
                {
                    template.base_type_name = (instruction.securities.security_types != null) ? instruction.securities.security_types.name : "";
                    template.base_code = instruction.securities.code;
                }
                else if (instruction.currencies != null)
                {
                    template.base_type_name = instruction.currencies.name;
                    template.base_code = instruction.currencies.code;
                }
                template.base_count = instruction.quantity;
                template.base_count_in_words = RusCurrency.Str(instruction.quantity.Value, false, "", "", "", "", "", "").Trim();
                template.basis = instruction.basis;
                template.owner_name = instruction.ownerName;
                template.owner_address = instruction.ownerAddress;
                template.owner_document = instruction.ownerDocument;
                return Ok(template);
            }

        }

        [HttpGet]
        public async Task<IActionResult> InstructionTemplate_07(int id)
        {
            accounts accountTo = new accounts();
            accounts accountFrom = new accounts();

            InstructionTemplateDTO template = new InstructionTemplateDTO();
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var instruction = unitOfWork.instructions.GetViaNestedTables(id);
                if (instruction == null) return Ok(template);
                if (instruction.accTo != null) accountTo = unitOfWork.accounts.Get(instruction.accTo.Value);
                if (instruction.accFrom != null) accountFrom = unitOfWork.accounts.Get(instruction.accFrom.Value);
                template.number = instruction.id.ToString("D9");
                template.executed_date = instruction.executedDate;
                template.created_at = DateTime.Now;
                template.depositor_name = instruction.depositors.partners.name;
                template.accountTo = accountTo.accnumber;
                template.accountFrom = accountFrom.accnumber;
                if (instruction.issuers != null) template.issuer_name = instruction.issuers.name;
                if (instruction.securities != null)
                {
                    template.base_type_name = (instruction.securities.security_types != null) ? instruction.securities.security_types.name : "";
                    template.base_code = instruction.securities.code;
                }
                else if (instruction.currencies != null)
                {
                    template.base_type_name = instruction.currencies.name;
                    template.base_code = instruction.currencies.code;
                }
                template.base_count = instruction.quantity;
                template.base_count_in_words = RusCurrency.Str(instruction.quantity.Value, false, "", "", "", "", "", "").Trim();
                template.basis = instruction.basis;
                template.owner_name = instruction.ownerName;
                template.owner_address = instruction.ownerAddress;
                template.owner_document = instruction.ownerDocument;
                return Ok(template);
            }

        }
    }
}
