using System;
using System.IO;

namespace Common.Utility
{
    public static class FileHelper
    {
        public static string GetEmailTemplateDirectory(string emailTemplate)
        {
            var path = new string[] { AppContext.BaseDirectory, "HtmlTemplates", "EmailTemplates", emailTemplate };
            return Path.Combine(path);
        }
    }
}