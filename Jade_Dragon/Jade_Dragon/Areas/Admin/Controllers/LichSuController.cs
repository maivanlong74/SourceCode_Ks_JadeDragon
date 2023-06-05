using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Jade_Dragon.Models;

namespace Jade_Dragon.Areas.Admin.Controllers
{
    public class LichSuController : baseController
    {
        private Connect db = new Connect();
        // GET: Admin/LichSu
        public ActionResult LichSu()
        {
            var nguoiTruyCap = db.SoNguoiTruyCaps.FirstOrDefault();
            ViewBag.NguoiTruyCap = nguoiTruyCap.SoLuongNguoi.ToString();
            ViewBag.NguoiOnline = HttpContext.Application["NguoiOnline"].ToString();
            ViewBag.sluser = thongkenguoidung();
            var ks = db.KhachSans.ToList();

            if (ks.Count > 0)
            {
                var hd = db.HoaDons.ToList();
                if (hd.Count > 0)
                {
                    ViewBag.tongdoanhthu = doanhthu();
                    ViewBag.sldonhang = thongkedonhang();
                }
                else
                {
                    ViewBag.tongdoanhthu = null;
                    ViewBag.sldonhang = null;
                }
                ViewBag.ksks = "abc";
                return View("LichSu", hd);
            }
            else
            {
                ViewBag.ksks = null;
                return View("LichSu");
            }
        }

        public decimal doanhthu()
        {
            decimal tongdoanhthu = db.HoaDons.Sum(n => n.TongTien).Value;
            return tongdoanhthu;
        }

        public decimal thongke(int thang, int nam)
        {

            var List = db.HoaDons.Where(n => n.NgayDat.Value.Month == thang && n.NgayDat.Value.Year == nam);
            decimal tongtien = 0;
            foreach (var item in List)
            {
                tongtien += decimal.Parse(item.TongTien.Value.ToString());
            }
            return tongtien;
        }

        public double thongkedonhang()
        {
            double sldonhang = db.HoaDons.Count();
            return sldonhang;
        }

        public double thongkenguoidung()
        {
            double sluser = db.NguoiDungs.Count();
            return sluser;
        }

        public ActionResult HoaDon(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HoaDon hoadon = db.HoaDons.Find(id);
            if (hoadon == null)
            {
                return HttpNotFound();
            }
            return View(hoadon);
        }

        public ActionResult ChiTiet(long? ma)
        {
            var List_ChiTiet = db.ChiTietHoaDons.Where(m => m.MaHoaDon == ma).ToList();
            Session["tongtien"] = tongtien(ma);
            Session["mahoadon"] = ma;
            HoaDon hd = db.HoaDons.Find(ma);
            Session["HoTen"] = hd.HoTen;
            return View("ChiTiet", List_ChiTiet);
        }

        public decimal tongtien(long? ma)
        {
            decimal tong = 0;
            var List = db.ChiTietHoaDons.Where(m => m.MaHoaDon == ma).ToList();
            foreach (var chiTiet in List)
            {
                tong += (decimal)chiTiet.Gia;
            }
            return tong;
        }

        public ActionResult LichSuManage(long? id)
        {
            var nguoiTruyCap = db.SoNguoiTruyCaps.FirstOrDefault();
            ViewBag.NguoiTruyCap = nguoiTruyCap.SoLuongNguoi.ToString();
            ViewBag.NguoiOnline = HttpContext.Application["NguoiOnline"].ToString();
            ViewBag.sluser = db.NguoiDungs.Count();
            KhachSan ks = db.KhachSans.Find(id);

            if (ks != null)
            {
                HoaDon hd = db.HoaDons.FirstOrDefault(m => m.MaKhachSan == id);
                if (hd != null)
                {
                    ViewBag.tongdoanhthuks = db.HoaDons.Where(m => m.MaKhachSan == id).Sum(n => n.TongTien).Value;
                    ViewBag.sldonhangks = db.HoaDons.Where(m => m.MaKhachSan == id).Count();
                }
                else
                {
                    ViewBag.tongdoanhthu = null;
                    ViewBag.sldonhang = null;
                }

                var hoadonn = db.HoaDons.Where(m => m.MaKhachSan == id).ToList();
                ViewBag.ksks = "123";
                return View("LichSuManage", hoadonn);
            }
            else
            {
                ViewBag.ksks = null;
                return View("LichSuManage");
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