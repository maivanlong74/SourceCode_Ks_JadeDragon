/*using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Jade_Dragon.common
{
    public class MyAuthorizeAttribute: AuthorizeAttribute
    {

        public string GroupName { set; get; }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var session = (UserLogin)HttpContext.Current.Session["USER_SESSION"];
            if (session == null)
            {
                return false;
            }

            List<string> privilegeLevels = (List<string>)HttpContext.Current.Session["SESSION_GROUP"]; // Call another method to get rights of the user from DB

            if (privilegeLevels.Contains(this.GroupName))
            {
                return true;
            }

            else
            {
                return false;
            }
        }

       


    }
}*/