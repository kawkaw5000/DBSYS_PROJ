//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EcommerceShop.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class Tbl_MemberInfo
    {
        public int id { get; set; }
        public Nullable<int> MemberId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string UserImage { get; set; }
        public Nullable<int> StoreId { get; set; }
        public string userId { get; set; }
    
        public virtual Tbl_Members Tbl_Members { get; set; }
        public virtual Tbl_Store Tbl_Store { get; set; }
        public virtual Tbl_MemberInfo Tbl_MemberInfo1 { get; set; }
        public virtual Tbl_MemberInfo Tbl_MemberInfo2 { get; set; }
    }
}
