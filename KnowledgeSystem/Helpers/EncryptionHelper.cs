using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeSystem.Helpers
{
    public class EncryptionHelper
    {
        public static string EncryptionFileName(string fileName)
        {
            string extension = Path.GetExtension(fileName).TrimStart('.');

            char[] charArray = TPConfigs.LoginUser.Id.Substring(3, 7).ToCharArray();
            Array.Reverse(charArray);

            string string1 = GenerateRandomString(16);
            string string3 = $"{new string(charArray)}{extension}tp";
            string string4 = DateTime.Now.ToString("ssmmHHddMMyy");

            string[] strings = new[] { string1, string3, string4 };

            string result = Enumerable.Range(0, strings.Max(chuoi => chuoi.Length))
                .Select(i => strings.Where(chuoi => i < chuoi.Length)
                .Select(chuoi => chuoi[i]))
                .SelectMany(chars => chars)
                .Aggregate(new StringBuilder(), (sb, c) => sb.Append(c)).ToString();

            return $"{GenerateRandomString(63 - result.Length)}-{result}";
        }

        static string GenerateRandomString(int length)
        {
            Random random = new Random();
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            // Tạo một chuỗi ngẫu nhiên bằng cách chọn ký tự từ danh sách cho đến khi đạt được độ dài mong muốn
            string randomFileName = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return randomFileName;
        }
    }
}
