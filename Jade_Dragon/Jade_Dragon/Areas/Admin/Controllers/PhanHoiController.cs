using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Jade_Dragon.Models;

namespace Jade_Dragon.Areas.Admin.Controllers
{
    public class PhanHoiController : baseController
    {
        private Connect db = new Connect();

        // GET: Admin/PhanHoi
        public ActionResult PhanHoi()
        {
            var listkh = db.NguoiDungs.Select(kh => kh.MaNguoiDung).ToList();
            var listph = db.BinhLuans.Include(k => k.NguoiDung).ToList();

            foreach (var item in listph)
            {
                if (item.MaNguoiDung != null && !listkh.Contains(item.MaNguoiDung.Value))
                {
                    item.MaNguoiDung = null;
                }
            }
            db.SaveChanges();

            return View("PhanHoi", listph);
        }
        // GET: Admin/PhanHoi/Delete/5
        public ActionResult Delete(long? id)
        {
            BinhLuan ph = db.BinhLuans.FirstOrDefault(x => x.MaBinhLuan == id);
            if (ph != null)
            {
                db.BinhLuans.Remove(ph);
            }
            db.SaveChanges();
            return RedirectToAction("PhanHoi");
        }

        public ActionResult PhanHoiManage(long? maks)
        {
            var listkh = db.NguoiDungs.Select(kh => kh.MaNguoiDung).ToList();
            var listph = db.BinhLuans.Where(l => l.MaKhachSan == maks).ToList();

            foreach (var item in listph)
            {
                if (item.MaNguoiDung != null && !listkh.Contains(item.MaNguoiDung.Value))
                {
                    item.MaNguoiDung = null;
                }
            }
            db.SaveChanges();

            return View("PhanHoiManage", listph);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
