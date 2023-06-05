using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jade_Dragon.Models
{
    public class Chat_Admin
    {
        public List<PhongChat> phchat { set; get; }
        public List<NguoiDung> kh { set; get; }
        public List<KhachSan> ks { set; get; }
    }
}