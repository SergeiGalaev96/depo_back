using System;
using System.Collections.Generic;
using System.Text;

namespace Depository.Core.Models.DTO
{
    public class mail_and_distributionsDTO
    {
        public mails mails { get; set; }
        public List<mail_distributions> mail_distributions { get; set; }
    }
}
