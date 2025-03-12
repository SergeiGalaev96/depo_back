using Depository.Mail.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Depository.Mail.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }
        public async Task<string> SendEmailAsync(MailRequest mailRequest)
        {
            try { 
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;
            var builder = new BodyBuilder();
            if (mailRequest.Attachments != null)
            {
                byte[] fileBytes;

                    foreach (var file in mailRequest.Attachments)
                    {
                    if (file.Length > 0)
                    {
                            MemoryStream ms = new MemoryStream();
                            //ms.Position = 0;
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                            builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                        //}
                    }
                }
            }
            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.AppPasswordLinux);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
            return "Success";
            }
            catch(Exception ex)
            {
                return ex.ToString();
            }
        }

        public async Task<string> SendFileToEmailAsync(MailRequest mailRequest, byte[] binaryData, string contentType, string fileName )
        {
            try
            {
                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
                email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
                email.Subject = mailRequest.Subject;
                var builder = new BodyBuilder();
                builder.Attachments.Add(fileName, binaryData, ContentType.Parse(contentType));
                builder.HtmlBody = mailRequest.Body;
                email.Body = builder.ToMessageBody();
                using var smtp = new SmtpClient();
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.Mail, _mailSettings.AppPasswordLinux);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}
