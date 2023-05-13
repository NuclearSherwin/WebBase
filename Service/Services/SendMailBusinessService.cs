using System;
using System.Threading.Tasks;
using Amazon.Runtime.Internal.Util;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Service.IServices;
using Service.Utility;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Service.Services
{
    public class SendMailBusinessService : ISendMailBusinessService
    {
        private readonly MailBusinessSettings _mailBusinessSettings;
        private readonly ILogger<SendMailErrorService> _logger;
        
        // mailSetting being inject service of system
        // inject logger to log info
        public SendMailBusinessService(IOptions<MailBusinessSettings> mailBusinessSettings, ILogger<SendMailErrorService> logger)
        {
            _mailBusinessSettings = mailBusinessSettings.Value;
            _logger = logger;
            logger.LogInformation("Create SendMailService");
        }
        
        // Send email, following mailContent

        public async Task SendMail(MailContent mailContent)
        {
            var email = new MimeMessage();
            email.Sender = new MailboxAddress(_mailBusinessSettings.DisplayName, _mailBusinessSettings.Mail);
            email.From.Add(new MailboxAddress(_mailBusinessSettings.DisplayName, _mailBusinessSettings.Mail));
            email.To.Add(MailboxAddress.Parse(mailContent.To));
            email.Subject = mailContent.Subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = mailContent.Body;
            email.Body = builder.ToMessageBody();
            
            // use Smtp of MailKit
            using var smtp = new MailKit.Net.Smtp.SmtpClient();

            try
            {
                smtp.Connect(_mailBusinessSettings.Host, _mailBusinessSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailBusinessSettings.Mail, _mailBusinessSettings.Password);
                await smtp.SendAsync(email);
            }
            catch (Exception exception)
            {
                // Send mail failed, email content will be saved to mailssave folder
                System.IO.Directory.CreateDirectory("mailssave");
                var emailsavefile = string.Format(@"mailssave/{0}.eml", Guid.NewGuid());
                await email.WriteToAsync(emailsavefile);
                
                _logger.LogInformation("Error send mail, saved at - "+ emailsavefile);
                _logger.LogError(exception.Message);
            } 
            
            smtp.Disconnect(true);
            _logger.LogInformation("Send mail to " + mailContent.To);
        }

        public async Task SendMailAsync(string email, string subject, string htmlMessage)
        {
            await SendMail(new MailContent()
            {
                To = email,
                Subject = subject,
                Body = htmlMessage,
            });
        }
    }
}