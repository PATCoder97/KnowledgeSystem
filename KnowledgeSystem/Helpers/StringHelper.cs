using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public static string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return "untitled";

            // 🔹 Cách 1: dùng regex để loại ký tự đặc biệt
            string invalidChars = new string(Path.GetInvalidFileNameChars());
            string invalidReStr = string.Format("[{0}]", Regex.Escape(invalidChars));

            string cleanName = Regex.Replace(fileName, invalidReStr, "_");

            // 🔹 Cắt độ dài nếu quá 100 ký tự (tránh lỗi khi lưu)
            if (cleanName.Length > 100)
                cleanName = cleanName.Substring(0, 100);

            // 🔹 Trim khoảng trắng thừa
            cleanName = cleanName.Trim();

            // 🔹 Nếu rỗng thì đặt lại mặc định
            if (string.IsNullOrWhiteSpace(cleanName))
                cleanName = "untitled";

            return cleanName;
        }
    }
}
