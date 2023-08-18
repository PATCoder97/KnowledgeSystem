using System;

namespace KnowledgeSystem
{
    public partial class dt207_Security
    {
        public int Id { get; set; }
        public string IdKnowledgeBase { get; set; }
        public Nullable<int> IdGroup { get; set; }
        public string IdUser { get; set; }
        public Nullable<bool> ReadInfo { get; set; }
        public Nullable<bool> UpdateInfo { get; set; }
        public Nullable<bool> DeleteInfo { get; set; }
        public Nullable<bool> SearchInfo { get; set; }
        public Nullable<bool> ReadFile { get; set; }
        public Nullable<bool> SaveFile { get; set; }
    }
}