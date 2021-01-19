using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Graph;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Glasswall.O365.FileHandler.App.Test.Models;

namespace Glasswall.O365.FileHandler.App.Test.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly GraphServiceClient _graphServiceClient;
        private readonly GlasswallSettings _glasswallSettings;

        public HomeController(ILogger<HomeController> logger,
                          GraphServiceClient graphServiceClient,
                          GlasswallSettings glasswallSettings)
        {
             _logger = logger;
            _graphServiceClient = graphServiceClient;
            _glasswallSettings = glasswallSettings;
        }

        [AuthorizeForScopes(ScopeKeySection = "DownstreamApi:Scopes")]
        public async Task<IActionResult> Index()
        {
            var user = await _graphServiceClient.Me.Request().GetAsync();
            ViewData["UserName"] = user.DisplayName;
            ViewData["UserEmail"] = user.Mail;
            ViewData["FileHandlerUrl"] = _glasswallSettings.FileHandlerUrl;

            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
