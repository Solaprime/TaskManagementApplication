using System;
using System.Collections.Generic;
using System.Text;

namespace AppShared.ConfigurationDto
{
   public class SmtpSecret
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string SmtpConnect { get; set; }
        public int SmtpPort { get; set; }
    }
}
