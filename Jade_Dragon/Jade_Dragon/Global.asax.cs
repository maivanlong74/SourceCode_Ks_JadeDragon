using Jade_Dragon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Xamarin.Essentials;
using static System.Net.Mime.MediaTypeNames;
using Jade_Dragon.common;

namespace Jade_Dragon
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            LuongTruyCap.TangSoLuongTruyCap();
            Application["NguoiTruyCap"] = 0;
            Application["NguoiOnline"] = 0;
        }

        protected void Session_Start()
        {
            Application.Lock();
            Application["NguoiTruyCap"] = (int)Application["NguoiTruyCap"] + 1;
            Application["NguoiOnline"] = (int)Application["NguoiOnline"] + 1;
            Application.UnLock();
        }
        protected void Session_End()
        {
            Application.Lock();
            Application["NguoiOnline"] = (int)Application["NguoiOnline"] - 1;
            Application.UnLock();
        }

        protected void Session_KhachHang()
        {
            Session["MaNguoiDung"] = "";
            Session["HoTen"] = "";
            Session["HoTen2"] = "";
            Session["SoDienThoai"] = "";
            Session["CMND"] = "";
            Session["DiaChi"] = "";
            Session["Gmail"] = "";
            Session["TheNganHang"] = "";
            Session["TenNganHang"] = "";
            Session["Avt"] = "";
            Session["TenDn"] = "";
            Session["Mk"] = "";
            Session["IDGroup"] = "";
            Session["MaKhachSan_ks"] = "";
        }

        protected void Session_KhachSan()
        {
            Session["MaKhachSan"] = "";
            Session["TenKhachSan"] = "";
            Session["DiaChi"] = "";
            Session["SoDienThoai_ks"] = "";
            Session["Gia"] = "";
            Session["GmailKhachSan"] = "";
            Session["AnhKs"] = "";
            Session["KhuVuc"] = "";
            Session["makv"] = "";
            Session["ListKv"] = "";
            Session["batdau"] = "";
            Session["ketthuc"] = "";
            Session["DongTime"] = "";
        }
        protected void Session_Phong()
        {
            Session["MaPhong"] = "";
            Session["TenPhong"] = "";
            Session["LoaiHinh"] = "";
            Session["Gia"] = "";
            Session["VIP"] = "";
            Session["TrangThai"] = "";
            Session["tongtien"] = "";
            Session["TongSoLuong"] = "";
            Session["mahoadon"] = "";
            Session["MaKhachSanPhong"] = "";
            Session["KinhDo"] = "";
            Session["ViDo"] = "";

        }
        protected void Sesson_Kh()
        {
            Session["makhachhang"] = "";
            Session["tenkhachhang"] = "";
            Session["sodienthoai"] = "";
            Session["gmail"] = "";
            Session["sotaikhoan"] = "";
            Session["tennganhang"] = "";
            Session["ngayden"] = "";
            Session["ngaydi"] = "";
            Session["sodem"] = "";
            Session["moeny"] = "";
        }

        protected void Sesson_hd()
        {
            Session["MaHoaDon_a"] = "";
            Session["TenKhachSan_a"] = "";
            Session["HoTen_a"] = "";
            Session["Sdt_a"] = "";
            Session["Cmnd_a"] = "";
            Session["SoLuong_a"] = "";
            Session["NgayDat_a"] = "";
            Session["TongTien_a"] = "";
            Session["TienDatCoc"] = "";
        }
        protected void Sesson_menu()
        {
            Session["ma_khachsan"] = "";
            Session["ten_khachsan"] = "";
            Session["diachi_khachsan"] = "";
            Session["sdt_khachsan"] = "";
            Session["gmail_khachsan"] = "";

            Session["LoaiPhong"] = "";
            Session["GiaTien"] = "";
            Session["Menu_VIP"] = "";
            Session["TrangThaiPhong"] = "";
            Session["PhongAdmin"] = "";
            Session["MaPhongAdmin"] = "";
            Session["MaPhong"] = "";
            Session["TenPhong"] = "";
        }
    }
}
