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
    
    public partial class dt307_ExamUser
    {
        public int Id { get; set; }
        public string ExamCode { get; set; }
        public string IdUser { get; set; }
        public string IdJob { get; set; }
        public Nullable<System.DateTime> SubmitTime { get; set; }
        public Nullable<int> Score { get; set; }
        public Nullable<bool> IsPass { get; set; }
        public string ExamData { get; set; }
    }
}
