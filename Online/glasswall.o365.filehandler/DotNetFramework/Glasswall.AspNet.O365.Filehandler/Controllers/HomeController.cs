using System;
using System.Web.Mvc;
using System.Threading.Tasks;
using Glasswall.AspNet.O365.Filehandler.Directory;

namespace Glasswall.AspNet.O365.Filehandler.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            Models.HomeModel model = new Models.HomeModel();

            model.SignInName = AuthHelper.GetUserId();
            if (!string.IsNullOrEmpty(model.SignInName))
            {
                try
                {
                    var accessToken = await AuthHelper.GetUserAccessTokenSilentAsync(SettingsHelper.AADGraphResourceId);

                    if (accessToken == null)
                    {
                        return Redirect("/Account/SignIn?force=1");
                    }
                    var response = await UserInfo.GetUserInfoAsync(SettingsHelper.AADGraphResourceId, model.SignInName, accessToken);
                    if (null != response)
                    {
                        model.DisplayName = response.DisplayName;
                        model.OneDriveUrl = response.MySite;
                    }
                    else
                    {
                        model.DisplayName = "Error getting data from Microsoft Graph.";
                    }
                }
                catch (Exception ex)
                {
                    model.DisplayName = ex.ToString();
                }
            }

            return View(model);
        }

        public ActionResult Error(string msg)
        {
            ViewBag.Message = msg;

            return View();
        }
    }
}