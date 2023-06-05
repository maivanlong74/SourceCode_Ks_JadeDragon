using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jade_Dragon.common;
using Jade_Dragon.Models;

namespace Jade_Dragon.Areas.Admin.Controllers
{
    public class XacNhanDonAdminController : baseController
    {
        private Connect db = new Connect();
        // GET: Admin/XacNhanDonAdmin
        public ActionResult DanhSachDon()
        {
            hienthiphong ls = new hienthiphong();
            ls.hd = db.HoaDons.ToList();
            ls.cthd = db.ChiTietHoaDons.ToList();
            return View("DanhSachDon", ls);
        }

        public ActionResult XacNhanDon(long mahd)
        {
            HoaDon hd = db.HoaDons.Find(mahd);
            if (hd == null)
            {
                return Redirect("DanhSachDon");
            }
            else
            {
                hd.DaDat = true;
                hd.TrangThaiDon = "Giao dịch thành công";
                db.SaveChanges();
            }
            return Redirect("DanhSachDon");
        }

        public ActionResult XoaDon(long mahd)
        {
            HoaDon hd = db.HoaDons.Find(mahd);
            if (hd == null)
            {
                return Redirect("DanhSachDon");
            }
            else
            {
                var ct = db.ChiTietHoaDons.Where(m => m.MaHoaDon == mahd).ToList();
                if (ct.Any())
                {
                    foreach (var m in ct)
                    {
                        db.ChiTietHoaDons.Remove(m);
                    }
                    db.SaveChanges();
                }
                db.HoaDons.Remove(hd);
            }
            db.SaveChanges();
            return Redirect("DanhSachDon");
        }

        public ActionResult XoaChiTietDon(long? mact)
        {
            ChiTietHoaDon ct = db.ChiTietHoaDons.Find(mact);
            long mahoadon = (long)ct.MaHoaDon;
            HoaDon hd = db.HoaDons.Find(mahoadon);
            if (ct == null)
            {
                return Redirect("DanhSachDon");
            }
            else
            {
                db.ChiTietHoaDons.Remove(ct);
                hd.SoLuongPhong = hd.SoLuongPhong - 1;
                db.SaveChanges();
            }
            var cthd = db.ChiTietHoaDons.Where(c => c.MaHoaDon == mahoadon).ToList();
            if (cthd.Count() == 0)
            {
                db.HoaDons.Remove(hd);
                db.SaveChanges();
            }
            return Redirect("DanhSachDon");
        }
    }
}