using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BBDEVSYS.Startup))]
namespace BBDEVSYS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
