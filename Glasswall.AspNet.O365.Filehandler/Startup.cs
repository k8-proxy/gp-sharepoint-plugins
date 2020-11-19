using Owin;

namespace Glasswall.AspNet.O365.Filehandler
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
