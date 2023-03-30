using System.Net.Mail;

namespace ECommerceWebApp.Services.Email
{
    public interface IEmailSender
    {
        Task SendMailAsync(MailMessage mailMessage);
    }
}
