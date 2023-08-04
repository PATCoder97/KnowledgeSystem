using System;

namespace KnowledgeSystem
{
    public class DocProgressInfo
    {
        public int Id { get; set; }
        public int IdDocProgress { get; set; }
        public DateTime TimeStep { get; set; }
        public int IndexStep { get; set; }
        public string IdUserProcess { get; set; }
        public string Descriptions { get; set; }
    }
}