using MailKit.Net.Smtp;
using MimeKit;

namespace Library_API.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _logger = logger;
            _config = configuration;
        }

        public async Task SendEmail(string recipientAddress, string subject, string body)
        {
            try
            {

                var message = new MimeMessage();
                message.From.Add(MailboxAddress.Parse(_config.GetSection("EmailSettings:MyEmailAddress").Value));

                var templatePath = Path.Combine(AppContext.BaseDirectory, "EmailTemplate.html");
                var htmlTemplate = File.ReadAllText(@"C:\Users\PaballoMmamaro\source\repos\MyEmailService\MyEmailService\EmailTemplate.html"); ;

                var emailBody = htmlTemplate.Replace("{{Subject}}", subject)
                                            .Replace("{{Body}}", body);

                message.To.Add(MailboxAddress.Parse(recipientAddress));
                message.Subject = subject;
                message.Body = new TextPart("html")
                {
                    Text = emailBody
                };
                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    client.Authenticate(_config.GetSection("EmailSettings:MyEmailAddress").Value,
                        _config.GetSection("EmailSettings:Password").Value);
                    await client.SendAsync(message);
                    client.Disconnect(true);
                }


            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the email service while trying to send an emaail: {ex}",ex.Message);
            }
        }
    }
}
