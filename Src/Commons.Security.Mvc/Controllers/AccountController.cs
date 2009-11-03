using System;
using System.Globalization;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Security;
using BoC.Security.Model;
using BoC.Security.Services;
using BoC.Web.Mvc.Controllers;
using JqueryMvc.Mvc;

namespace BoC.Security.Mvc.Controllers
{
    [HandleError]
    public class AccountController : CommonBaseController
    {
        private readonly IUserService service;

        // This constructor is not used by the MVC framework but is instead provided for ease
        // of unit testing this type. See the comments at the end of this file for more
        // information.
        public AccountController(IUserService service)
        {
            this.service = service;
        }

        public ActionResult Register()
        {
            ViewData["PasswordLength"] = service.MinRequiredPasswordLength;

            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Register(User user, string password, string confirmPassword) 
        {
            ViewData["PasswordLength"] = service.MinRequiredPasswordLength;

            if (ValidateRegistration(user.Login, user.Email, password, confirmPassword)) {
                // Attempt to register the user
                try
                {
                    user = service.CreateUser(user, password);
                    if (user != null)
                    {
                        return View("RegisterSuccess");
                    }
                }
                catch (Exception exc)
                {
                    this.ModelState.AddModelError("_FORM", exc.Message);
                }
            }

            // If we got this far, something failed, redisplay form
            return View();
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            ViewData["PasswordLength"] = service.MinRequiredPasswordLength;

            return View();
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exceptions result in password not being changed.")]
        public ActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            ViewData["PasswordLength"] = service.MinRequiredPasswordLength;

            if (!ValidateChangePassword(currentPassword, newPassword, confirmPassword)) {
                return View();
            }

            try {
                if (service.ChangePassword(User.Identity.Name, currentPassword, newPassword)) {
                    return RedirectToAction("ChangePasswordSuccess");
                } else {
                    ModelState.AddModelError("_FORM", "The current password is incorrect or the new password is invalid.");
                    return View();
                }
            } catch {
                ModelState.AddModelError("_FORM", "The current password is incorrect or the new password is invalid.");
                return View();
            }
        }

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext) {
            if (filterContext.HttpContext.User.Identity is WindowsIdentity) {
                throw new InvalidOperationException("Windows authentication is not supported.");
            }
        }

        #region Validation Methods

        protected bool ValidateChangePassword(string currentPassword, string newPassword, string confirmPassword) {
            if (String.IsNullOrEmpty(currentPassword)) {
                ModelState.AddModelError("currentPassword", "You must specify a current password.");
            }
            if (newPassword == null || newPassword.Length < service.MinRequiredPasswordLength) {
                ModelState.AddModelError("newPassword",
                                         String.Format(CultureInfo.CurrentCulture,
                                                       "You must specify a new password of {0} or more characters.",
                                                       service.MinRequiredPasswordLength));
            }

            if (!String.Equals(newPassword, confirmPassword, StringComparison.Ordinal)) {
                ModelState.AddModelError("_FORM", "The new password and confirmation password do not match.");
            }

            return ModelState.IsValid;
        }

        protected bool ValidateRegistration(string userName, string email, string password, string confirmPassword) {
            if (String.IsNullOrEmpty(userName)) {
                ModelState.AddModelError("Login", "You must specify a username.");
            }
            if (String.IsNullOrEmpty(email)) {
                ModelState.AddModelError("email", "You must specify an email address.");
            }
            if (password == null || password.Length < service.MinRequiredPasswordLength)
            {
                ModelState.AddModelError("password",
                                         String.Format(CultureInfo.CurrentCulture,
                                                       "You must specify a password of {0} or more characters.",
                                                       service.MinRequiredPasswordLength));
            }
            if (!String.Equals(password, confirmPassword, StringComparison.Ordinal)) {
                ModelState.AddModelError("_FORM", "The new password and confirmation password do not match.");
            }
            return ModelState.IsValid;
        }

        #endregion
    }
}