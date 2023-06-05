using Jade_Dragon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jade_Dragon.common
{
    public class InfoPhong
    {
        private Connect db = new Connect();
        public List<SoTangKhachSan> LayDanhSachSoTang(long MaKs)
        {
            KhachSan ks = db.KhachSans.Find(MaKs);
            if (ks == null)
            {
                return new List<SoTangKhachSan>();
            }
            List<SoTangKhachSan> danhSachSoTang = new List<SoTangKhachSan>();

            for (int i = 1; i <= ks.SoTang; i++)
            {
                danhSachSoTang.Add(new SoTangKhachSan { MaSoTang = i, SoTang = "B" + i });
            }
            return danhSachSoTang;
        }
    }
}