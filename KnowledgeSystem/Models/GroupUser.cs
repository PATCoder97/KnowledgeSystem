using System;

namespace KnowledgeSystem
{
    public partial class GroupUser
    {
        public int Id { get; set; }
        public Nullable<int> IdGroup { get; set; }
        public string IdUser { get; set; }
    }
}