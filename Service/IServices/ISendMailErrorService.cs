using System.Threading.Tasks;
using Service.Utility;

namespace Service.IServices
{
    public interface ISendMailErrorService
    {
        Task SendMail(MailContent mailContent);
        Task SendMailAsync(string email, string subject, string htmlMessage);
    }
}