using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using WebRozgar.Models;
using WebRozgar.ViewModels;
using WebRozgar.DAL;
using WebRozgar.Helpers;
using System.Text;

namespace WebRozgar.Controllers
{
    
    public class AccountController : Controller
    {
        private IWebRozgarService _Service;

        public AccountController(IWebRozgarService service)
        {
            _Service = service;
        }

        public ActionResult Login(string returnUrl)
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index","Home");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
                {
                    if (_Service.isConfirmed(model.UserName))
                    {
                        if (!_Service.IsProfileCompleted(model.UserName))
                        {
                            return RedirectToAction("Manage", "Account");
                        }
                        if (string.IsNullOrEmpty(returnUrl))
                        {
                            return RedirectToAction("Profile", "My");
                        }
                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        WebSecurity.Logout();
                        return RedirectToAction("EmailVerification", "Account", new { status = "notactivate", email = "" });
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            ViewData["Error"] = "The user name or password provided is incorrect.";
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Forgot()
        {
            StringViewModel model = new StringViewModel();
            return View(model);
        }
        
        [HttpPost]
        [AllowAnonymous]
        public string Forgot(StringViewModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception();
            }
            _Service.SendForgotMail(model.Data);
            return "Reset Link Sent Successfully!!!";
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string resetToken)
        {
            if (!_Service.VerifyResetToken(resetToken))
            {
                throw new Exception();
            }
            PasswordResetModel model = new PasswordResetModel();
            model.ResetToken = resetToken;
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(PasswordResetModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);   
            }
            if (model.Password != model.ConfirmPassword)
            {
                ViewData["Error"] = "Don't try to play";
                return View(model);
            }
            WebSecurity.ResetPassword(model.ResetToken, model.Password);
            return RedirectToAction("Login", "Account");
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            ViewBag.MessageCount = _Service.NoOfUnseenMessages(User.Identity.Name);
            ChangePasswordViewModel model = new ChangePasswordViewModel();
            return View(model);
        }
        
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public string ChangePassword(ChangePasswordViewModel model)
        {
            ViewBag.MessageCount = _Service.NoOfUnseenMessages(User.Identity.Name);
            
            try
            {
                if (!ModelState.IsValid)
                {
                    return "Please enter the requried field";
                }
                if (WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                {
                    return "Password changed successfully";
                }
                return "Please enter correct old password";
            }
            catch
            {
                return "An error occured while processing your request";
            }      
        }

        [Authorize]
        public ActionResult Manage()
        {
            ViewBag.MessageCount = _Service.NoOfUnseenMessages(User.Identity.Name);
            
            if (User.IsInRole("recruiter"))
            {
                RecruiterProfileViewModel model = _Service.GetRecruiterProfile(User.Identity.Name);
                return View("recruiterManageView",model);
            }
            else if (User.IsInRole("seeker"))
            {
                SeekerProfileViewModel model = _Service.GetSeekerProfile(User.Identity.Name);
                return View("seekerManageView",model);
            }
            else
            {
                //Have to think about it
                return View();
            }
        }

        [Authorize(Roles="seeker")]
        [HttpPost]
        public ActionResult Manage(SeekerProfileViewModel model)
        {
            ViewBag.MessageCount = _Service.NoOfUnseenMessages(User.Identity.Name);
            
            if (!ModelState.IsValid)
            {
                ViewBag.Message = "-1";
                return View("seekerManageView", model);
            }
            _Service.UpdateProfile(model, User.Identity.Name);
            if (_Service.IsProfileCompleted(User.Identity.Name))
            {
                model.IsProfileCompleted = 1;
            }
            ViewBag.Message = "1";
            return View("seekerManageView", model);
        }

        [Authorize(Roles="recruiter")]
        [HttpPost]
        public ActionResult ManageProfile(RecruiterProfileViewModel model)
        {
            ViewBag.MessageCount = _Service.NoOfUnseenMessages(User.Identity.Name);
            
            if (!ModelState.IsValid)
            {
                ViewBag.Message = "-1";
                return View("recruiterManageView", model);
            }
            _Service.UpdateRecruiterProfile(model, User.Identity.Name);
            if (_Service.IsProfileCompleted(User.Identity.Name))
            {
                model.IsProfileCompleted = 1;
            }
            ViewBag.Message = "1";
            return View("recruiterManageView", model);
        }

        [Authorize]
        public ActionResult Resume(string id=null)
        {
            ViewBag.MessageCount = _Service.NoOfUnseenMessages(User.Identity.Name);
            string filepath=null;
            if (id == null)
            {
                filepath = _Service.GetResumePath(User.Identity.Name);
            }
            else
            {
                if (User.IsInRole("recruiter"))
                {
                    filepath = _Service.GetResumePath(id);
                }
            }
            if (filepath == null)
            {
                return HttpNotFound();
            }
            var filecontent = System.IO.File.ReadAllBytes(System.Web.HttpContext.Current.Server.MapPath(filepath));
            return File(filecontent, "application/pdf");
        }

        [AllowAnonymous]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _Service.emailExist(model.Email);
                    string confirmationToken = WebSecurity.CreateUserAndAccount(model.UserName, model.Password, new { Email = model.Email, IsProfileCompleted = -1, IsConfirmed = -1 }, true);
                    Roles.AddUserToRole(model.UserName, model.TypeOfUser);
                    WebSecurity.ConfirmAccount(confirmationToken);
                    string link = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Authority + "/account/Activate?confirmationToken=" + confirmationToken + "&email=" + model.Email;
                    if (HttpContext.Request.Url.Host != "localhost")
                    {
                        link = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Host + "/account/Activate?confirmationToken=" + confirmationToken + "&email=" + model.Email;
                    }
                    StringBuilder builder = new StringBuilder(100000);
                    builder.Append("<h3><a href=\"http://webrozgar.apphb.com\">WebRozgar.com</a></h3></br></hr>");
                    builder.Append("<p>Hello User</p></br>");
                    builder.Append("<p>Welcome to WebRozgar</p></br>");
                    builder.Append("<p>Thank you for choosing to register for WebRozgar, the upcoming site for jobs. Please click on the activation link below to complete the registration process.<p></br>");
                    builder.Append("<p>Activation Link</p></br>");
                    builder.Append(link);
                    builder.Append("</br>");
                    builder.Append("<p>If this does not work, copy, paste and launch the link in a web browser.<p>");

                    if (_Service.SendMail(model.Email, "WebRozgar Account Activation", builder.ToString()))
                    {
                        return RedirectToAction("EmailVerification", "Account", new { status = "success", email = model.Email });
                    }
                    return RedirectToAction("EmailVerification", "Account", new { status = "fail" });
                }
                catch (MembershipCreateUserException e)
                {
                    ViewData["Error"] = ErrorCodeToString(e.StatusCode);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult EmailVerification(string status, string email)
        {
            ViewBag.Message = status;
            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        public string SendActivationEmail(string email)
        {
            string confirmationToken = _Service.GetConfirmationToken(email);
            if (confirmationToken == null)
            {
                return "Sorry email can't be sent";
            }

            string link = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Authority + "/account/Activate?confirmationToken=" + confirmationToken + "&email=" + email;
            if (HttpContext.Request.Url.Host != "localhost")
            {
                link = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Host + "/account/Activate?confirmationToken=" + confirmationToken + "&email=" + email;
            }       
            StringBuilder builder = new StringBuilder(100000);
            builder.Append("<h3><a href=\"http://webrozgar.apphb.com\">WebRozgar.com</a></h3></br></hr>");
            builder.Append("<p>Hello User</p></br>");
            builder.Append("<p>Welcome to WebRozgar</p></br>");
            builder.Append("<p>Thank you for choosing to register for WebRozgar, the upcoming site for jobs. Please click on the activation link below to complete the registration process.<p></br>");
            builder.Append("<p>Activation Link</p></br>");
            builder.Append(link);
            builder.Append("</br>");
            builder.Append("<p>If this does not work, copy, paste and launch the link in a web browser.<p>");

            if (_Service.SendMail(email, "WebRozgar Account Activation", builder.ToString()))
            {
                return "Email sent successfully!!!!";
            }
            else
            {
                return "Sorry email can't be sent";
            }
        }

        public ActionResult Activate(string confirmationToken, string email)
        {
            ViewBag.status = _Service.Activate(confirmationToken, email);
            return View();
        }

        public JsonResult CheckUsername(String UserName)
        {
            if (UserName == null)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (!WebSecurity.UserExists(UserName))
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public JsonResult CheckEmail(String Email)
        {
            if (Email == null)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (!_Service.EmailExists(Email))
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [Authorize]
        public ActionResult DeleteMe()
        {
            if (Roles.GetRolesForUser(User.Identity.Name).Count() > 0)
            {
                Roles.RemoveUserFromRoles(User.Identity.Name,Roles.GetRolesForUser(User.Identity.Name));
            }
            _Service.DeleteResume(User.Identity.Name);
            WebSecurity.Logout();
            ((SimpleMembershipProvider)Membership.Provider).DeleteAccount(User.Identity.Name);
            ((SimpleMembershipProvider)Membership.Provider).DeleteUser(User.Identity.Name,true);
    //        _Service.DeleteResume(User.Identity.Name);
            return RedirectToAction("Index","Home");
        }


        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
