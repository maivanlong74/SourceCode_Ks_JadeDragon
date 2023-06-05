using System.Web.Mvc;

namespace Jade_Dragon.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new {Controller="TrangChuAdmin", action = "TrangChu", id = UrlParameter.Optional },
                namespaces: new[] { "Jade_Dragon.Areas.Admin.Controllers" }
            );
        }
    }
}