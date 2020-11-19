using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security;

namespace Glasswall.AspNet.O365.Filehandler.Controllers
{
    public class AccountController : Controller
    {
        public void SignIn(string force)
        {
            bool forceLogin = false;
            if (!string.IsNullOrEmpty(force) && force == "1") forceLogin = true;
            if (forceLogin || !Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = "/" },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
        }

        public void SignInAdmin()
        {
            if (!Request.IsAuthenticated)
            {
                var authProps = new AuthenticationProperties { RedirectUri = "/" };
                authProps.Dictionary["prompt"] = "admin_consent";
                HttpContext.GetOwinContext().Authentication.Challenge(authProps,
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
        }

        public void SignOut()
        {
            string callbackUrl = Url.Action("SignOutCallback", "Account", routeValues: null, protocol: Request.Url.Scheme);

            HttpContext.GetOwinContext().Authentication.SignOut(
                new AuthenticationProperties { RedirectUri = callbackUrl },
                OpenIdConnectAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);
        }

        public ActionResult SignOutCallback()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
    }
}
