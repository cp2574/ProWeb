using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TheFamilyFriend.Startup))]
namespace TheFamilyFriend
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            //注册集线器路由  
            app.MapSignalR();
        }
    }
}
