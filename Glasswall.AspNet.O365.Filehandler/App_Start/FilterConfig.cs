using System.Web.Mvc;

namespace Glasswall.AspNet.O365.Filehandler
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
