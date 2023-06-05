using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;
using Jade_Dragon.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Windows.Interop;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Threading.Tasks;


namespace Jade_Dragon.Hubs
{
    [HubName("chat")]
    public class DemoChat : Hub
    {
        private Connect db = new Connect();
        public void Message(int roomId, long makh, string message, string imageUrl, bool loai)
        {
            var machat = db.TinNhanNhoms.ToList();
            var machatadmin = db.TinNhanNguoiDungs.ToList();

            // Thêm mới tin nhắn chat vào CSDL
            if (loai == false)
            {
                var newtn = new TinNhanNhom();
                {

                    bool trungMaTinNhan = true;
                    while (trungMaTinNhan)
                    {
                        newtn.MaTinNhan = "ChatGroup" + RadomCode();
                        trungMaTinNhan = false;
                        foreach (var i in machat)
                        {
                            if (newtn.MaTinNhan == i.MaTinNhan)
                            {
                                trungMaTinNhan = true;
                                break;
                            }
                        }
                    }

                    newtn.MaPhongChat = roomId;
                    newtn.MaNguoiDung = makh;
                    newtn.NoiDungChat = message;
                    newtn.LinkAnh = imageUrl;
                    newtn.NgayGui = DateTime.Now;
                }
                db.TinNhanNhoms.Add(newtn);
                db.SaveChanges();

                PhongChat phchat = db.PhongChats.Find(roomId);
                NguoiDung kh = db.NguoiDungs.Find(makh);
                string name = kh.HoTen;
                string tenphong = phchat.TenPhongChat;
                DateTime timenow = DateTime.Now;
                TinNhanNhom tn = db.TinNhanNhoms.FirstOrDefault(m => m.MaNguoiDung == makh);
                // Gửi tin nhắn chat đến tất cả các client đang kết nối đến phòng chat
                Clients.All.Message(name, message, makh, timenow, tn.MaTinNhan, imageUrl);
            }
            else
            {
                var tn = new TinNhanNguoiDung();
                {
                    bool trungMaTinNhan = true;
                    while (trungMaTinNhan)
                    {
                        tn.MaChatNguoiDung = "ChatClient" + RadomCode();
                        trungMaTinNhan = false;
                        foreach (var i in machatadmin)
                        {
                            if (tn.MaChatNguoiDung == i.MaChatNguoiDung)
                            {
                                trungMaTinNhan = true;
                                break;
                            }
                        }
                    }


                    tn.IdNguoiNhan = roomId;
                    tn.IdNguoiGui = makh;
                    tn.NoiDungChat = message;
                    tn.LinkAnh = imageUrl;
                    tn.NgayGuiChat = DateTime.Now;
                }
                db.TinNhanNguoiDungs.Add(tn);
                db.SaveChanges();

                NguoiDung kh = db.NguoiDungs.Find(makh);
                string name = kh.HoTen;
                DateTime timenow = DateTime.Now;
                TinNhanNguoiDung tnadmin = db.TinNhanNguoiDungs.FirstOrDefault(m => m.IdNguoiNhan == roomId && m.IdNguoiGui == makh);
                // Gửi tin nhắn chat đến tất cả các client đang kết nối đến phòng chat
                Clients.Caller.Message(name, message, makh, timenow, tnadmin.MaChatNguoiDung, imageUrl);
            }

        }

        public void GetMessages(int roomId)
        {
            // Lấy ra các tin nhắn trong phòng chat và gửi cho client yêu cầu
            var messages = db.TinNhanNhoms.Where(tn => tn.MaPhongChat == roomId).OrderBy(tn => tn.NgayGui).ToList();

            // Gửi danh sách tin nhắn về cho client
            foreach (var msg in messages)
            {
                NguoiDung kh = db.NguoiDungs.Find(msg.MaNguoiDung);
                string name = kh.HoTen;
                Clients.Caller.Message(name, msg.NoiDungChat, msg.MaNguoiDung, msg.NgayGui, msg.MaTinNhan, msg.LinkAnh);
            }
        }

        public void GetMessagesAdmin(long MaNguoiNhan, long MaNguoiGui)
        {
            // Lấy ra các tin nhắn trong phòng chat và gửi cho client yêu cầu
            var messagesAdmin = db.TinNhanNguoiDungs.Where(tn => tn.IdNguoiNhan == MaNguoiNhan && tn.IdNguoiGui == MaNguoiGui
                                                        || tn.IdNguoiNhan == MaNguoiGui && tn.IdNguoiGui == MaNguoiNhan).OrderBy(tn => tn.NgayGuiChat).ToList();

            // Gửi danh sách tin nhắn về cho client
            foreach (var msg in messagesAdmin)
            {
                NguoiDung kh = db.NguoiDungs.Find(msg.IdNguoiGui);
                string name = kh.HoTen;
                Clients.Caller.Message(name, msg.NoiDungChat, msg.IdNguoiGui, msg.NgayGuiChat, msg.MaChatNguoiDung, msg.LinkAnh);
            }
        }

        public void TaoMoi(string room_new, long? maks)
        {
            int count = 1; // khởi tạo biến đếm là 1

            var ph = db.PhongChats.ToList();
            if (ph.Count > 0)
            {
                foreach (var dem in ph)
                {
                    if (dem.TenPhongChat == room_new)
                    {
                        room_new = room_new + "(" + count.ToString() + ")"; // nếu phòng đã tồn tại, thêm số thứ tự vào tên phòng
                        count++; // tăng biến đếm lên 1
                    }
                }
            }

            var Room = new PhongChat();
            {
                Room.TenPhongChat = room_new;
                Room.MaKhachSan = maks;
            }
            db.PhongChats.Add(Room);
            db.SaveChanges();

            PhongChat phchat = db.PhongChats.FirstOrDefault(n => n.TenPhongChat == room_new);
            Clients.All.TaoMoi(phchat.MaPhongChat, phchat.TenPhongChat, phchat.MaKhachSan);
        }

        public void GetTaoMoi()
        {
            var phongchat = db.PhongChats.ToList();
            foreach (var dem in phongchat)
            {
                if (dem.MaKhachSan == null)
                {
                    Clients.Caller.TaoMoi(dem.MaPhongChat, dem.TenPhongChat, null);
                }
                else
                {
                    Clients.Caller.TaoMoi(dem.MaPhongChat, dem.TenPhongChat, dem.KhachSan.TenKhachSan);
                }
            }
        }

        public void GetTaoMoiManage(long? maks)
        {
            if (maks != null)
            {
                var phongchat = db.PhongChats.Where(m => m.MaKhachSan == maks || m.MaKhachSan == null).ToList();
                foreach (var dem in phongchat)
                {
                    Clients.Caller.TaoMoi(dem.MaPhongChat, dem.TenPhongChat, dem.MaKhachSan);
                }
            }
            else
            {
                var phongchat = db.PhongChats.Where(m => m.MaKhachSan == null).ToList();
                foreach (var dem in phongchat)
                {
                    Clients.Caller.TaoMoi(dem.MaPhongChat, dem.TenPhongChat, dem.MaKhachSan);
                }
            }
        }


        public void DeletePhong(long? id)
        {
            PhongChat phong = db.PhongChats.Find(id);
            if (phong != null)
            {
                var tn = db.TinNhanNhoms.Where(m => m.MaPhongChat == id).ToList();
                if (tn.Count > 0)
                {
                    foreach (var n in tn)
                    {
                        db.TinNhanNhoms.Remove(n);
                        db.SaveChanges();
                    }
                }
                db.PhongChats.Remove(phong);
            }
            db.SaveChanges();
            Clients.Caller.DaXoaPhong();
        }

        public void DeleteTinNhan(string id, long gui, long makh)
        {
            TinNhanNhom tn = db.TinNhanNhoms.FirstOrDefault(m => m.MaTinNhan == id && m.MaNguoiDung == gui);
            TinNhanNguoiDung tnAdmin = db.TinNhanNguoiDungs.FirstOrDefault(m => m.MaChatNguoiDung == id && m.IdNguoiGui == gui);
            if (tn != null)
            {
                long idphong = (long)tn.MaPhongChat;
                long kh = (long)tn.MaNguoiDung;
                db.TinNhanNhoms.Remove(tn);
                db.SaveChanges();
                Clients.Caller.TinNhanDaXoa(idphong, kh, false);
            }
            else if (tnAdmin != null)
            {
                long idphong = (long)tnAdmin.IdNguoiNhan;
                long kh = (long)tnAdmin.IdNguoiGui;
                db.TinNhanNguoiDungs.Remove(tnAdmin);
                db.SaveChanges();
                Clients.Caller.TinNhanDaXoa(idphong, kh, true);
            }
        }

        public string RadomCode()
        {
            int codeLength = 3; // Độ dài của mã xác minh
            Random random = new Random();
            string code = "";

            for (int i = 0; i < codeLength; i++)
            {
                int digit = random.Next(0, 9); // Lấy ngẫu nhiên một số từ 0 đến 9
                code += digit.ToString(); // Thêm số vào chuỗi mã xác minh
            }

            return code;
        }
    }
}

