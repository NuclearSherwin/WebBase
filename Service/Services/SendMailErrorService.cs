using System;
using System.Threading.Tasks;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Service.IServices;
using Service.Utility;

namespace Service.Services
{
    public class SendMailErrorService : ISendMailErrorService
    {
        private readonly MailErrorSettings _mailErrorSettings;
        private readonly ILogger<SendMailErrorService> _logger;

        public SendMailErrorService(
            IOptions<MailErrorSettings> mailSettings, 
            ILogger<SendMailErrorService> logger
        )
        {
            _mailErrorSettings = mailSettings.Value;
            _logger = logger;
            _logger.LogInformation("Create SendMailSerivce");
        }
        
        
        // send mail following in mail content
        public async Task SendMail(MailContent mailContent)
        {
            var email = new MimeMessage();
            email.Sender = new MailboxAddress(_mailErrorSettings.DisplayName, _mailErrorSettings.Mail);
            email.From.Add(new MailboxAddress(_mailErrorSettings.DisplayName, _mailErrorSettings.Mail));
            email.To.Add(MailboxAddress.Parse(mailContent.To));
            email.Subject = mailContent.Subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = mailContent.Body;
            email.Body = builder.ToMessageBody();
            
            // use StmpClient of MailKit
            using var smtp = new MailKit.Net.Smtp.SmtpClient();

            try
            {
                await smtp.ConnectAsync(_mailErrorSettings.Host, _mailErrorSettings.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_mailErrorSettings.Mail, _mailErrorSettings.Password);
                await smtp.SendAsync(email);
            }
            catch (Exception ex)
            {
                // error send mail 
                System.IO.Directory.CreateDirectory("mailssave");
                var emailSavefile = string.Format(@"mailssave/{0}.eml", Guid.NewGuid());
                await email.WriteToAsync(emailSavefile);
                
                _logger.LogInformation("Error send mail, save at - " + emailSavefile);
                _logger.LogError(ex.Message);
                Console.WriteLine(ex.Message);
                throw;
            }
            
            await smtp.DisconnectAsync(true);
            _logger.LogInformation("send mail to " + mailContent.To);
        }

        public async Task SendMailAsync(string email, string subject, string htmlMessage)
        {
            await SendMail(new MailContent()
            {
                To = email,
                Subject = subject,
                Body = htmlMessage
            });
        }
    }
}