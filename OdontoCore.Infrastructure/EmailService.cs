using Gbarber.Application.Dtos.Email;
using Gbarber.Application.Interfaces;
using GBarber.Core.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace GBarber.Infrastructure
{
    public class EmailService : IEmailService
    {

        public MailSettings _mailSettings { get; }
        public ILogger<EmailService> _logger { get; }
        public EmailService(IOptions<MailSettings> mailSettings, ILogger<EmailService> logger)
        {
            _mailSettings = mailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(EmailRequest request)
        {
            try
            {
                // create message
                var email = new MimeMessage();
                email.Sender = new MailboxAddress(_mailSettings.Display, _mailSettings.Remetente);
                email.From.Add(email.Sender);
                email.To.Add(MailboxAddress.Parse(request.ToEmail));
                email.Subject = request.Subject;
                var builder = new BodyBuilder();
                builder.HtmlBody = request.Body;
                email.Body = builder.ToMessageBody();
                using var smtp = new SmtpClient();
                smtp.Connect(_mailSettings.Server, _mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.Login, _mailSettings.Senha);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);

            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw new Exception(ex.Message);
            }
        }








    }
}
