//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataAccessLayer
{
    using System;
    using System.Collections.Generic;
    
    public partial class dt201_Forms
    {
        public int Id { get; set; }
        public int IdBase { get; set; }
        public string Code { get; set; }
        public string DisplayName { get; set; }
        public System.DateTime UploadTime { get; set; }
        public string UploadUser { get; set; }
        public Nullable<int> AttId { get; set; }
        public Nullable<bool> IsProcessing { get; set; }
        public Nullable<bool> DigitalSign { get; set; }
        public string DisplayNameVN { get; set; }
        public Nullable<bool> IsCancel { get; set; }
        public string NextStepProg { get; set; }
        public string Descript { get; set; }
        public Nullable<bool> IsDel { get; set; }
        public Nullable<System.DateTime> DelTime { get; set; }
    }
}
