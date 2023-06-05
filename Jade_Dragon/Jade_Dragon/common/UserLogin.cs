using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jade_Dragon.common
{
    [Serializable]
    public class UserLogin
    {
        public string TenDn { set; get; }
        public int DaXacMinh { set; get; }
        public string IDGroup { set; get; }
    }
}