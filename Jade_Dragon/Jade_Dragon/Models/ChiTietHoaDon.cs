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
    
    public partial class ChiTietHoaDon
    {
        public long MaChiTietHoaDon { get; set; }
        public Nullable<System.DateTime> NgayDen { get; set; }
        public Nullable<System.DateTime> NgayDi { get; set; }
        public Nullable<long> Gia { get; set; }
        public Nullable<long> MaPhong { get; set; }
        public Nullable<long> MaHoaDon { get; set; }
        public string TenPhong { get; set; }
        public Nullable<bool> DaDen { get; set; }
        public Nullable<bool> HoanThanh { get; set; }
    
        public virtual HoaDon HoaDon { get; set; }
        public virtual PhongKhachSan PhongKhachSan { get; set; }
    }
}
