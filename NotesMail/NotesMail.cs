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
        public static async Task<string> SendNoteAsync(Mail mailNotes)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://10.199.1.32:1234");

                var mail = new Mail()
                {
                    //To = "VNW0014732@VNFPG",
                    lsTO = mailNotes.lsTO,
                    lsCC = mailNotes.lsCC,
                    Subject = mailNotes.Subject,
                    SystemName = "冶金文件管理系統",
                    SystemOwner = "潘英俊(分機:6779)",
                    Content = mailNotes.Content,
                    Attachments = new List<AttachmentFile>()
                };

                if (mailNotes.LsAttachments != null)
                {
                    foreach (var fileLocation in mailNotes.LsAttachments)
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
                return "Debug OK";
#else
                var response = await client.PostAsync("/api/Mail", requestContent);
                return response.StatusCode.ToString();
#endif
            }
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
        public string To
        {
            get { return lsTO != null ? string.Join(",", lsTO.Select(r => $"{r}@VNFPG")) : ""; }
            set { }
        }
        public List<string> lsTO { get; set; }
        public string CC
        {
            get { return lsCC != null ? string.Join(",", lsCC.Select(r => $"{r}@VNFPG")) : ""; }
            set { }
        }
        public List<string> lsCC { get; set; }
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
