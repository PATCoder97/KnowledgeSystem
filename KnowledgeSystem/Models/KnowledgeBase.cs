using System;

namespace KnowledgeSystem
{
    public partial class KnowledgeBase
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string UserRequest { get; set; }
        public Nullable<int> IdTypes { get; set; }
        public string Keyword { get; set; }
        public string UserUpload { get; set; }
        public Nullable<System.DateTime> UploadDate { get; set; }
    }
}