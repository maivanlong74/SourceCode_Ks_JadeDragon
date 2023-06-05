using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Jade_Dragon.Startup))]
namespace Jade_Dragon
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
