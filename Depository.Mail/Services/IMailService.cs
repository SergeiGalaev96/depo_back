using Depository.Mail.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Mail.Services
{
    public interface IMailService
    {
        Task<string> SendEmailAsync(MailRequest mailRequest);
        Task<string> SendFileToEmailAsync(MailRequest mailRequest, byte[] binaryData, string contentType, string fileName);
    }
}
