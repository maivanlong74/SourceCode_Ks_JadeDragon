using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.EMMA;
using Jade_Dragon.common;
using Jade_Dragon.Models;

namespace Jade_Dragon.Areas.Admin.Controllers
{
    public class QLKhachHangController : baseController
    {
        private Connect db = new Connect();

        // GET: Admin/QLKhachHang
        public ActionResult QuanLyKh()
        {
            var khachhangs = db.NguoiDungs.Include(k => k.PhanQuyen);
            var ListKh = db.NguoiDungs.ToList();
            return View("QuanLyKh", ListKh);
        }

        public ActionResult QuanLyKhManage()
        {
            var khachhangs = db.NguoiDungs.Include(k => k.PhanQuyen);
            var ListKh = db.NguoiDungs.Where(m => m.MaPhanQuyen == 2 || m.MaPhanQuyen == 3).ToList();
            return View("QuanLyKhManage", ListKh);
        }

        // GET: Admin/QLKhachHang/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NguoiDung nguoiDung = db.NguoiDungs.Find(id);
            if (nguoiDung == null)
            {
                return HttpNotFound();
            }
            return View(nguoiDung);
        }

        // GET: Admin/Test/Create
        public ActionResult Create()
        {
            ViewBag.KhachSans = new List<SelectListItem>
            {
                new SelectListItem { Text = "Không chọn khách sạn", Value = "" }
            }.Concat(db.KhachSans.Select(k => new SelectListItem { Text = k.TenKhachSan, Value = k.MaKhachSan.ToString() }));

            ViewBag.MaKhachSan = new SelectList(db.KhachSans, "MaKhachSan", "TenKhachSan");
            ViewBag.MaPhanQuyen = new SelectList(db.PhanQuyens, "MaPhanQuyen", "TenQuyen");
            return View();
        }

        // POST: Admin/Test/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaNguoiDung,HoTen,SoDienThoai,CMND,DiaChi,Gmail,Avt,TenDangNhap,MatKhau,MaKhachSan,MaPhanQuyen,Code,DaXacMinh")] NguoiDung nguoiDung, HttpPostedFileBase uploadhinh)
        {
            if (ModelState.IsValid)
            {
                db.NguoiDungs.Add(nguoiDung);
                nguoiDung.DaXacMinh = true;
                nguoiDung.MatKhau = GetMD5(nguoiDung.MatKhau);
                db.SaveChanges();
                Up_IMG(nguoiDung, uploadhinh);
                return RedirectToAction("QuanLyKh");
            }

            ViewBag.IDGroup = new SelectList(db.PhanQuyens, "MaPhanQuyen", "TenQuyen", nguoiDung.MaPhanQuyen);
            return View(nguoiDung);
        }

        // GET: Admin/Test/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NguoiDung nguoiDung = db.NguoiDungs.Find(id);
            if (nguoiDung == null)
            {
                return HttpNotFound();
            }
            ViewBag.KhachSans = new List<SelectListItem>
            {
                new SelectListItem { Text = "Không chọn khách sạn", Value = "" }
            }.Concat(db.KhachSans.Select(k => new SelectListItem { Text = k.TenKhachSan, Value = k.MaKhachSan.ToString() }));
            ViewBag.MaPhanQuyen = new SelectList(db.PhanQuyens, "MaPhanQuyen", "TenQuyen", nguoiDung.MaPhanQuyen);
            Session["HoTen2"] = nguoiDung.HoTen;
            return View(nguoiDung);
        }

        // POST: Admin/Test/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaNguoiDung,HoTen,SoDienThoai,CMND,DiaChi,Gmail,Avt,TenDangNhap,MatKhau,MaKhachSan,MaPhanQuyen,Code,DaXacMinh")] NguoiDung nguoiDung, HttpPostedFileBase uploadhinh)
        {
            if (ModelState.IsValid)
            {
                /*db.Entry(khachhang).State = EntityState.Modified;*/

                NguoiDung unv = db.NguoiDungs.FirstOrDefault(x => x.MaNguoiDung == nguoiDung.MaNguoiDung);

                unv.HoTen = nguoiDung.HoTen;
                unv.SoDienThoai = nguoiDung.SoDienThoai;
                unv.CMND = nguoiDung.CMND;
                unv.DiaChi = nguoiDung.DiaChi;
                unv.Gmail = nguoiDung.Gmail;
                unv.TenDangNhap = nguoiDung.TenDangNhap;
                unv.MaPhanQuyen = nguoiDung.MaPhanQuyen;
                unv.MaKhachSan = nguoiDung.MaKhachSan;

                if (unv.MatKhau != nguoiDung.MatKhau)
                {
                    unv.MatKhau = GetMD5(nguoiDung.MatKhau);
                }

                unv.DaXacMinh = nguoiDung.DaXacMinh;

                if (uploadhinh != null && uploadhinh.ContentLength > 0)
                {
                    long id = nguoiDung.MaNguoiDung;

                    string _FileName = "";
                    string code = RandomCode();
                    int index = uploadhinh.FileName.IndexOf('.');
                    _FileName = "nv" + code + "." + uploadhinh.FileName.Substring(index + 1);
                    string _path = Path.Combine(Server.MapPath("~/UpLoad_Img/KhachHang"), _FileName);
                    uploadhinh.SaveAs(_path);
                    unv.Avt = _FileName;
                }

                db.SaveChanges();
                return Redirect("~/Admin/QLKhachHang/Details/" + nguoiDung.MaNguoiDung);
            }
            ViewBag.IDGroup = new SelectList(db.PhanQuyens, "MaPhanQuyen", "TenQuyen", nguoiDung.MaPhanQuyen);
            return View(nguoiDung);
        }

        // GET: Admin/QLKhachHang/Delete/5
        public ActionResult Delete(long id)
        {
            List<TinNhanNhom> tn = db.TinNhanNhoms.Where(m => m.MaNguoiDung == id).ToList();
            List<TinNhanNguoiDung> tnAdmin = db.TinNhanNguoiDungs.Where(n => n.IdNguoiGui == id || n.IdNguoiNhan == id).ToList();
            List<HoaDon> hd = db.HoaDons.Where(c => c.MaNguoiDung == id).ToList();
            List<DanhGiaKhachSan> so = db.DanhGiaKhachSans.Where(b => b.MaNguoiDung == id).ToList();

            if (tn != null)
            {
                foreach (var i in tn)
                {
                    db.TinNhanNhoms.Remove(i);
                }
            }
            if (tnAdmin != null)
            {
                foreach (var o in tnAdmin)
                {
                    db.TinNhanNguoiDungs.Remove(o);
                }
            }
            if (hd != null)
            {
                foreach (var j in hd)
                {
                    j.MaNguoiDung = null;
                }
            }
            if (so != null)
            {
                foreach (var p in so)
                {
                    db.DanhGiaKhachSans.Remove(p);
                }
            }
            db.SaveChanges();

            NguoiDung kh = db.NguoiDungs.FirstOrDefault(x => x.MaNguoiDung == id);
            db.NguoiDungs.Remove(kh);
            db.SaveChanges();
            return RedirectToAction("QuanLyKh");
        }

        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }

        [HttpPost]
        public ActionResult Up_IMG(NguoiDung kh, HttpPostedFileBase uploadhinh)
        {
            if (uploadhinh != null && uploadhinh.ContentLength > 0)
            {
                int id = int.Parse(db.NguoiDungs.ToList().Last().MaNguoiDung.ToString());

                string _FileName = "";
                string code = RandomCode();
                int index = uploadhinh.FileName.IndexOf('.');
                _FileName = "nv" + code + "." + uploadhinh.FileName.Substring(index + 1);
                string _path = Path.Combine(Server.MapPath("~/UpLoad_Img/KhachHang"), _FileName);
                uploadhinh.SaveAs(_path);

                NguoiDung unv = db.NguoiDungs.FirstOrDefault(x => x.MaNguoiDung == id);
                unv.Avt = _FileName;
                db.SaveChanges();
            }
            return View();
        }

        public string RandomCode()
        {
            int codeLength = 4; // Độ dài của mã xác minh
            Random random = new Random();
            string code = "";

            for (int i = 0; i < codeLength; i++)
            {
                int digit = random.Next(0, 9); // Lấy ngẫu nhiên một số từ 0 đến 9
                code += digit.ToString(); // Thêm số vào chuỗi mã xác minh
            }

            return code;
        }

        public ActionResult XoaAnh(long? kh)
        {
            NguoiDung khachhang = db.NguoiDungs.Find(kh);
            khachhang.Avt = null;
            db.SaveChanges();
            return Redirect("~/Admin/QLKhachHang/Details/" + khachhang.MaNguoiDung);
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
