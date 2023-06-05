using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Jade_Dragon.common;

namespace Jade_Dragon.Areas.Admin.Controllers
{
    public class baseController : Controller
    {
        // GET: Admin/base
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (Session["USER_SESSION"] == null && Session["SESSION_GROUP"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(new
                    RouteValueDictionary(new { controller = "Account", action = "Login", Area = "Admin" }));
            }
            else
            {
                List<string> privilegeLevels = (List<string>)Session["SESSION_GROUP"];
                if (privilegeLevels[0].CompareTo("Client") == 0)
                {
                    WebMsgBox.Show("Bạn cần đăng nhập tài khoản Admin", this);
                    filterContext.Result = new RedirectToRouteResult(new
                    RouteValueDictionary(new { controller = "Account", action = "Login", Area = "Admin" }));
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}