using System;

namespace KnowledgeSystem
{
    public partial class Group
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Describe { get; set; }
        public Nullable<int> Prioritize { get; set; }
    }
}