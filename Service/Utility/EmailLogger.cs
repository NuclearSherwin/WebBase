using System;
using System.Threading.Tasks;
using Common.Constants;
using Service.IServices;

namespace Service.Utility
{
    public static class EmailLogger
    {
        public static Task Throw(StringEnums.EmailExceptionType type, Exception ex,
            ISendMailErrorService sendMailErrorService)
        {
            var subject = GetSubject(type, ex.Message);

            MailContent content = new MailContent()
            {
                To = "phongspacenasa@gmail.com",
                Subject = subject,
                Body = GetPlainMessage(type, ex)
            };

            return sendMailErrorService.SendMail(content);
        }

        public static Task Throw(StringEnums.EmailExceptionType type, ISendMailErrorService sendMailErrorService,
            string message, string htmlContent = null)
        {
            var subject = GetSubject(type, message);

            MailContent content = new MailContent()
            {
                To = "phongspacenasa@gmail.com",
                Subject = subject,
                Body = $"{message} - content: {htmlContent}"
            };

            return sendMailErrorService.SendMail(content);
        }
        
        
        
        // helpers
        public static string GetSubject(StringEnums.EmailExceptionType type, string message)
        {
            string subject;
            if (type == StringEnums.EmailExceptionType.Info)
            {
                subject = $"[{type.ToValue()}]: {message}".Substring(0, 200);
            }
            else
            {
                subject =
                    $" [{type.ToValue()}] From {Environment.MachineName} - Exception: {message}".Substring(0, 200);
                
            }

            return subject;
        }

        private static string GetPlainMessage(StringEnums.EmailExceptionType type, Exception ex)
        {
            if (type == StringEnums.EmailExceptionType.Info)
            {
                return ex.Message;
            }

            return $"Exeption message: {ex.Message} StackTrace: {ex.StackTrace}";
        }
    }
}