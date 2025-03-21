using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;

namespace Bulky.Utility
{
    public class EmailSender: IEmailSender
    {
        private string SendGridSecret { get; set; }

        public EmailSender(IConfiguration configuration)
        {
            Console.WriteLine(configuration.GetValue<string>("SenderGrid:APIKey"));
            SendGridSecret = configuration.GetValue<string>("SenderGrid:APIKey");
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SendGrid.SendGridClient(SendGridSecret);
            var from = new SendGrid.Helpers.Mail.EmailAddress("vistaliciaermelinda07@gmail.com");
            var to = new SendGrid.Helpers.Mail.EmailAddress(email);
            var msg = SendGrid.Helpers.Mail.MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);
            return client.SendEmailAsync(msg);
        }
    }
}
