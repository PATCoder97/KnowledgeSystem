using System;

namespace KnowledgeSystem
{
    public partial class AppForm
    {
        public int Id { get; set; }
        public Nullable<int> ParentId { get; set; }
        public Nullable<int> GroupId { get; set; }
        public Nullable<int> IndexRow { get; set; }
        public string NameForm { get; set; }
        public string DisplayName { get; set; }
    }
}