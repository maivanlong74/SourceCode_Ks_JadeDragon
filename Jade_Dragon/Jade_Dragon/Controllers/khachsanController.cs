using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Jade_Dragon.Areas.Admin.Controllers;
using Jade_Dragon.common;
using Jade_Dragon.Models;
using PagedList;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Menu;

namespace Jade_Dragon.Controllers
{
    public class khachsanController : Controller
    {
        private Connect db = new Connect();
        private const string giohang = "giohang";

        // GET: khachsan
        public ActionResult khachsan(int? page, string searchTerm, string searchType, long? khuvuc,
            long? ma, string loai, long? gia, string vip, string trangthai, DateTime? batdau = null, DateTime? ketthuc = null)
        {

            Session["KhachSan"] = null;
            Session["LoaiPhong"] = null;
            Session["GiaTien"] = null;
            Session["Menu_VIP"] = null;
            Session["TrangThaiPhong"] = null;

            hienthiphong m = new hienthiphong();
            List<KhachSan> list = new List<KhachSan>();
            list = db.KhachSans.OrderBy(khachsan => khachsan.ThangDiem).ToList();

            List<PhongKhachSan> listphong = db.PhongKhachSans.ToList();
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

            List<PhongKhachSan> list2 = new List<PhongKhachSan>();
            list2 = db.PhongKhachSans.ToList();

            int pageSize = 6; // số lượng phần tử hiển thị trong mỗi trang
            int pageNumber = (page ?? 1); // trang hiện tại, mặc định là trang đầu tiên
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
                if (ma != null)
                {
                    Session["KhachSan"] = ma;
                    ViewBag.ma = ma;
                    KhachSan ksks = db.KhachSans.Find(ma);
                    ViewBag.ksks = ksks.TenKhachSan;
                    Phong = Phong.Where(a => a.MaKhachSan == ma);
                }
                if (loai != null)
                {
                    Session["LoaiPhong"] = loai;
                    Phong = Phong.Where(a => a.LoaiHinh == loai);
                }
                if (gia != null)
                {
                    Session["GiaTien"] = gia;
                    Phong = Phong.Where(a => a.Gia == gia);
                }
                if (vip != null)
                {
                    Session["Menu_VIP"] = vip;
                    bool isVip = vip.ToLower() == "true";
                    Phong = Phong.Where(a => a.VIP == isVip);
                }
                if (trangthai != null)
                {
                    Session["TrangThaiPhong"] = trangthai;
                    bool isTrangThai = trangthai.ToLower() == "true";
                    Phong = Phong.Where(a => a.TrangThai == isTrangThai);
                }
            }

            m.ph = Phong.OrderBy(x => x.TenPhong)
                         .Skip((pageNumber - 1) * pageSize)
                         .Take(pageSize)
                         .ToList();

            int totalItems = Phong.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;

            m.ks = list;
            m.ctphong = list2;

            if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrEmpty(searchType))
            {
                var searchResult = TimKiem(searchTerm, searchType);
                m.ph = searchResult.OrderBy(x => x.TenPhong).ToList();
                m.ph = m.ph.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                ViewBag.TotalItems = m.ph.Count();
                ViewBag.TotalPages = (int)Math.Ceiling((double)m.ph.Count() / pageSize);
                ViewBag.PageNumber = pageNumber;
                ViewBag.PageSize = pageSize;
            }
            DateTime time_now = DateTime.Now.AddDays(-1);
            DateTime time_max = DateTime.Now.AddDays(30);
            ViewBag.time_now = loadtime(time_now);
            ViewBag.time_max = loadtime(time_max);
            Session["batdau"] = batdau;
            Session["ketthuc"] = ketthuc;
            m.AnhKs = db.AnhKhachSans.ToList();

            return View("khachsan", m);
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

        public ActionResult QuayVe(long? ma)
        {
            Session["batdau"] = null;
            Session["ketthuc"] = null;
            if (ma == null)
            {
                return Redirect("khachsan");
            }
            else
            {
                return RedirectToAction("khachsan", "khachsan", new { ma = ma });
            }
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
