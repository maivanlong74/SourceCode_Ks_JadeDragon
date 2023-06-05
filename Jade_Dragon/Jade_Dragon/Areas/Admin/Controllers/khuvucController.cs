using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.EMMA;
using Jade_Dragon.Models;

namespace Jade_Dragon.Areas.Admin.Controllers
{
    public class khuvucController : baseController
    {
        private Connect db = new Connect();

        // GET: Admin/khuvuc
        public ActionResult khuvuc()
        {
            List<KhachSan> Ks = new List<KhachSan>();
            Ks = db.KhachSans.ToList();
            ViewBag.ListKs = Ks;
            var ListKv = db.KhuVucs.ToList();
            return View("khuvuc", ListKv);
        }

        // GET: Admin/khuvuc/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KhuVuc khuvuc = db.KhuVucs.Find(id);
            if (khuvuc == null)
            {
                return HttpNotFound();
            }
            return View(khuvuc);
        }

        // GET: Admin/khuvuc/Create
        public ActionResult Create(KhuVuc khuvuc, string TenKhuVuc, string DiaChi, double KinhDo, double ViDo)
        {
            KhuVuc kv = db.KhuVucs.FirstOrDefault(m => m.TenKhuVuc == TenKhuVuc);
            if (kv != null)
            {
                kv.TenKhuVuc = TenKhuVuc;
                kv.KinhDo = KinhDo.ToString();
                kv.ViDo = ViDo.ToString();
            }
            else
            {
                khuvuc.TenKhuVuc = TenKhuVuc;
                khuvuc.KinhDo = KinhDo.ToString();
                khuvuc.ViDo = ViDo.ToString();
                db.KhuVucs.Add(khuvuc);
            }

            db.SaveChanges();
            return Redirect("khuvuc");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string TenKhuVuc, long MaKhuVuc)
        {
            if (Request.Cookies["TenKhuVuc"] != null)
            {
                TenKhuVuc = Request.Cookies["TenKhuVuc"].Value;
                MaKhuVuc = long.Parse(Request.Cookies["MaKhuVuc"].Value);
            }
            var khuVuc = db.KhuVucs.FirstOrDefault(x => x.MaKhuVuc == MaKhuVuc);

            if (db.KhuVucs.Where(m => m.TenKhuVuc == TenKhuVuc).Count() > 0)
            {
                return Redirect("khuvuc");
            }
            khuVuc.TenKhuVuc = TenKhuVuc;
            db.SaveChanges();
            return Redirect("khuvuc");

        }

        public ActionResult Delete(long? id)
        {
            KhuVuc kv = db.KhuVucs.FirstOrDefault(x => x.MaKhuVuc == id);
            KhachSan ks = db.KhachSans.FirstOrDefault(x => x.MaKhuVuc == id);
            db.KhuVucs.Remove(kv);
            if (ks != null)
            {
                db.KhachSans.Remove(ks);
                PhongKhachSan ph = db.PhongKhachSans.FirstOrDefault(x => x.MaKhachSan == ks.MaKhachSan);
                if (ph != null)
                {
                    db.PhongKhachSans.Remove(ph);
                }
            }
            db.SaveChanges();
            return RedirectToAction("khuvuc");
        }

        public JsonResult Search(string search)
        {
            var results = db.KhuVucs
                .Where(khuvuc => khuvuc.TenKhuVuc.Contains(search))
                .Select(khuvuc => new { khuvuc.TenKhuVuc, khuvuc.KinhDo, khuvuc.ViDo })
                .ToList();
            return Json(results, JsonRequestBehavior.AllowGet);
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
