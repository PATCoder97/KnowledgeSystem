using BusinessLayer;
using DataAccessLayer;
using Logger;
using Scriban;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NotesMail
{
    public class Mail301SafetyCert
    {
        static TPLogger logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName);

        public static async Task Notify301DeptChange()
        {
            var mails = sys_NotesMailBUS.Instance.GetListNotNotifyByThread("301");

            foreach (var mail in mails)
            {
                string nameFile = $"DeptChange {DateTime.Now:HHmmss}.html";
                NotesMail.SaveFileHtml(nameFile, mail.Content);

                // Send notes

                string res = await NotesMail.SendNoteAsync(mail.Subjects, mail.Content, mail.ToUsers);
                await Console.Out.WriteLineAsync(res);
                logger.Info(MethodBase.GetCurrentMethod().ReflectedType.Name, res);

                mail.TimeNotify = DateTime.Now;
                sys_NotesMailBUS.Instance.AddOrUpdate(mail);
#if DEBUG
#else
                // Cập nhật ngày thông báo notes lên DB
#endif
            }
        }
    }
}
