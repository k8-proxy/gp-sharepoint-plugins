using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Graph;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Glasswall.O365.FileHandler.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication;

namespace Glasswall.O365.FileHandler.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly GraphServiceClient _graphServiceClient;

        public HomeController(ILogger<HomeController> logger,
                          GraphServiceClient graphServiceClient)
        {
            _logger = logger;
            _graphServiceClient = graphServiceClient;
        }

        [AuthorizeForScopes(ScopeKeySection = "DownstreamApi:Scopes")]
        public async Task<IActionResult> Index()
        {
            var model = new HomeModel();
            if (User.Identity.IsAuthenticated)
            {
                var user = await _graphServiceClient.Me.Request().GetAsync();
                model.SignInName = user.Id;
                var userInfo = await _graphServiceClient.Users[user.Id].Request().Select(u => new { u.DisplayName, u.MySite }).GetAsync();
                model.DisplayName = userInfo.DisplayName;
                model.OneDriveUrl = userInfo.MySite;
            }

            return View(model);
        }

        public IActionResult SignInAdmin()
        {
            var scheme = OpenIdConnectDefaults.AuthenticationScheme;
            var redirectUrl = Url.Content("~/");
            var authProps = new AuthenticationProperties { RedirectUri = redirectUrl };
            authProps.SetString("prompt","admin_consent");
            return Challenge(authProps,scheme);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
