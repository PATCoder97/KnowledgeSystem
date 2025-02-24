using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeSystem.Helpers
{
    public class DateTimeHelper
    {
        private static DateTimeHelper instance;

        public static DateTimeHelper Instance
        {
            get { if (instance == null) instance = new DateTimeHelper(); return instance; }
            private set { instance = value; }
        }

        private DateTimeHelper()
        {
        }

        public static bool IsWithinWorkingHours(DateTime time)
        {
            // Kiểm tra thứ (Monday = 1, Sunday = 7)
            DayOfWeek day = time.DayOfWeek;
            if (day == DayOfWeek.Sunday) return false; // Chủ nhật nghỉ
            if (day == DayOfWeek.Saturday || day >= DayOfWeek.Monday) // T2 -> T7
            {
                TimeSpan startWork = new TimeSpan(7, 30, 0); // 07:30
                TimeSpan endWork = new TimeSpan(17, 0, 0);   // 17:00
                TimeSpan currentTime = time.TimeOfDay;

                return currentTime >= startWork && currentTime <= endWork;
            }
            return false;
        }
    }
}
