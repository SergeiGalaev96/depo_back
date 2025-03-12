using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Mail.Models
{
    public class MailRequestConsumeContext
    {
        public MailRequest _mailRequest { get; set; }

        public MailRequestConsumeContext(MailRequest mailRequest)
        {
            _mailRequest = mailRequest;
        }
    }
}
