using System;

namespace KnowledgeSystem
{
    public partial class Function
    {
        public int Id { get; set; }
        public Nullable<int> IdParent { get; set; }
        public string DisplayName { get; set; }
        public Nullable<bool> Status { get; set; }
    }
}