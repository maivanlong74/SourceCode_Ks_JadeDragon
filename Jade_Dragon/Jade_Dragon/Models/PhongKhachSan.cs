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
    
    public partial class PhongKhachSan
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PhongKhachSan()
        {
            this.AnhPhongKhachSans = new HashSet<AnhPhongKhachSan>();
            this.ChiTietHoaDons = new HashSet<ChiTietHoaDon>();
        }
    
        public long MaPhong { get; set; }
        public Nullable<long> MaKhachSan { get; set; }
        public Nullable<long> MaSoTang { get; set; }
        public Nullable<long> MaSoPhong { get; set; }
        public string TenPhong { get; set; }
        public string LoaiHinh { get; set; }
        public Nullable<long> Gia { get; set; }
        public Nullable<bool> VIP { get; set; }
        public Nullable<bool> TrangThai { get; set; }
        public Nullable<bool> KhoaPhong { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AnhPhongKhachSan> AnhPhongKhachSans { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; }
        public virtual KhachSan KhachSan { get; set; }
        public virtual SoPhongKhachSan SoPhongKhachSan { get; set; }
        public virtual SoTangKhachSan SoTangKhachSan { get; set; }
    }
}