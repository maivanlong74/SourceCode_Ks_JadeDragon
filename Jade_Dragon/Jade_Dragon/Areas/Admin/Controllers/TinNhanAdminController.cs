using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jade_Dragon.Models;

namespace Jade_Dragon.Areas.Admin.Controllers
{
    public class TinNhanAdminController : baseController
    {
        private Connect db = new Connect();
        // GET: Admin/TinNhanAdmin
        public ActionResult TinNhanAdmin()
        {
            var phong = db.PhongChats.FirstOrDefault(); // lấy ra phòng đầu tiên trong danh sách
            if (phong != null) // kiểm tra nếu danh sách phòng không rỗng
            {
                Session["MaPhong"] = phong.MaPhongChat; // lưu mã phòng vào session "MaPhong"
                Session["TenPhong"] = phong.TenPhongChat; // lưu tên phòng vào session "TenPhong"
            }
            Chat_Admin chat = new Chat_Admin();
            chat.kh = db.NguoiDungs.Where(n => n.MaPhanQuyen == 2 || n.MaPhanQuyen == 3).ToList();
            chat.ks = db.KhachSans.ToList();
            return View(chat);
        }

        public ActionResult DeletePhong(long? id)
        {
            PhongChat phong = db.PhongChats.Find(id);
            if (phong != null)
            {
                var tn = db.TinNhanNhoms.Where(m => m.MaPhongChat == id).ToList();
                if (tn.Count > 0)
                {
                    foreach (var n in tn)
                    {
                        db.TinNhanNhoms.Remove(n);
                        db.SaveChanges();
                    }
                }
                db.PhongChats.Remove(phong);
            }
            db.SaveChanges();
            return Redirect("TinNhanAdmin");
        }

        public ActionResult TinNhanManage()
        {
            var phong = db.PhongChats.FirstOrDefault(); // lấy ra phòng đầu tiên trong danh sách
            if (phong != null) // kiểm tra nếu danh sách phòng không rỗng
            {
                Session["MaPhong"] = phong.MaPhongChat; // lưu mã phòng vào session "MaPhong"
                Session["TenPhong"] = phong.TenPhongChat; // lưu tên phòng vào session "TenPhong"
            }
            Chat_Admin chat = new Chat_Admin();
            chat.kh = db.NguoiDungs.Where(n => n.MaPhanQuyen == 1).ToList();
            return View(chat);
        }
        public ActionResult DeletePhongManage(long? id)
        {
            PhongChat phong = db.PhongChats.Find(id);
            if (phong != null)
            {
                var tn = db.TinNhanNhoms.Where(m => m.MaPhongChat == id).ToList();
                if (tn.Count > 0)
                {
                    foreach (var n in tn)
                    {
                        db.TinNhanNhoms.Remove(n);
                        db.SaveChanges();
                    }
                }
                db.PhongChats.Remove(phong);
            }
            db.SaveChanges();
            return Redirect("TinNhanManage");
        }
    }
}