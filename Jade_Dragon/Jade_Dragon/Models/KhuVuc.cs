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
    
    public partial class KhuVuc
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public KhuVuc()
        {
            this.KhachSans = new HashSet<KhachSan>();
        }
    
        public long MaKhuVuc { get; set; }
        public string TenKhuVuc { get; set; }
        public string KinhDo { get; set; }
        public string ViDo { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KhachSan> KhachSans { get; set; }
    }
}