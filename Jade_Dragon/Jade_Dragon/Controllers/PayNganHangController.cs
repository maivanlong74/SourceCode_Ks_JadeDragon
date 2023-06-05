using Jade_Dragon.common;
using Jade_Dragon.Models;
using Jade_Dragon.Others;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Jade_Dragon.Controllers
{
    public class PayNganHangController : Controller
    {
        private Connect db = new Connect();
        public ActionResult Payment(long? id)
        {
            HoaDon hd = db.HoaDons.Find(id);
            string url = ConfigurationManager.AppSettings["Url"];
            string returnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
            string tmnCode = ConfigurationManager.AppSettings["TmnCode"];
            string hashSecret = ConfigurationManager.AppSettings["HashSecret"];

            PayLib pay = new PayLib();

            pay.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
            pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)

            if (hd.HinhThuc == "chuyenkhoan")
            {
                pay.AddRequestData("vnp_Amount", (hd.TongTien * 100).ToString()); //số tiền cần thanh toán
            }
            else if (hd.HinhThuc == "taiquay")
            {
                pay.AddRequestData("vnp_Amount", (hd.DatCoc * 100).ToString()); //số tiền cần thanh toán
            }

            pay.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
            pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            pay.AddRequestData("vnp_IpAddr", Util.GetIpAddress()); //Địa chỉ IP của khách hàng thực hiện giao dịch
            pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            pay.AddRequestData("vnp_OrderInfo", "Thanh toan hoa don khach san"); //Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            pay.AddRequestData("vnp_ReturnUrl", returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            pay.AddRequestData("vnp_TxnRef", id.ToString()); //mã hóa đơn
            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);

            return Redirect(paymentUrl);
        }

        public ActionResult HoaDon()
        {
            if (Request.QueryString.Count > 0)
            {
                string hashSecret = ConfigurationManager.AppSettings["HashSecret"]; //Chuỗi bí mật
                var vnpayData = Request.QueryString;
                PayLib pay = new PayLib();

                //lấy toàn bộ dữ liệu được trả về
                foreach (string s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(s, vnpayData[s]);
                    }
                }


                long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef")); //mã hóa đơn
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo")); //mã giao dịch tại hệ thống VNPAY
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode"); //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string vnp_SecureHash = Request.QueryString["vnp_SecureHash"]; //hash của dữ liệu trả về
                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret); //check chữ ký đúng hay không?

                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    {
                        long slct = 0;
                        var cthdd = db.ChiTietHoaDons.Where(m => m.MaHoaDon == orderId).ToList();
                        foreach (var m in cthdd)
                        {
                            slct++;
                        }
                        HoaDon hd = db.HoaDons.Find(orderId);
                        hd.TrangThaiDon = "Đang chờ duyệt";
                        db.SaveChanges();

                        KhachSan ksks = db.KhachSans.Find(hd.MaKhachSan);
                        Session["MaKhachSanPhong"] = ksks.MaKhachSan;
                        Session["TenKhachSan"] = ksks.TenKhachSan;
                        Session["DiaChi"] = ksks.DiaChi;
                        Session["SoDienThoai_ks"] = ksks.SoDienThoai;
                        Session["GmailKhachSan"] = ksks.Gmail;
                        Session["Gia"] = ksks.Gia;
                        Session["KinhDo"] = ksks.KinhDo;
                        Session["ViDo"] = ksks.ViDo;

                        ViewBag.now = DateTime.Now;
                        List<KhuVuc> KhuVuc = new List<KhuVuc>();
                        KhuVuc = db.KhuVucs.ToList();
                        ViewBag.ListKhuVuc = KhuVuc;

                        List<ChiTietHoaDon> cthd = new List<ChiTietHoaDon>();
                        cthd = db.ChiTietHoaDons.Where(m => m.MaHoaDon == hd.MaHoaDon).ToList();
                        ViewBag.ListPhong = cthd;

                        ChiTietHoaDon chitiet = db.ChiTietHoaDons.FirstOrDefault(m => m.MaHoaDon == orderId);
                        ViewBag.ngayden = chitiet.NgayDen;
                        ViewBag.ngaydi = chitiet.NgayDi;
                        ViewBag.sodem = demsodem((DateTime)chitiet.NgayDen, (DateTime)chitiet.NgayDi);
                        db.SaveChanges();

                        Session["DongTime"] = "mo";

                        List<KhachSan> listks = new List<KhachSan>();
                        listks = db.KhachSans.Where(m => m.MaKhuVuc == hd.KhachSan.KhuVuc.MaKhuVuc).ToList();
                        ViewBag.listks = listks;

                        //Thanh toán thành công
                        ViewBag.Message = "Thanh toán thành công hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId;
                        return View(hd);
                    }
                    else
                    {
                        //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                        List<KhuVuc> KhuVuc = new List<KhuVuc>();
                        KhuVuc = db.KhuVucs.ToList();
                        ViewBag.ListKhuVuc = KhuVuc;

                        HoaDon hd = db.HoaDons.Find(orderId);
                        var ct = db.ChiTietHoaDons.Where(n => n.MaHoaDon == orderId).ToList();
                        foreach (var a in ct)
                        {
                            db.ChiTietHoaDons.Remove(a);
                            db.SaveChanges();
                        }
                        db.HoaDons.Remove(hd);
                        db.SaveChanges();
                        ViewBag.Error = "DaCoLoi";
                        ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId;
                    }
                }
                else
                {
                    List<KhuVuc> KhuVuc = new List<KhuVuc>();
                    KhuVuc = db.KhuVucs.ToList();
                    ViewBag.ListKhuVuc = KhuVuc;

                    HoaDon hd = db.HoaDons.Find(orderId);
                    var ct = db.ChiTietHoaDons.Where(n => n.MaHoaDon == orderId).ToList();
                    foreach (var a in ct)
                    {
                        db.ChiTietHoaDons.Remove(a);
                        db.SaveChanges();
                    }
                    db.HoaDons.Remove(hd);
                    db.SaveChanges();
                    ViewBag.Error = "DaCoLoi";
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId;
                }
            }

            return View();
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
    }
}