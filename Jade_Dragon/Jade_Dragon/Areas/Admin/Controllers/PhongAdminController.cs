using Jade_Dragon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jade_Dragon.common;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;

namespace Jade_Dragon.Areas.Admin.Controllers
{
    public class PhongAdminController : baseController
    {
        private Connect db = new Connect();
        public ActionResult DanhSachPhong(long MaKs)
        {
            hienthiphong ph = new hienthiphong();
            InfoPhong infophong = new InfoPhong();
            var phks = db.PhongKhachSans.Where(a => a.MaKhachSan == MaKs).ToList();
            foreach (var phk in phks)
            {
                ChiTietHoaDon ct = db.ChiTietHoaDons.FirstOrDefault(s => s.MaPhong == phk.MaPhong);
                if (ct != null)
                {
                    phk.TrangThai = false; db.SaveChanges();
                }
                else
                {
                    phk.TrangThai = true; db.SaveChanges();
                }
            }

            ph.sotang = infophong.LayDanhSachSoTang(MaKs);
            ph.ph = phks;
            ph.cthd = db.ChiTietHoaDons.ToList();
            KhachSan ks = db.KhachSans.Find(MaKs);

            ViewBag.ma = ks.MaKhachSan;
            ViewBag.ten = ks.TenKhachSan;
            ViewBag.time = DateTime.Now;

            return View("DanhSachPhong", ph);
        }

        public ActionResult khoaphong(long maph)
        {
            PhongKhachSan ph = db.PhongKhachSans.Find(maph);
            ph.KhoaPhong = true;
            db.SaveChanges();
            return RedirectToAction("DanhSachPhong", "PhongAdmin", new { MaKs = ph.MaKhachSan });
        }

        public ActionResult mokhoaphong(long maph)
        {
            PhongKhachSan ph = db.PhongKhachSans.Find(maph);
            ph.KhoaPhong = false;
            db.SaveChanges();
            return RedirectToAction("DanhSachPhong", "PhongAdmin", new { MaKs = ph.MaKhachSan });
        }

        public ActionResult themphong(long maks, long matang)
        {
            KhachSan ks = db.KhachSans.Find(maks);
            var ph = db.PhongKhachSans.Where(m => m.MaKhachSan == maks && m.MaSoTang == matang).ToList();
            SoTangKhachSan tang = db.SoTangKhachSans.Find(matang);
            SoPhongKhachSan sophong = db.SoPhongKhachSans.Find(ph.Count + 1);
            if (sophong != null)
            {
                PhongKhachSan phks = new PhongKhachSan();
                phks.MaKhachSan = maks;
                phks.MaSoTang = matang;
                phks.MaSoPhong = ph.Count + 1;
                phks.TenPhong = tang.SoTang.Replace(" ", "") + sophong.SoPhong.Replace(" ", "");
                phks.LoaiHinh = "Đơn";
                phks.Gia = ks.Gia;
                phks.VIP = false;
                phks.TrangThai = false;
                phks.KhoaPhong = true;
                db.PhongKhachSans.Add(phks);
                db.SaveChanges();
            }
            else
            {
                SoPhongKhachSan sl = new SoPhongKhachSan();
                sl.MaSoPhong = ph.Count + 1;
                sl.SoPhong = "0" + ((ph.Count + 1).ToString().Replace(" ", ""));
                db.SoPhongKhachSans.Add(sl);
                db.SaveChanges();

                SoPhongKhachSan soks = db.SoPhongKhachSans.Find(ph.Count + 1);
                PhongKhachSan phks = new PhongKhachSan();
                phks.MaKhachSan = maks;
                phks.MaSoTang = matang;
                phks.MaSoPhong = soks.MaSoPhong;
                phks.TenPhong = tang.SoTang.Replace(" ", "") + soks.SoPhong.Replace(" ", "");
                phks.LoaiHinh = "Đơn";
                phks.Gia = ks.Gia;
                phks.VIP = false;
                phks.TrangThai = false;
                phks.KhoaPhong = true;
                db.PhongKhachSans.Add(phks);
                db.SaveChanges();
            }
            db.SaveChanges();
            return RedirectToAction("DanhSachPhong", "PhongAdmin", new { MaKs = maks });
        }

        public ActionResult themtang(long maks)
        {
            KhachSan ks = db.KhachSans.Find(maks);
            SoTangKhachSan sotang = db.SoTangKhachSans.Find(ks.SoTang + 1);
            SoPhongKhachSan sophong = db.SoPhongKhachSans.Find(1);
            if (sotang != null)
            {
                PhongKhachSan phks = new PhongKhachSan();
                phks.MaKhachSan = maks;
                phks.MaSoTang = (ks.SoTang + 1);
                phks.MaSoPhong = 1;
                phks.TenPhong = sotang.SoTang.Replace(" ", "") + sophong.SoPhong.Replace(" ", "");
                phks.LoaiHinh = "Đơn";
                phks.Gia = ks.Gia;
                phks.VIP = false;
                phks.TrangThai = false;
                phks.KhoaPhong = true;
                db.PhongKhachSans.Add(phks);
                db.SaveChanges();
            }
            else
            {
                SoTangKhachSan themtangks = new SoTangKhachSan();
                themtangks.MaSoTang = (long)(ks.SoTang + 1);
                themtangks.SoTang = ("B" + (ks.SoTang + 1).ToString()).Replace(" ", "");
                db.SoTangKhachSans.Add(themtangks);
                db.SaveChanges();

                SoTangKhachSan sotangks = db.SoTangKhachSans.Find(ks.SoTang + 1);
                PhongKhachSan phks = new PhongKhachSan();
                phks.MaKhachSan = maks;
                phks.MaSoTang = sotangks.MaSoTang;
                phks.MaSoPhong = 1;
                phks.TenPhong = sotangks.SoTang.Replace(" ", "") + sophong.SoPhong.Replace(" ", "");
                phks.LoaiHinh = "Đơn";
                phks.Gia = ks.Gia;
                phks.VIP = false;
                phks.TrangThai = false;
                phks.KhoaPhong = true;
                db.PhongKhachSans.Add(phks);
                db.SaveChanges();
            }

            long up = (long)(ks.SoTang + 1);
            ks.SoTang = up;
            db.SaveChanges();
            return RedirectToAction("DanhSachPhong", "PhongAdmin", new { MaKs = maks });
        }

        public ActionResult Edit(long maks, long maph, string loaihinh, long gia, string vip)
        {
            PhongKhachSan ph = db.PhongKhachSans.Find(maph);
            ph.LoaiHinh = loaihinh;
            ph.Gia = gia;

            if (!string.IsNullOrEmpty(vip) && vip == "true")
            {
                ph.VIP = true;
            }
            else
            {
                ph.VIP = false;
            }

            db.SaveChanges();
            return RedirectToAction("DanhSachPhong", "PhongAdmin", new { MaKs = maks });
        }


        public ActionResult Xoatang(long maks, long matang)
        {
            var ph = db.PhongKhachSans.Where(a => a.MaKhachSan == maks && a.MaSoTang == matang && a.TrangThai == false).ToList();
            if (ph.Count > 0)
            {
                WebMsgBox.Show("Tầng này đang có phòng khách đặt", this);
            }
            else
            {
                var phks = db.PhongKhachSans.Where(a => a.MaKhachSan == maks && a.MaSoTang == matang).ToList();
                foreach (var k in phks)
                {
                    List<AnhPhongKhachSan> anhph = db.AnhPhongKhachSans.Where(l => l.MaPhong == k.MaPhong).ToList();
                    if (anhph != null)
                    {
                        foreach (var a in anhph)
                        {
                            db.AnhPhongKhachSans.Remove(a);
                            db.SaveChanges();
                        }
                    }
                    db.PhongKhachSans.Remove(k);
                }
                db.SaveChanges();

                KhachSan ks = db.KhachSans.Find(maks);
                ks.SoTang = ks.SoTang - 1;
                db.SaveChanges();
            }
            return RedirectToAction("DanhSachPhong", "PhongAdmin", new { MaKs = maks });
        }

        public ActionResult Check_In(long mact, long maks)
        {
            ChiTietHoaDon ct = db.ChiTietHoaDons.Find(mact);
            if (ct != null)
            {
                ct.DaDen = true;
                db.SaveChanges();
            }
            return RedirectToAction("DanhSachPhong", "PhongAdmin", new { MaKs = maks });
        }

        public ActionResult Check_Out(long mact, long maks)
        {
            ChiTietHoaDon ct = db.ChiTietHoaDons.Find(mact);
            HoaDon hd = db.HoaDons.Find(ct.MaHoaDon);
            DateTime now = DateTime.Now;
            if (now < ct.NgayDi)
            {
                long sodem_truoc = demsodem((DateTime)ct.NgayDen, (DateTime)ct.NgayDi);
                ct.NgayDi = now;
                ct.HoanThanh = true;
                db.SaveChanges();
                long sodem_sau = demsodem((DateTime)ct.NgayDen, (DateTime)ct.NgayDi);
                long gia_thua = (long)(ct.PhongKhachSan.Gia * (sodem_truoc - sodem_sau));
                ct.Gia = gia_thua;
                hd.TongTien = hd.TongTien - gia_thua;
                db.SaveChanges();
            }
            else if (now > ct.NgayDi)
            {
                long sodem_truoc = demsodem((DateTime)ct.NgayDen, (DateTime)ct.NgayDi);
                ct.NgayDi = now;
                ct.HoanThanh = true;
                db.SaveChanges();
                long sodem_sau = demsodem((DateTime)ct.NgayDen, (DateTime)ct.NgayDi);
                long gia_thieu = (long)(ct.PhongKhachSan.Gia * (sodem_sau - sodem_truoc));
                ct.Gia = gia_thieu;
                hd.TongTien = hd.TongTien + gia_thieu;
                db.SaveChanges();
            }

            var chitiet = db.ChiTietHoaDons.Where(a => a.MaHoaDon == hd.MaHoaDon).ToList();
            if (chitiet.Count > 0)
            {
                bool allHoanThanh = chitiet.All(i => i.HoanThanh == true);
                if (allHoanThanh)
                {
                    hd.TrangThaiDon = "Hoàn thành";
                    db.SaveChanges();
                }
            }

            return RedirectToAction("DanhSachPhong", "PhongAdmin", new { MaKs = maks });
        }

        public long demsodem(DateTime ngayden, DateTime ngaydi)
        {
            TimeSpan den = ngayden.TimeOfDay;
            int gioden = (int)den.TotalSeconds / 3600;
            int phutden = (int)den.TotalSeconds / 60 % 60;

            TimeSpan di = ngaydi.TimeOfDay;
            int giodi = (int)di.TotalSeconds / 3600;
            int phutdi = (int)di.TotalSeconds / 60 % 60;

            DateTime checkInDate = new DateTime(ngayden.Year, ngayden.Month, ngayden.Day, gioden, phutden, 0);
            DateTime checkOutDate = new DateTime(ngaydi.Year, ngaydi.Month, ngaydi.Day, giodi, phutdi, 0);
            int sodem = check_In_Out.check(checkInDate, checkOutDate);

            return sodem;
        }

        public ActionResult ThemAnh(long? maks, long? maph)
        {
            Ht_KhachSan ht = new Ht_KhachSan();
            PhongKhachSan ph = db.PhongKhachSans.FirstOrDefault(a => a.MaKhachSan == maks && a.MaPhong == maph);
            ht.phks = ph;
            ht.anhph = db.AnhPhongKhachSans.Where(s => s.MaKhachSan == maks && s.MaPhong == maph).ToList();
            return View(ht);
        }

        [HttpPost]
        public ActionResult ThemAnhPhong(long? maks, long? maph, HttpPostedFileBase uploadhinh)
        {
            Up_IMG(maks, maph, uploadhinh);
            return RedirectToAction("ThemAnh", "PhongAdmin", new {maks = maks, maph = maph});
        }

        public ActionResult XoaAnh(long? maks, long? maph, long? maanh)
        {
            AnhPhongKhachSan anh = db.AnhPhongKhachSans.FirstOrDefault(l => l.MaAnhPhong == maanh);
            db.AnhPhongKhachSans.Remove(anh);
            db.SaveChanges();
            return RedirectToAction("ThemAnh", "PhongAdmin", new {maks = maks, maph = maph});
        }
        public ActionResult Up_IMG(long? maks, long? maph, HttpPostedFileBase uploadhinh)
        {
            if (uploadhinh != null && uploadhinh.ContentLength > 0)
            {
                string _FileName = "";
                int index = uploadhinh.FileName.IndexOf('.');
                _FileName = "nv" + RandomCode() + "." + uploadhinh.FileName.Substring(index + 1);
                string _path = Path.Combine(Server.MapPath("~/UpLoad_Img/Phong"), _FileName);
                uploadhinh.SaveAs(_path);

                
                AnhPhongKhachSan anh = new AnhPhongKhachSan();
                anh.LinkAnhPhong = _FileName;
                anh.MaPhong = maph;
                anh.MaKhachSan = maks;
                db.AnhPhongKhachSans.Add(anh);
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