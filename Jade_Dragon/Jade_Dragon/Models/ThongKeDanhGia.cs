//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Jade_Dragon.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ThongKeDanhGia
    {
        public long MaThongKeDanhGia { get; set; }
        public Nullable<long> MotSao { get; set; }
        public Nullable<long> HaiSao { get; set; }
        public Nullable<long> BaSao { get; set; }
        public Nullable<long> BonSao { get; set; }
        public Nullable<long> NamSao { get; set; }
        public Nullable<long> MaKhachSan { get; set; }
        public Nullable<long> TongSao { get; set; }
    
        public virtual KhachSan KhachSan { get; set; }
    }
}
