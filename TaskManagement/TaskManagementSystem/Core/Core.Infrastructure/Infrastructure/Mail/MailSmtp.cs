using AppShared.Models;
using Infrastructure.Contract;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
namespace Infrastructure.Mail
{
    public class MailSmtp : IMailService
    {
        private readonly ILogger<MailSmtp> _logger;
        public MailSmtp(ILogger<MailSmtp> logger)
        {
            _logger = logger;
        }
        public async Task SendEmail(EmailRequest emailRequest)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse("yadira75@ethereal.email"));
                email.To.Add(MailboxAddress.Parse(emailRequest.To));
                email.Subject = emailRequest.Subject;
                email.Body = new TextPart(TextFormat.Html) { Text = emailRequest.Body };
                using (var smtp = new SmtpClient())
                {
                    smtp.Connect("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
                    //I removed the password to authenticate to Smtp, since i will be pUrsing the code to sourceControl
                    smtp.Authenticate("", "");
                    smtp.Send(email);
                    var result = smtp.Send(email);
                }
                _logger.LogInformation($" send Email to {emailRequest.Body} succesfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An Exception ocuur  {ex.ToString()}");

            }


        }

    }
}

