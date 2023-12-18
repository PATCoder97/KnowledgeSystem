using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    internal class EncryptionHelper
    {
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
    }
}
