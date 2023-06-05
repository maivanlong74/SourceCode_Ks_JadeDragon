using Jade_Dragon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Jade_Dragon.Areas.Admin.Controllers
{
    public class XacNhanDonManageController : baseController
    {
        private Connect db = new Connect();
        public ActionResult DanhSachDon(long maks)
        {
            hienthiphong ls = new hienthiphong();
            ls.hd = db.HoaDons.Where(m => m.MaKhachSan == maks).ToList();
            ls.cthd = db.ChiTietHoaDons.ToList();
            return View("DanhSachDon", ls);
        }

        public ActionResult XacNhanDon(long mahd)
        {
            HoaDon hd = db.HoaDons.Find(mahd);
            long ma = (long)hd.MaKhachSan;
            if (hd == null)
            {
                return Redirect("~/trangchu/trangchu");
            }
            else
            {
                hd.DaDat = true;
                hd.TrangThaiDon = "Giao dịch thành công";
                db.SaveChanges();
            }
            return RedirectToAction("DanhSachDon", "XacNhanDonManage", new { maks = ma });
        }

        public ActionResult XoaDon(long mahd)
        {
            HoaDon hd = db.HoaDons.Find(mahd);
            long ma = (long)hd.MaHoaDon;
            if (hd == null)
            {
                return Redirect("~/trangchu/tramhchu");
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
            return RedirectToAction("DanhSachDon", "XacNhanDonManage", new { maks = ma });
        }

        public ActionResult XoaChiTietDon(long? mact)
        {
            ChiTietHoaDon ct = db.ChiTietHoaDons.Find(mact);
            long mahoadon = (long)ct.MaHoaDon;
            HoaDon hoadonn = db.HoaDons.Find(mahoadon);
            if (ct == null)
            {
                return Redirect("DanhSachDon");
            }
            else
            {
                db.ChiTietHoaDons.Remove(ct);
                hoadonn.SoLuongPhong = hoadonn.SoLuongPhong - 1;
                db.SaveChanges();
            }
            var cthd = db.ChiTietHoaDons.Where(c => c.MaHoaDon == mahoadon).ToList();
            if (cthd.Count() == 0)
            {
                db.HoaDons.Remove(hoadonn);
                db.SaveChanges();
            }
            return RedirectToAction("DanhSachDon", "XacNhanDonManage", new { maks = hoadonn.MaHoaDon });
        }
    }
}