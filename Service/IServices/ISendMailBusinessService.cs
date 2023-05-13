using System.Threading.Tasks;
using Service.Utility;

namespace Service.IServices
{
    public interface ISendMailBusinessService
    {
        Task SendMail(MailContent mailContent);
        Task SendMailAsync(string email, string subject, string htmlMessage);
    }
}