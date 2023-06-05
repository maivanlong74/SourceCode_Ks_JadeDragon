using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Wordprocessing;
using Jade_Dragon.Areas.Admin.Controllers;
using Jade_Dragon.common;
using Jade_Dragon.Models;
using PagedList;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Menu;

namespace Jade_Dragon.Controllers
{
    public class TrangKhachSanController : Controller
    {
        private Connect db = new Connect();
        private const string giohang = "giohang";
        // GET: TrangKhachSan
        public ActionResult TrangKhachSan(string searchTerm, string searchType, long? khuvuc, long? maks, long? makh,
            string loai, long? gia, string vip, string trangthai, DateTime? batdau = null, DateTime? ketthuc = null)
        {
            Session["KhachSan"] = null;
            Session["LoaiPhong"] = null;
            Session["GiaTien"] = null;
            Session["Menu_VIP"] = null;
            Session["TrangThaiPhong"] = null;
            hienthiphong hienthi = new hienthiphong();
            var khach_san = db.KhachSans.ToList();
            if(maks == null)
            {
                foreach(var k in khach_san)
                {
                    maks = k.MaKhachSan;
                    break;
                }
            }
            List<PhongKhachSan> listphong = db.PhongKhachSans.Where(a => a.MaKhachSan == maks).ToList();
            foreach (var dem in listphong)
            {
                if (dem.TrangThai == false)
                {
                    dem.TrangThai = true;
                    db.SaveChanges();
                }
            }

            DateTime timenow = DateTime.Now;

            foreach (var mm in listphong)
            {
                if (mm.KhoaPhong == true)
                {
                    mm.TrangThai = false; db.SaveChanges();
                }
                else
                {
                    var cthd = db.ChiTietHoaDons.Where(m => m.MaPhong == mm.MaPhong).ToList();
                    if (cthd.Count() > 0)
                    {
                        foreach (var ct in cthd)
                        {
                            if (ct.HoanThanh == false)
                            {
                                if (ct.DaDen == true)
                                {
                                    mm.TrangThai = false;
                                    db.SaveChanges();
                                }
                                else
                                {
                                    if (ct.HoaDon.DaDat == true)
                                    {
                                        if (batdau == null && ketthuc == null)
                                        {

                                            if (ct.NgayDen <= timenow && timenow <= ct.NgayDi)
                                            {
                                                mm.TrangThai = false;
                                            }

                                        }
                                        else
                                        {
                                            if (batdau > ketthuc)
                                            {
                                                DateTime tam = (DateTime)batdau;
                                                batdau = ketthuc;
                                                ketthuc = tam;
                                            }
                                            long sodem = demsodem((DateTime)batdau, (DateTime)ketthuc);
                                            for (long i = 0; i <= sodem; i++)
                                            {
                                                DateTime day = batdau.Value.AddDays(i);
                                                if (ct.NgayDen <= day && day <= ct.NgayDi)
                                                {
                                                    mm.TrangThai = false;
                                                    i = sodem + 1;
                                                }
                                                else if (day > ct.NgayDi)
                                                {
                                                    mm.TrangThai = true;
                                                    i = sodem + 1;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            db.SaveChanges();

            var Phong = db.PhongKhachSans.Include(k => k.KhachSan);

            if (khuvuc != null && loai != null && vip != null && batdau != null && ketthuc != null)
            {
                bool isVip = vip.ToLower() == "true";
                Phong = Phong.Where(a => a.KhachSan.MaKhuVuc == khuvuc && a.LoaiHinh == loai
                            && a.VIP == isVip);
                if (batdau > ketthuc)
                {
                    DateTime tam = (DateTime)batdau;
                    batdau = ketthuc;
                    ketthuc = tam;
                }

                var chitiet = db.ChiTietHoaDons.ToList();
                long sodem = demsodem((DateTime)batdau, (DateTime)ketthuc);
                if (chitiet != null)
                {
                    for (long i = 0; i <= sodem; i++)
                    {
                        DateTime day = batdau.Value.AddDays(i);
                        foreach (var item in chitiet)
                        {
                            if (item.NgayDen != null && item.NgayDi != null)
                            {
                                PhongKhachSan ph = db.PhongKhachSans.FirstOrDefault(m => m.MaPhong == item.MaPhong);
                                if (item.NgayDen <= day && day <= item.NgayDi)
                                {
                                    ph.TrangThai = false;
                                    i = sodem;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (maks != null)
                {
                    Session["KhachSan"] = maks;
                    ViewBag.ma = maks;
                    KhachSan ksks = db.KhachSans.Find(maks);
                    ViewBag.ksks = ksks.TenKhachSan;
                    Phong = Phong.Where(a => a.MaKhachSan == maks);
                }
                if (loai != null)
                {
                    Session["LoaiPhong"] = loai;
                    Phong = Phong.Where(a => a.LoaiHinh == loai && a.MaKhachSan == maks);
                }
                if (gia != null)
                {
                    Session["GiaTien"] = gia;
                    Phong = Phong.Where(a => a.Gia == gia && a.MaKhachSan == maks);
                }
                if (vip != null)
                {
                    Session["Menu_VIP"] = vip;
                    bool isVip = vip.ToLower() == "true";
                    Phong = Phong.Where(a => a.VIP == isVip && a.MaKhachSan == maks);
                }
                if (trangthai != null)
                {
                    Session["TrangThaiPhong"] = trangthai;
                    bool isTrangThai = trangthai.ToLower() == "true";
                    Phong = Phong.Where(a => a.TrangThai == isTrangThai && a.MaKhachSan == maks);
                }
            }

            hienthi.htks = db.KhachSans.Find(maks);
            hienthi.ks = db.KhachSans.ToList();
            hienthi.ph = Phong.OrderBy(x => x.TenPhong).ToList();
            hienthi.ctphong = db.PhongKhachSans.Where(a => a.MaKhachSan == maks).ToList();
            hienthi.cmt = db.BinhLuans.Where(s => s.MaKhachSan == maks).ToList();
            hienthi.AnhKs = db.AnhKhachSans.Where(l => l.MaKhachSan == maks).ToList();
            hienthi.AnhPhong = db.AnhPhongKhachSans.Where(k => k.MaKhachSan == maks).ToList();
            hienthi.dg = db.DanhGiaKhachSans.Where(a => a.MaNguoiDung == makh).ToList();
            hienthi.tkdg = db.ThongKeDanhGias.ToList();
            DanhGiaKhachSan dg = db.DanhGiaKhachSans.FirstOrDefault(q => q.MaKhachSan == maks && q.MaNguoiDung == makh);
            if(dg != null)
            {
                ViewBag.danhgia = dg.SoSao;
            }

            if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrEmpty(searchType))
            {
                var searchResult = TimKiem(searchTerm, searchType);
                hienthi.ph = searchResult.OrderBy(x => x.TenPhong).ToList();
            }
            DateTime time_now = DateTime.Now.AddDays(-1);
            DateTime time_max = DateTime.Now.AddDays(30);
            ViewBag.time_now = loadtime(time_now);
            ViewBag.time_max = loadtime(time_max);
            Session["batdau"] = batdau;
            Session["ketthuc"] = ketthuc;

            return View("TrangKhachSan", hienthi);
        }

        public List<PhongKhachSan> TimKiem(string searchTerm, string searchType)
        {
            IQueryable<PhongKhachSan> query = db.PhongKhachSans;
            switch (searchType.ToLower())
            {
                case "khachsan":
                    query = query.Where(c => c.KhachSan.TenKhachSan.Contains(searchTerm));
                    break;
                case "name":
                    query = query.Where(c => c.TenPhong.Contains(searchTerm));
                    break;
                case "gia":
                    long Gia;
                    if (long.TryParse(searchTerm, out Gia))
                    {
                        query = query.Where(c => c.Gia == Gia);
                    }
                    break;
                case "all":
                    query = query.Where(c =>
                        c.KhachSan.TenKhachSan.Contains(searchTerm) ||
                        c.TenPhong.Contains(searchTerm) ||
                        c.Gia.ToString().Contains(searchTerm)
                    );
                    break;
                default:
                    break;
            }
            return query.ToList();
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

        public string loadtime(DateTime time)
        {
            TimeSpan den = time.TimeOfDay;
            int sogio = (int)den.TotalSeconds / 3600;
            int sophut = (int)den.TotalSeconds / 60 % 60;

            string nam = time.Year.ToString("D4");
            string thang = time.Month.ToString("D2");
            string ngay = time.Day.ToString("D2");
            string gio = sogio.ToString("D2");
            string phut = sophut.ToString("D2");

            string xuly = nam + "-" + thang + "-" + ngay + "T" + gio + ":" + phut;
            return xuly;
        }

        [HttpPost]
        public ActionResult DanhGia(long? SoSao, long makh, long maks)
        {
            if (SoSao != null)
            {
                HoaDon hd = db.HoaDons.FirstOrDefault(s => s.MaNguoiDung == makh && s.MaKhachSan == maks);
                if (hd != null)
                {
                    ChiTietHoaDon chitiet = db.ChiTietHoaDons.FirstOrDefault(a => a.MaHoaDon == a.MaHoaDon && a.HoanThanh == true);
                    if (chitiet != null)
                    {
                        DanhGiaKhachSan so = new DanhGiaKhachSan();
                        so.MaKhachSan = maks;
                        so.MaNguoiDung = makh;
                        so.SoSao = SoSao;
                        db.DanhGiaKhachSans.Add(so);
                        db.SaveChanges();
                        ThongKeDanhGia thongke = db.ThongKeDanhGias.FirstOrDefault(a => a.MaKhachSan == maks);

                        if (SoSao == 1)
                        {
                            thongke.MotSao = thongke.MotSao + 1;
                            thongke.TongSao = thongke.TongSao + 1;
                            db.SaveChanges();
                        }
                        else if (SoSao == 2)
                        {
                            thongke.HaiSao = thongke.HaiSao + 1;
                            thongke.TongSao = thongke.TongSao + 1;
                            db.SaveChanges();
                        }
                        else if (SoSao == 3)
                        {
                            thongke.BaSao = thongke.BaSao + 1;
                            thongke.TongSao = thongke.TongSao + 1;
                            db.SaveChanges();
                        }
                        else if (SoSao == 4)
                        {
                            thongke.BonSao = thongke.BonSao + 1;
                            thongke.TongSao = thongke.TongSao + 1;
                            db.SaveChanges();
                        }
                        else if (SoSao == 5)
                        {
                            thongke.NamSao = thongke.NamSao + 1;
                            thongke.TongSao = thongke.TongSao + 1;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        WebMsgBox.Show("Bạn hãy trải nghiệm phòng và cho chúng tôi đánh giá", this);
                    }
                }
                else
                {
                    WebMsgBox.Show("Bạn hãy trải nghiệm phòng và cho chúng tôi đánh giá", this);
                }
            }
            else
            {
                WebMsgBox.Show("Bạn chưa đánh giá", this);
            }
            return RedirectToAction("TrangKhachSan", "TrangKhachSan", new { maks = maks });
        }

        public ActionResult QuayVe(long? ma)
        {
            Session["batdau"] = null;
            Session["ketthuc"] = null;
            if (ma == null)
            {
                return Redirect("TrangKhachSan");
            }
            else
            {
                return RedirectToAction("TrangKhachSan", "TrangKhachSan", new { ma = ma });
            }
        }
    }
}