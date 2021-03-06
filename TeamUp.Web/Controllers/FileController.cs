﻿namespace TeamUp.Web.Controllers
{
    using System.IO;
    using System.Web;
    using System.Web.Mvc;
    using Microsoft.AspNet.Identity;
    using TeamUp.Data.Contracts;

    public class FileController : BaseController
    {
        public FileController(ITeamUpData data)
            : base(data)
        {
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AvatarUpload(HttpPostedFileBase avatar)
        {
            HttpPostedFileBase avatarPic = Request.Files["avatar"];

            if (avatarPic == null) // Same for file2, file3, file4
            {
                return this.View();
            }

            string extension = Path.GetExtension(avatarPic.FileName);

            if (extension != ".jpg" && extension != ".png")
            {
                return this.View();
            }

            if (avatarPic.ContentLength > 0)
            {
                // Change user ImgPath
                var currentUserId = User.Identity.GetUserId();
                var currentUser = this.Data.Users.Find(currentUserId);
                string fileName = User.Identity.Name.Split('@')[0] + extension;
                currentUser.ImgPath = "Content/Images/Avatars/" + fileName;
                this.Data.Users.Update(currentUser);
                this.Data.SaveChanges();

                // Save locally
                string filePath = Path.Combine(HttpContext.Server.MapPath("~/Content/Images/Avatars"), fileName);
                avatarPic.SaveAs(filePath);
            }

            return this.RedirectToAction("Index", "Home");
        }
    }
}