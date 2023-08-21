using System;

namespace KnowledgeSystem
{
    public class dt207_DocProgress
    {
        public int Id { get; set; }
        public string IdKnowledgeBase { get; set; }
        public int IdProgress { get; set; }
        public bool IsSuccess { get; set; }
        public bool IsComplete { get; set; }
        public string Descriptions { get; set; }
        public string IdUserProcess { get; set; }
        public string Change { get; set; }
    }
}