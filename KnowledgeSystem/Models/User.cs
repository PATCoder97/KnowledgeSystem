using System;

namespace KnowledgeSystem
{
    public partial class User
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public Nullable<int> IdRole { get; set; }
        public Nullable<System.DateTime> DateCreate { get; set; }
        public string IdDepartment { get; set; }
        public string SecondaryPassword { get; set; }
    }
}