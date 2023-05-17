using System.Collections.Generic;
using Data.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebBase.Configurations;

namespace WebBase.Controllers
{
    public class ApiBaseController : ControllerBase
    {
        public string CurrentUserRole => HttpContext.GetRole();

        /// <summary>
        /// The current user logged in.
        /// </summary>

        protected string UserId => HttpContext.GetUserId();

        public User Account => (User)HttpContext.Items["User"];

        protected (IList<IFormFile> attackmentFiles, IList<IFormFile> imageInLineFiles) Attachment()
        {
            var imageInLineFiles = new List<IFormFile>();
            var attachmentFiles = new List<IFormFile>();
            var request = HttpContext.Request;
            foreach (var file in request.Form.Files)
            {
                if (file.Name.Contains("imageInline"))
                {
                    imageInLineFiles.Add(file);
                }
                else
                {
                    attachmentFiles.Add(file);
                }
            }

            return (attachmentFiles, imageInLineFiles);
        }
    }
}