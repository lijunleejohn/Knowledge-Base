using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GoogleOauth2.Startup))]
namespace GoogleOauth2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
