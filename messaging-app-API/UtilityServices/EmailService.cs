using messaging_app_API.UtilityServices;
using MailKit.Net.Smtp;
using MailKit.Security;
using messaging_app_API.Models;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;

namespace messaging_app_API.UtilityServices
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration configuration)
        {
            _config = configuration;
        }
        public void SendEmail(EmailModel emailModel)
        {
            var emailMessage = new MimeMessage();
            var from = new MailboxAddress(_config["EmailSettings:FromName"], _config["EmailSettings:From"]);
            emailMessage.From.Add(from);
            emailMessage.To.Add(new MailboxAddress(emailModel.To, emailModel.To));
            emailMessage.Subject = emailModel.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = emailModel.Content
            };
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_config["EmailSettings:SmtpServer"], int.Parse(_config["EmailSettings:SmtpPort"]), SecureSocketOptions.SslOnConnect);
                    client.Authenticate(_config["EmailSettings:From"], _config["EmailSettings:Password"]);
                    client.Send(emailMessage);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error sending email.", ex);
                }
                finally
                {
                    client.Disconnect(true);
                }
            }
        }
    }
}
