using BusinessLayer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NotesMail
{
    public class NotesMail
    {
        /// <summary>
        /// Gửi Notes FHS theo chuỗi User
        /// </summary>
        /// <param name="content"></param>
        /// <param name="subject"></param>
        /// <param name="to"></param>
        /// <param name="cc"></param>
        /// <param name="attachments"></param>
        /// <returns></returns>
        public static async Task<string> SendNoteAsync(string subject, string content, string to, string cc = "", List<string> attachments = null)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://10.199.1.32:1234");

                var mail = new Mail()
                {
                    To = "VNW0014732@VNFPG",
                    // To = to,
                    CC = cc,
                    Subject = subject,
                    Content = content,
                    SystemName = "冶金文管系統",
                    SystemOwner = "潘英俊(分機:6779)",
                    Attachments = new List<AttachmentFile>()
                };

                if (attachments != null)
                {
                    foreach (var fileLocation in attachments)
                    {
                        var file = File.Open(fileLocation, FileMode.Open);
                        var file_byteCode = new byte[file.Length];
                        file.Read(file_byteCode, 0, (int)file.Length);
                        var file_string = Convert.ToBase64String(file_byteCode);
                        mail.Attachments.Add(
                            new AttachmentFile()
                            {
                                Name = file.Name.Substring(file.Name.LastIndexOf('\\') + 1),
                                FileOfBase64String = file_string
                            });
                    }
                }

                var json_string = JsonConvert.SerializeObject(mail);
                var requestContent = new StringContent(json_string, Encoding.UTF8, "application/json");

#if DEBUG
                return $"Status: Debug Subject:{mail.Subject} To:{mail.To}"; ;
#else
                var response = await client.PostAsync("/api/Mail", requestContent);
                return $"Status:{response.StatusCode} Subject:{mail.Subject} To:{mail.To}";
#endif
            }
        }

        /// <summary>
        /// Gửi Notes FHS theo list User
        /// </summary>
        /// <param name="content"></param>
        /// <param name="subject"></param>
        /// <param name="tos"></param>
        /// <param name="ccs"></param>
        /// <param name="attachments"></param>
        /// <returns></returns>
        public static async Task<string> SendNoteAsync(string subject, string content, List<string> tos, List<string> ccs = null, List<string> attachments = null)
        {
            string to = string.Join(",", tos.Select(r => $"{r}@VNFPG"));
            string cc = ccs != null ? string.Join(",", ccs.Select(r => $"{r}@VNFPG")) : "";

            return await SendNoteAsync(content, subject, to, cc, attachments);
        }

        internal static void SaveFileHtml(string nameFile, string value)
        {
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DateTime.Today.ToString("yyyy"), DateTime.Today.ToString("MM"), DateTime.Today.ToString("dd"));
            Directory.CreateDirectory(folderPath);

            string pathFileSave = Path.Combine(folderPath, nameFile);

            File.WriteAllText(pathFileSave, value);
        }
    }

    public class Mail
    {
        public string To { get; set; }
        public string CC { get; set; }
        public List<AttachmentFile> Attachments { get; set; }
        public string Subject { get; set; }
        public string From { get; set; }
        public string Content { get; set; }
        public string SystemName { get; set; }
        public string SystemOwner { get; set; }
        public List<string> LsAttachments { get; set; }
    }

    public class AttachmentFile
    {
        public string Name { get; set; }
        public string FileOfBase64String { get; set; }
    }
}
