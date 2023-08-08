using System;

namespace KnowledgeSystem
{
    public class dm_Departments
    {
        public string Id { get; set; }
        public int IdChild { get; set; }
        public int IdParent { get; set; }
        public string DisplayName { get; set; }
    }
}