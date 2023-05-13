using System.IO;
using System.Threading.Tasks;

namespace Common.Utility
{
    public static class EmailHelper
    {
        public static async Task UserManagementSendMail()
        {

        }
		
        private static async Task<string> GetEmailBody(string templateName)
        {
            return await File.ReadAllTextAsync(FileHelper.GetEmailTemplateDirectory(templateName));
        }
    }
}