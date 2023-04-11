using Microsoft.Extensions.Options;
using ECommerceWebApp.Options;
using System.Net;
using System.Net.Mail;

namespace ECommerceWebApp.Services.Email
{
    public class EmailSender:IEmailSender
    {
        private readonly SmtpConfigurationOption SmtpConfig;
        public EmailSender(IOptions<SmtpConfigurationOption> smtpConfigOption)
        {
            SmtpConfig = smtpConfigOption.Value;
        }

        public async Task SendMailAsync(MailMessage mailMessage)
        {
            using var client = new SmtpClient
            {
                Host = SmtpConfig.Host,
                Port = SmtpConfig.Port,
                Credentials = new NetworkCredential(SmtpConfig.UserName, SmtpConfig.Password),
                EnableSsl = true
            };

            await client.SendMailAsync(mailMessage);
        }

    }
}
