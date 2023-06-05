using Jade_Dragon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Jade_Dragon.Areas.Admin.Controllers
{
    public class TrangChuAdminController : baseController
    {
        private Connect db = new Connect();
        // GET: Admin/TrangChuAdmin
        public ActionResult TrangChu()
        {
            return View();
        }

        public ActionResult TrangChuManage()
        {
            return View();
        }
    }
}