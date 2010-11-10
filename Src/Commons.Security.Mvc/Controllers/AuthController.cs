using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BoC.Security.Model;
using BoC.Security.Services;
using BoC.Services;
using BoC.Validation;
using BoC.Web.Mvc.Controllers;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using DotNetOpenAuth.OpenId.RelyingParty;

namespace BoC.Security.Mvc.Controllers
{
    [HandleError]
    public class AuthController : CommonBaseController
    {
        public IFormsAuthentication FormsAuthentication { get; set; }
        public IOpenIdRelyingParty RelyingParty { get; private set; }

        private readonly IUserService service;
    	private readonly IModelService<AuthenticationToken> authTokenService;

    	// This constructor is not used by the MVC framework but is instead provided for ease
        // of unit testing this type. See the comments at the end of this file for more
        // information.
        public AuthController(IUserService service, IModelService<AuthenticationToken> authTokenService)
        {
            FormsAuthentication = new FormsAuthenticationWrapper();
            RelyingParty = new OpenIdRelyingPartyService();
            this.service = service;
        	this.authTokenService = authTokenService;
        }

		public virtual ActionResult OpenId(string openid_identifier)
		{
			// Stage 2: user submitting Identifier, this is the non-ajax way and probably not used in the current scenario
			Identifier id;
			if (Identifier.TryParse(openid_identifier, out id))
			{
				try
				{
					var req = RelyingParty.CreateRequest(openid_identifier, Realm.AutoDetect, Request.Url, new Uri(Request.Url, Url.Action("PrivacyStatement")));
					return req.RedirectingResponse.AsActionResult();
				}
				catch (ProtocolException ex)
				{
					ViewData.ModelState.AddModelError("openid_identifier", ex);
					return View("Logon");
				}
			}
			else
			{
				ViewData.ModelState.AddModelError("openid_identifier", "Invalid identifier");
				return View("Logon");
			}
		}
		/// <summary>
		/// Handles the positive assertion that comes from Providers.
		/// </summary>
		/// <param name="openid_openidAuthData">The positive assertion obtained via AJAX.</param>
		/// <returns>The action result.</returns>
		/// <remarks>
		/// This method instructs ASP.NET MVC to <i>not</i> validate input
		/// because some OpenID positive assertions messages otherwise look like
		/// hack attempts and result in errors when validation is turned on.
		/// </remarks>
		[AcceptVerbs(HttpVerbs.Post), ValidateInput(false)]
		public virtual ActionResult OpenId(string openid_openidAuthData, string returnUrl)
		{
			IAuthenticationResponse response;
			if (!string.IsNullOrEmpty(openid_openidAuthData))
			{
				var auth = new Uri(openid_openidAuthData);
				var headers = new WebHeaderCollection();
				foreach (string header in Request.Headers)
				{
					headers[header] = Request.Headers[header];
				}

				// Always say it's a GET since the payload is all in the URL, even the large ones.
				HttpRequestInfo clientResponseInfo = new HttpRequestInfo("GET", auth, auth.PathAndQuery, headers, null);
				response = this.RelyingParty.GetResponse(clientResponseInfo);
			}
			else
			{
				response = this.RelyingParty.GetResponse();
			}

			try
			{
				if (response != null)
				{
					// Stage 3: OpenID Provider sending assertion response
					switch (response.Status)
					{
						case AuthenticationStatus.Authenticated:

							User user = null;
							var authtoken =
								authTokenService.Find(token => token.ClaimedIdentifier == response.ClaimedIdentifier.ToString()).FirstOrDefault();
							if (authtoken != null) user = authtoken.User;
							//checks:
							if (user != null)
							{
								if (user.IsLockedOut)
								{
									this.ModelState.AddModelError("_FORM", "Your account is locked out");
									break;
								}
								if (!user.IsApproved)
								{
									this.ModelState.AddModelError("_FORM", "Your account is not yet approved");
									break;
								}
							}

							var claims = response.GetExtension<ClaimsResponse>();
							var name = claims != null ? claims.FullName ?? "" : "";
							var email = claims != null ? claims.Email ?? "" : "";

							if (user != null)
							{
								//Sync the email from OpenID provider.
								if (string.IsNullOrEmpty(user.Email))
								{
									user.Email = email;
								}
								if (string.IsNullOrEmpty(user.Name))
								{
									user.Name = name;
								}
							}
							else
							{
								user = new User
								       	{
								       		Email = email,
								       		Name = name
								       	};
							}
							user.LastActivity = DateTime.Now;
							user = service.SaveOrUpdate(user);

							var isNew = authtoken == null;
							if (isNew)
							{
								authtoken = new AuthenticationToken
								            	{
								            		ClaimedIdentifier = response.ClaimedIdentifier.ToString(),
								            		User = user
								            	};
							}
							authtoken.FriendlyIdentifier = response.FriendlyIdentifierForDisplay;
							authtoken.LastUsed = DateTime.Now;
							authtoken = authTokenService.SaveOrUpdate(authtoken);

							if (isNew)
							{
								user.AuthenticationTokens.Add(authtoken);
								user = service.SaveOrUpdate(user);
							}

							FormsAuthentication.SignIn(user.Id.ToString(), false);

							return Redirect(returnUrl ?? VirtualPathUtility.ToAbsolute("~/"));
						case AuthenticationStatus.Canceled:
							ViewData.ModelState.AddModelError("openid_identifier", "Canceled at provider");
							break;
						case AuthenticationStatus.Failed:
							ViewData.ModelState.AddModelError("openid_identifier", response.Exception);
							break;
					}
				}
			}
			catch(RulesException rulesException)
			{
				foreach (var error in rulesException.Errors)
				{
					ViewData.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
				}
			}
			// If we're to this point, login didn't complete successfully.
			// Show the LogOn view again to show the user any errors and
			// give another chance to complete login.
			return View("LogOn");
		}

        public virtual ActionResult LogOn()
        {
        	PreloadDiscoveryResults();
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

        /// <summary>
        /// Performs discovery on a given identifier.
        /// </summary>
        /// <param name="identifier">The identifier on which to perform discovery.</param>
        /// <returns>The JSON result of discovery.</returns>
        public ActionResult Discover(string identifier)
        {
            if (!this.Request.IsAjaxRequest())
            {
                throw new InvalidOperationException();
            }

            return this.RelyingParty.AjaxDiscovery(
                identifier,
                Realm.AutoDetect,
                new Uri(Request.Url, Url.Action("PopUpReturnTo")),
                new Uri(Request.Url, Url.Action("PrivacyStatement")));
        }
        
        /// <summary>
        /// Handles the positive assertion that comes from Providers to Javascript running in the browser.
        /// </summary>
        /// <returns>The action result.</returns>
        /// <remarks>
        /// This method instructs ASP.NET MVC to <i>not</i> validate input
        /// because some OpenID positive assertions messages otherwise look like
        /// hack attempts and result in errors when validation is turned on.
        /// </remarks>
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post), ValidateInput(false)]
        public ActionResult PopUpReturnTo()
        {
            return this.RelyingParty.ProcessAjaxOpenIdResponse();
        }

		/// <summary>
		/// Preloads discovery results for the OP buttons we display on the selector in the ViewData.
		/// </summary>
		private void PreloadDiscoveryResults()
		{
			this.ViewData["PreloadedDiscoveryResults"] = this.RelyingParty.PreloadDiscoveryResults(
				Realm.AutoDetect,
				new Uri(Request.Url, Url.Action("PopUpReturnTo")),
                new Uri(Request.Url, Url.Action("PrivacyStatement")),
				"https://me.yahoo.com/",
				"https://www.google.com/accounts/o8/id");
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