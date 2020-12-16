using System;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Glasswall.AspNet.O365.Filehandler.Controllers;
using Glasswall.AspNet.O365.Filehandler.Utils;
using Glasswall.AspNet.O365.Filehandler.Models;

namespace Glasswall.AspNet.O365.Filehandler
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions { });

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = SettingsHelper.ClientId,
                    Authority = SettingsHelper.Authority,
                    ClientSecret = SettingsHelper.AppKey,
                    ResponseType = "code id_token",
                    Resource = "https://graph.microsoft.com",
                    PostLogoutRedirectUri = "/",
                    TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = false
                    },
                    Notifications = new OpenIdConnectAuthenticationNotifications()
                    {
                        SecurityTokenValidated = (context) =>
                        {
                            return Task.FromResult(0);
                        },
                        AuthenticationFailed = (context) =>
                        {
                            string message = Uri.EscapeDataString(context.Exception.Message);
                            context.OwinContext.Response.Redirect("/Home/Error?msg=" + message);
                            context.HandleResponse();
                            return Task.FromResult(0);
                        },
                        AuthorizationCodeReceived = async (context) =>
                        {
                            var code = context.Code;
                            ClientCredential credential = new ClientCredential(SettingsHelper.ClientId, SettingsHelper.AppKey);

                            string tenantID = context.AuthenticationTicket.Identity.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
                            string signInUserId = context.AuthenticationTicket.Identity.FindFirst(AuthHelper.ObjectIdentifierClaim).Value;

                            var authContext = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext(SettingsHelper.Authority,
                                new AzureTableTokenCache(signInUserId));

                            AuthenticationResult result = await authContext.AcquireTokenByAuthorizationCodeAsync(code,
                                new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path)),
                                credential,
                                SettingsHelper.AADGraphResourceId);
                        },
                        RedirectToIdentityProvider = (context) =>
                        {
                            string appBaseUrl = context.Request.Scheme + "://" + context.Request.Host + context.Request.PathBase;
                            context.ProtocolMessage.RedirectUri = appBaseUrl + "/";
                            context.ProtocolMessage.PostLogoutRedirectUri = appBaseUrl;

                            FileHandlerActivationParameters fileHandlerActivation;
                            if (FileHandlerController.IsFileHandlerActivationRequest(new HttpRequestWrapper(HttpContext.Current.Request), out fileHandlerActivation))
                            {
                                context.ProtocolMessage.LoginHint = fileHandlerActivation.UserId;
                                context.ProtocolMessage.DomainHint = "organizations";
                                CookieStorage.Save(HttpContext.Current.Request.Form, HttpContext.Current.Response);
                            }

                            var challengeProperties = context.OwinContext?.Authentication?.AuthenticationResponseChallenge?.Properties;
                            if (null != challengeProperties && challengeProperties.Dictionary.ContainsKey("prompt"))
                            {
                                context.ProtocolMessage.Prompt = challengeProperties.Dictionary["prompt"];
                            }

                            return Task.FromResult(0);
                        }
                    }
                });
        }
    }
}
