﻿using DevExpress.XtraBars.Docking2010.Views.Widget;
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
        public static string EncryptionFileName(string filePath, int length = 64)
        {
            // Lấy phần mở rộng của file
            string extension = Path.GetExtension(filePath).TrimStart('.');

            // Tạo chuỗi ngẫu nhiên
            string randomStringPart = GenerateRandomString(16);

            // Tạo GUID cho phần mã hóa
            string guidPart = Guid.NewGuid().ToString("N");

            // Kết hợp chuỗi ngẫu nhiên, phần mở rộng file và GUID
            string combinedString = $"{randomStringPart}{extension}tp{guidPart}";

            // Nếu kết quả vượt quá chiều dài yêu cầu, cắt bớt
            if (combinedString.Length > length) combinedString = combinedString.Substring(0, length);

            // Nếu chuỗi ngắn hơn chiều dài yêu cầu, thêm ký tự ngẫu nhiên để đủ độ dài
            if (combinedString.Length < length) combinedString = combinedString + GenerateRandomString(length - combinedString.Length);
            return combinedString;
        }

        public static string EncryptPass(string password)
        {
            int lengPass = password.Length;

            // Generate three different salts
            string salt1 = GenerateRandomString(lengPass);
            string salt2 = DateTime.Now.ToString("ssmmHHddMMyyyyssmmHHddMMyyyy").Substring(0, lengPass);
            int lengSalt3 = password.Length <= 16 ? lengPass : 64 - lengPass * 3;
            string sal3 = GenerateRandomString(lengSalt3, false, true);

            // Combine the salts and password
            string[] strings = new[] { salt1, salt2, password, sal3 };

            // Use LINQ to interleave characters and reverse the order
            string result = Enumerable.Range(0, lengPass)
                .Select(i => strings.Where(chuoi => i < chuoi.Length)
                .Select(chuoi => chuoi[i]))
                .SelectMany(chars => chars)
                .Reverse()
                .Aggregate(new StringBuilder(), (sb, c) => sb.Append(c)).ToString();

            // Ensure the result has a length of 64 characters
            return result.Length == 64 ? result : $"{GenerateRandomString(63 - result.Length, true, true)}|{result}";
        }

        public static string DecryptPass(string encryptedText)
        {
            // Split the encrypted text using the '|' character as a delimiter
            string[] splitText = encryptedText.Split('|');
            string originalString = splitText.Length > 1 ? splitText.Last() : splitText.First();

            // Reverse the original string
            string reversedString = new string(originalString.Reverse().ToArray());

            // Extract salt3 from the reversed string
            var salt3 = new string(Enumerable.Range(0, reversedString.Length / 4)
                              .Select(i => reversedString[i * 4 + 3]).ToArray());

            // Count non-alphanumeric characters in salt3
            int count = salt3.Count(c => !char.IsLetterOrDigit(c));

            if (count == 0)
            {
                return "";
            }

            // Extract characters from reversed string to reconstruct the password
            string first = reversedString.Substring(0, count * 4);
            var result = new string(Enumerable.Range(0, first.Length / 4)
                              .Select(i => first[i * 4 + 2]).ToArray());

            // Remove salt3, adjust the group size to 3
            if (reversedString.Length != count * 4)
            {
                string second = reversedString.Substring(count * 4, 64 - (count * 4));
                result += new string(Enumerable.Range(0, second.Length / 3)
                                 .Select(i => second[i * 3 + 2]).ToArray());
            }

            return result;
        }

        private static string GenerateRandomString(int length, bool includeLetters = true, bool includeSpecialChars = false)
        {
            // Các ký tự có thể sử dụng trong chuỗi ngẫu nhiên
            string letterChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string specialChars = "!@#$%^&*()_+-=[]{};:'\",.<>?";

            // Kết hợp các ký tự dựa trên tham số
            string chars = "";
            if (includeLetters) chars += letterChars;
            if (includeSpecialChars) chars += specialChars;

            if (string.IsNullOrEmpty(chars))
                throw new ArgumentException("Phải bao gồm ít nhất một loại ký tự (chữ cái hoặc ký tự đặc biệt).");

            // Tạo đối tượng Random với seed từ GUID
            Random random = new Random(Guid.NewGuid().GetHashCode());

            // Tạo chuỗi ngẫu nhiên
            string randomString = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return randomString;
        }
    }
}
