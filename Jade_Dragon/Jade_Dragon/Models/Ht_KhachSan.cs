using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jade_Dragon.Models
{
    public class Ht_KhachSan
    {
        public KhachSan ks { get; set; }
        public List<AnhKhachSan> anhks { get; set; }
        public PhongKhachSan phks { get; set; }
        public List<AnhPhongKhachSan> anhph { get; set; }
    }
}