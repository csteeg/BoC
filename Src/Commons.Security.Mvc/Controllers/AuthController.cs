using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BoC.Security.Model;
using BoC.Security.Services;
using BoC.Validation;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.RelyingParty;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using JqueryMvc.Mvc;

namespace BoC.Web.Mvc.Controllers
{
    [HandleError]
    public class AuthController : CommonBaseController
    {
        public IFormsAuthentication FormsAuthentication { get; set; }
        private readonly IUserService service;

        // This constructor is not used by the MVC framework but is instead provided for ease
        // of unit testing this type. See the comments at the end of this file for more
        // information.
        public AuthController(IUserService service)
        {
            FormsAuthentication = new FormsAuthenticationWrapper();
            this.service = service;
        }

        public virtual ActionResult OpenId(string openid_identifier, string returnUrl) {
            var openid = new OpenIdRelyingParty();
            var response = openid.GetResponse();
            if (response == null) {
                // Stage 2: user submitting Identifier
                Identifier id;
                if (Identifier.TryParse(openid_identifier, out id)) {
                    try {
                        IAuthenticationRequest req = openid.CreateRequest(openid_identifier);

                        var fetch = new FetchRequest();
                        //ask for more info - the email address
                        var item = new AttributeRequest(WellKnownAttributes.Contact.Email);
                        item.IsRequired=true;
                        fetch.Attributes.Add(item);
                        req.AddExtension(fetch);

                        return req.RedirectingResponse.AsActionResult();
                    } catch (ProtocolException ex) {
                        ViewData.ModelState.AddModelError("openid_identifier", ex);
                        return View("Logon");
                    }
                } else {
                    ViewData.ModelState.AddModelError("openid_identifier", "Invalid identifier");
                    return View("Logon");
                }
            } else {
                // Stage 3: OpenID Provider sending assertion response
                switch (response.Status) {
                    case AuthenticationStatus.Authenticated:

                        var user = service.FindUser(response.ClaimedIdentifier);
                        //checks:
                        if (user != null)
                        {
                            if (user.IsLockedOut)
                            {
                                this.ModelState.AddModelError("_FORM", "Your account is locked out");
                                return View("Logon");
                            }
                            if (!user.IsApproved)
                            {
                                this.ModelState.AddModelError("_FORM", "Your account is not yet approved");
                                return View("Logon");
                            }
                        }


                        var fetch = response.GetExtension<FetchResponse>();
                        string name = response.FriendlyIdentifierForDisplay ?? "";
                        string email = "";

                        if (fetch != null)
                        {
                            IList<string> emailAddresses =
                                fetch.Attributes[WellKnownAttributes.Contact.Email].Values;
                            email = emailAddresses.Count > 0 ? emailAddresses[0] : "";
                        }

                        if (user != null)
                        {
                            //Sync the email from OpenID provider.
                            if (!email.Equals(user.Email, StringComparison.OrdinalIgnoreCase) ||
                                !name.Equals(user.Name, StringComparison.OrdinalIgnoreCase))
                            {
                                user.Email = email;
                                user.Name = name;
                                service.Update(user);
                            }
                        }
                        else
                        {
                            user = new User()
                                       {
                                           Login = response.ClaimedIdentifier,
                                           Email = email,
                                           Name = name
                                       };
                            service.Insert(user);
                        }
                        FormsAuthentication.SignIn(user.Id.ToString(), false);

                        return Redirect(returnUrl ?? VirtualPathUtility.ToAbsolute("~/"));
                    case AuthenticationStatus.Canceled:
                        ViewData.ModelState.AddModelError("openid_identifier", "Canceled at provider");
                        return View("Logon");
                    case AuthenticationStatus.Failed:
                        ViewData.ModelState.AddModelError("openid_identifier", response.Exception);
                        return View("Logon");
                }
            }
            return new EmptyResult();

        }

        public virtual ActionResult LogOn() {

            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings",
            Justification = "Needs to take same parameter type as Controller.Redirect()")]
        public virtual ActionResult LogOn(string userName, string password, bool rememberMe, string returnUrl) {

            if (!ValidateLogOn(userName, password)) {
                return View();
            }

            User user = null;

            try
            {
                user = service.Authenticate(userName, password);
            }
            catch (RulesException rulesException)
            {
                foreach (ErrorInfo info in rulesException.Errors)
                {
                    ModelState.AddModelError("_FORM", info.ErrorMessage);
                }
                return View();
            }

            if (user == null)
            {
                ModelState.AddModelError("_FORM", "The username or password provided is incorrect.");
                return View();
            }

            FormsAuthentication.SignIn(user.Id.ToString(), rememberMe);
            HttpContext.User = user;

            if (!String.IsNullOrEmpty(returnUrl)) {
                return Redirect(returnUrl);
            } else {
                return RedirectToAction("Index", "Home");
            }
        }

        public virtual ActionResult LogOff() {

            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext) {
            if (filterContext.HttpContext.User.Identity is WindowsIdentity) {
                throw new InvalidOperationException("Windows authentication is not supported.");
            }
        }

        #region Validation Methods

        public virtual bool ValidateChangePassword(string currentPassword, string newPassword, string confirmPassword) {
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

        public virtual bool ValidateLogOn(string userName, string password) {
            if (String.IsNullOrEmpty(userName)) {
                ModelState.AddModelError("username", "You must specify a username.");
            }
            if (String.IsNullOrEmpty(password)) {
                ModelState.AddModelError("password", "You must specify a password.");
            }

            return ModelState.IsValid;
        }

        #endregion
    }

    public interface IFormsAuthentication {
        void SignIn(string userName, bool createPersistentCookie);
        void SignOut();
    }

    public class FormsAuthenticationWrapper : IFormsAuthentication {
        public void SignIn(string userName, bool createPersistentCookie) {
            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }
        public void SignOut() {
            FormsAuthentication.SignOut();
        }
    }
}