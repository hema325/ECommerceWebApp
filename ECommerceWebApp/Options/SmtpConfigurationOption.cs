using System.ComponentModel.DataAnnotations;

namespace ECommerceWebApp.Options
{
    public class SmtpConfigurationOption
    {
        public const string SmtpConfiguration = "SmtpConfiguration";
        [Required]
        public string Host { get; set; }
        [Required]
        public int Port { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
