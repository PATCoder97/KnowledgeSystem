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
            //string string4 = DateTime.Now.ToString("ssmmHHddMMyy");
            string string4 = Guid.NewGuid().ToString("N");

            string[] strings = new[] { string1, string3, string4 };

            string result = Enumerable.Range(0, strings.Max(chuoi => chuoi.Length))
                .Select(i => strings.Where(chuoi => i < chuoi.Length)
                .Select(chuoi => chuoi[i]))
                .SelectMany(chars => chars)
                .Aggregate(new StringBuilder(), (sb, c) => sb.Append(c)).ToString();

            return $"{GenerateRandomString(63 - result.Length)}-{result}";
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

        static string GenerateRandomString(int length, bool includeLetters = true, bool includeSpecialChars = false)
        {
            Guid guid = Guid.NewGuid();
            byte[] guidBytes = guid.ToByteArray();
            int seed = BitConverter.ToInt32(guidBytes, 0);
            Random random = new Random(seed);

            string letterChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string numberChars = "!@#$%^&*()_+-=[]{};:'\",.<>?";
            string chars = "";

            if (includeLetters && includeSpecialChars)
                chars = letterChars + numberChars;
            else if (includeLetters)
                chars = letterChars;
            else if (includeSpecialChars)
                chars = numberChars;
            else
                chars = string.Empty; // No valid characters selected

            // Create a random string by selecting characters from the character set until the desired length is reached
            string randomString = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return randomString;
        }
    }
}
