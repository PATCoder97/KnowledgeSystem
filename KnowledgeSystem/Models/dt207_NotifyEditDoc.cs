using System;

namespace KnowledgeSystem
{
    public class dt207_NotifyEditDoc
    {
        public int Id { get; set; }
        public DateTime TimeNotify { get; set; }
        public int IdDocProcess { get; set; }
        public string IdUserNotify { get; set; }
        public bool IsRead { get; set; }
        public DateTime? TimeNotifyNotes { get; set; }
    }
}