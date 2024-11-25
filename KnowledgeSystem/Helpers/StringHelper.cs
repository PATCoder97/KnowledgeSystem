using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeSystem.Helpers
{
    public class StringHelper
    {
        public static bool CheckUpcase(string inputString, int percent)
        {
            if (string.IsNullOrEmpty(inputString) || percent < 0 || percent > 100)
                return false; // Kiểm tra đầu vào hợp lệ

            // Đếm số chữ cái viết hoa
            int upperCount = inputString.Count(char.IsUpper);

            // Đếm tổng số chữ cái (cả viết hoa và viết thường)
            int letterCount = inputString.Count(char.IsLetter);

            // Kiểm tra tỷ lệ viết hoa
            return letterCount > 0 && (double)upperCount / letterCount * 100 > percent;
        }
    }
}
