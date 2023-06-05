using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jade_Dragon.Models
{
    public class Cart
    {
        public PhongKhachSan htphong { get; set; }
        public int htsoluong { set; get; }
        public decimal TongTien()
        {
            return (decimal)(htsoluong * htphong.Gia);
        }
        public decimal TongSoLuong()
        {
            return (decimal)(htsoluong);
        }
        public DateTime NgayDen { set; get; }       
        public DateTime NgayDi { set; get; }
    }

}