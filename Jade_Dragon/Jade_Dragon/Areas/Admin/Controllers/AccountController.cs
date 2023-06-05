using Jade_Dragon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jade_Dragon.common;
using System.Data.Entity.Infrastructure;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Owin.BuilderProperties;
using System.Reflection;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using Xamarin.Essentials;
using System.Configuration;
using System.Web.UI;


namespace Jade_Dragon.Areas.Admin.Controllers
{
    public class AccountController : Controller
    {

        private Connect db = new Connect();
        // GET: Admin/Account
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (Request.Cookies["username"] != null && Request.Cookies["password"] != null)
            {
                ViewBag.username = Request.Cookies["username"].Value;
                ViewBag.password = Request.Cookies["password"].Value;
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult kiemtradangnhap(string username, string password)
        {

            if (Request.Cookies["username"] != null && Request.Cookies["username"] != null)
            {
                username = Request.Cookies["username"].Value;
                password = Request.Cookies["password"].Value;
            }
            var f_password = GetMD5(password);
            var data = db.NguoiDungs.Where(s => s.TenDangNhap.Equals(username) && s.MatKhau.Equals(f_password)).ToList();
            if (checkpassword(username, f_password))
            {
                var userSession = new UserLogin();
                userSession.TenDn = username;

                var listGroups = GetListGroupID(username);//Có thể viết dòng lệnh lấy các GroupID từ CSDL, ví dụ gán ="ADMIN", dùng List<string>

                Session.Add("SESSION_GROUP", listGroups);
                Session.Add("USER_SESSION", userSession);

                //List thông tin User
                var Data = db.NguoiDungs.Where(x => x.TenDangNhap == username && x.MatKhau == f_password).ToList();
                long makhh = Data.FirstOrDefault().MaNguoiDung;
                Session["MaNguoiDung"] = Data.FirstOrDefault().MaNguoiDung;
                Session["HoTen"] = Data.FirstOrDefault().HoTen;
                Session["SoDienThoai"] = Data.FirstOrDefault().SoDienThoai;
                Session["CMND"] = Data.FirstOrDefault().CMND;
                Session["DiaChi"] = Data.FirstOrDefault().DiaChi;
                Session["Gmail"] = Data.FirstOrDefault().Gmail;
                Session["Avt"] = Data.FirstOrDefault().Avt;
                Session["TenDn"] = Data.FirstOrDefault().TenDangNhap;
                Session["Mk"] = Data.FirstOrDefault().MatKhau;
                Session["IDGroup"] = Data.FirstOrDefault().MaPhanQuyen;
                Session["MaKhachSan_ks"] = Data.FirstOrDefault().MaKhachSan;

                if (Data.FirstOrDefault().PhanQuyen.MaPhanQuyen == 1)
                {
                    return Redirect("~/Admin/TrangChuAdmin/TrangChu");
                }
                else
                if (Data.FirstOrDefault().PhanQuyen.MaPhanQuyen == 2)
                {
                    return Redirect("~/trangchu/trangchu?makhh=" + makhh);
                }
                else
                if (Data.FirstOrDefault().PhanQuyen.MaPhanQuyen == 3)
                {
                    return Redirect("~/Admin/QLKhachHang/QuanLyKhManage");
                }

            }
            WebMsgBox.Show("Đăng nhập không thành công", this);
            return Redirect("~/Admin/Account/Login");
        }
        public List<string> GetListGroupID(string userName)
        {
            // var user = db.User.Single(x => x.UserName == userName);

            var data = (from a in db.PhanQuyens
                        join b in db.NguoiDungs on a.MaPhanQuyen equals b.MaPhanQuyen
                        where b.TenDangNhap == userName

                        select new
                        {
                            UserGroupID = b.MaPhanQuyen,
                            UserGroupName = a.TenQuyen
                        });

            return data.Select(x => x.UserGroupName).ToList();

        }
        public bool checkpassword(string username, string password)
        {
            if (db.NguoiDungs.Where(x => x.TenDangNhap == username && x.MatKhau == password).Count() > 0)

                return true;
            else
                return false;


        }

        public string GenerateVerificationCode()
        {
            int codeLength = 6; // Độ dài của mã xác minh
            Random random = new Random();
            string code = "";

            for (int i = 0; i < codeLength; i++)
            {
                int digit = random.Next(0, 9); // Lấy ngẫu nhiên một số từ 0 đến 9
                code += digit.ToString(); // Thêm số vào chuỗi mã xác minh
            }

            return code;
        }

        public ActionResult SignUpp()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _SignUp(NguoiDung kh, string NhapLai)
        {
            if (ModelState.IsValid)
            {
                var check = db.NguoiDungs.FirstOrDefault(s => s.TenDangNhap == kh.TenDangNhap);
                if (check == null)
                {
                    kh.MatKhau = GetMD5(kh.MatKhau);
                    if (kh.MatKhau == GetMD5(NhapLai))
                    {

                        kh.Code = GenerateVerificationCode();
                        db.Configuration.ValidateOnSaveEnabled = false;
                        db.NguoiDungs.Add(kh).MaPhanQuyen = 2;
                        db.SaveChanges();
                        if (XacNhanGmail("", "", kh.Gmail, "", "", kh.Code) == true)
                        {
                            Session["MaNguoiDung"] = kh.MaNguoiDung;
                            return Redirect("~/Admin/Account/SignUpp#XnGmail");
                        }
                        else
                        {
                            WebMsgBox.Show("Đã có lỗi gửi mã xác nhận, vui lòng thử lại", this);
                            return Redirect("~/Admin/Account/SignUpp");
                        }
                    }
                    else
                    {
                        WebMsgBox.Show("Bạn nhập mật khẩu không trùng khớp với mật khẩu trước đó", this);
                        return Redirect("~/Admin/Account/SignUpp");
                    }
                }
                else
                {
                    WebMsgBox.Show("Tên đăng nhập này đã tồn tại", this);
                    return Redirect("~/Admin/Account/SignUpp");
                }

            }
            return Redirect("~/Admin/Account/SignUpp");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult XacNhan(string Code, long MaKH)
        {
            if (Request.Cookies["Code"] != null)
            {
                Code = Request.Cookies["Code"].Value;
                MaKH = long.Parse(Request.Cookies["MaKH"].Value);
            }
            NguoiDung KhachHang = db.NguoiDungs.Find(MaKH);
            if (db.NguoiDungs.Where(m => m.Code == Code).Count() > 0)
            {
                KhachHang.DaXacMinh = true;
                db.SaveChanges();
                return Redirect("~/trangchu/trangchu");
            }
            else
                return Redirect("~/Admin/Account/SignUpp#XnGmail");
        }

        public ActionResult QuenPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QuenPasswordd(string hoten, string tendn, string gmail)
        {
            if (string.IsNullOrEmpty(hoten) || string.IsNullOrEmpty(tendn) || string.IsNullOrEmpty(gmail))
            {
                return Redirect("QuenPassword");
            }
            else
            {
                string code = GenerateVerificationCode();
                NguoiDung kh = db.NguoiDungs.FirstOrDefault(a => a.TenDangNhap == tendn);
                if (db.NguoiDungs.Where(m => m.HoTen == hoten && m.TenDangNhap == tendn && m.Gmail == gmail).Count() > 0)
                {
                    kh.Code = code;
                    XacNhanGmail(hoten, "", gmail, "", "", code);
                    db.SaveChanges();
                    return Redirect("NewPassword");
                }
                else
                {
                    WebMsgBox.Show("Đã có lỗi, vui lòng thử lại", this);
                    return Redirect("QuenPassword");
                }
            }
        }

        public ActionResult NewPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewPassword(string ma, string NewPass, string NewPass2)
        {
            if (string.IsNullOrEmpty(ma) || string.IsNullOrEmpty(NewPass) || string.IsNullOrEmpty(NewPass2))
            {
                return Redirect("NewPassword");
            }
            else
            {
                NguoiDung kh = db.NguoiDungs.FirstOrDefault(m => m.Code == ma);
                if (NewPass == NewPass2)
                {
                    kh.MatKhau = GetMD5(NewPass);
                    db.SaveChanges();
                    return Redirect("Login");
                }
                else
                {
                    WebMsgBox.Show("Mật khẩu không giống nhau", this);
                    return Redirect("NewPassword");
                }
            }
        }

        //create a string MD5
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

        public ActionResult SignOut()
        {

            Session.Clear();

            if (Request.Cookies["TenKh"] != null && Request.Cookies["Mk"] != null)
            {
                HttpCookie us = Request.Cookies["TenKh"];
                HttpCookie ps = Request.Cookies["Mk"];

                ps.Expires = DateTime.Now.AddDays(-1);
                us.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(us);
                Response.Cookies.Add(ps);
            }

            return Redirect("~/trangchu/trangchu");
        }

        public bool XacNhanGmail(string HoTen, string sdt, string Gmail, string DiaChi, string CMND, string Ma)
        {
            try
            {
                string content = System.IO.File.ReadAllText(Server.MapPath("~/content/template/neworder.html"));
                content = content.Replace("{{CustomerName}}", HoTen);
                content = content.Replace("{{Phone}}", sdt);
                content = content.Replace("{{Email}}", Gmail);
                content = content.Replace("{{Address}}", DiaChi);
                content = content.Replace("{{CMND}}", CMND);
                content = content.Replace("{{MaXacNhan}}", Ma);
                var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();

                // Để Gmail cho phép SmtpClient kết nối đến server SMTP của nó với xác thực 
                //là tài khoản gmail của bạn, bạn cần thiết lập tài khoản email của bạn như sau:
                //Vào địa chỉ https://myaccount.google.com/security  Ở menu trái chọn mục Bảo mật, sau đó tại mục Quyền truy cập 
                //của ứng dụng kém an toàn phải ở chế độ bật
                //  Đồng thời tài khoản Gmail cũng cần bật IMAP
                //Truy cập địa chỉ https://mail.google.com/mail/#settings/fwdandpop

                new MailHelper().SendMail(Gmail, "Xác nhận Gmail", content);
                new MailHelper().SendMail(toEmail, "Xác nhận Gmail", content);
            }
            catch (Exception)
            {
                //ghi log
                return false;
            }
            return true;
        }
    }
}