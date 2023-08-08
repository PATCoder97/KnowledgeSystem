using System;

namespace KnowledgeSystem
{
    public class KnowledgeHistoryGetFile
    {
        public int Id { get; set; }
        public DateTime TimeGet { get; set; }
        public string IdKnowledgeBase { get; set; }
        public string KnowledgeAttachmentName { get; set; }
        public string IdUser { get; set; }
    }
}