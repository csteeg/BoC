using System;
using System.Globalization;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Security;
using BoC.Security.Model;
using BoC.Security.Mvc.ViewModels;
using BoC.Security.Services;
using BoC.Validation;
using BoC.Web.Mvc.Controllers;

namespace BoC.Security.Mvc.Controllers
{
    [HandleError]
    public class AccountController : CommonBaseController
    {
        private readonly IModelValidator modelValidator;
        private readonly IUserService service;

        public AccountController(IModelValidator modelValidator, IUserService service)
        {
            this.modelValidator = modelValidator;
            this.service = service;
        }

        public ActionResult Register()
        {
            ViewModel.PasswordLength = service.MinRequiredPasswordLength;

            return View(new RegisterModel());
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Register(RegisterModel registration)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    var user = service.CreateUser(new User
                                                      {
                                                          Email = registration.Email,
                                                          Login = registration.UserName,
                                                      }, registration.Password);
                    if (user != null)
                    {
                        return View("RegisterSuccess");
                    }
                }
                catch (Exception exc)
                {
                    this.ModelState.AddModelError("", exc.Message);
                }
            }

            // If we got this far, something failed, redisplay form
            ViewModel.PasswordLength = service.MinRequiredPasswordLength;

            return Register(registration);
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

        #endregion
    }
}