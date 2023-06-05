using Jade_Dragon.common;
using Jade_Dragon.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Jade_Dragon.Controllers
{
    public class UserController : Controller
    {
        private Connect db = new Connect();
        // GET: User
        public ActionResult TrangCaNhan(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NguoiDung nguoidung = db.NguoiDungs.Find(id);
            if (nguoidung == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDGroup = new SelectList(db.PhanQuyens, "MaPhanQuyen", "TenQuyen", nguoidung.MaPhanQuyen);
            return View(nguoidung);
        }

        // POST: Admin/Test/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TrangCaNhan([Bind(Include = "MaNguoiDung,HoTen,SoDienThoai,CMND,DiaChi,Gmail,Avt,TenDangNhap,MatKhau,MaKhachSan,MaPhanQuyen,Code,DaXacMinh")] NguoiDung nguoiDung, HttpPostedFileBase uploadhinh)
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

                if (unv.MatKhau != nguoiDung.MatKhau)
                {
                    unv.MatKhau = GetMD5(nguoiDung.MatKhau);
                }

                unv.DaXacMinh = nguoiDung.DaXacMinh;

                if (uploadhinh != null && uploadhinh.ContentLength > 0)
                {
                    long id = nguoiDung.MaNguoiDung;
                    string _FileName = "";
                    string code_name = GenerateVerificationCode();
                    int index = uploadhinh.FileName.IndexOf('.');
                    _FileName = "nv" + code_name + "a" + "." + uploadhinh.FileName.Substring(index + 1);
                    string _path = Path.Combine(Server.MapPath("~/UpLoad_Img/KhachHang"), _FileName);
                    uploadhinh.SaveAs(_path);
                    unv.Avt = _FileName;
                }
                db.SaveChanges();

                Session["Avt"] = unv.Avt;
                return Redirect("~/User/TrangCaNhan/" + nguoiDung.MaNguoiDung);
            }
            ViewBag.IDGroup = new SelectList(db.PhanQuyens, "MaPhanQuyen", "TenQuyen", nguoiDung.MaPhanQuyen);
            return View(nguoiDung);
        }

        public ActionResult QuenMatKhau(long? id)
        {
            NguoiDung kh = db.NguoiDungs.Find(id);
            kh.Code = GenerateVerificationCode();
            db.SaveChanges();
            Session["MaNguoiDung"] = kh.MaNguoiDung;
            XacNhanGmail(kh.HoTen, "", kh.Gmail, "", "", kh.Code);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult XacNhan(string Code, long MaKH, string Mk, string NhapLai)
        {
            if (Request.Cookies["Code"] != null)
            {
                Code = Request.Cookies["Code"].Value;
                MaKH = long.Parse(Request.Cookies["MaKH"].Value);
                Mk = Request.Cookies["Mk"].Value;
            }
            NguoiDung KhachHang = db.NguoiDungs.Find(MaKH);
            if (db.NguoiDungs.Where(m => m.Code == Code).Count() > 0)
            {
                if (KhachHang.MatKhau == GetMD5(Mk))
                {
                    KhachHang.MatKhau = GetMD5(NhapLai);
                    db.SaveChanges();
                    return Redirect("~/Admin/Account/Login");
                }
                else
                {
                    return Redirect("~/trangchu/trangchu");
                }
            }
            else
                return Redirect("~/trangchu/trangchu");
        }

        [HttpPost]
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
                string code_name = GenerateVerificationCode();
                int index = uploadhinh.FileName.IndexOf('.');
                _FileName = "nv" + code_name + "." + uploadhinh.FileName.Substring(index + 1);
                string _path = Path.Combine(Server.MapPath("~/UpLoad_Img/KhachHang"), _FileName);
                uploadhinh.SaveAs(_path);

                NguoiDung unv = db.NguoiDungs.FirstOrDefault(x => x.MaNguoiDung == id);
                unv.Avt = _FileName;
                db.SaveChanges();
            }
            return View();
        }

        public ActionResult XoaAnh(long? kh)
        {
            NguoiDung khachhang = db.NguoiDungs.Find(kh);
            khachhang.Avt = null;
            db.SaveChanges();
            return Redirect("~/Admin/QLKhachHang/Details/" + khachhang.MaNguoiDung);
        }

        public bool XacNhanGmail(string HoTen, string sdt, string Gmail, string DiaChi, string CMND, string Ma)
        {
            try
            {
                string content = System.IO.File.ReadAllText(Server.MapPath("~/content/template/QuenMatKhau.html"));
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