using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProductionWIP.Startup))]
namespace ProductionWIP
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
