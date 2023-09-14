using AppShared.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Contract
{
    public  interface IMailService
    {
        Task SendEmail(EmailRequest emailRequest);

    }
}
