using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OkCatStore.Startup))]
namespace OkCatStore
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
