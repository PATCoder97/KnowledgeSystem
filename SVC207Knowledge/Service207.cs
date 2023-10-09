using DataAccessLayer;
using Scriban;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Assemblies;
using System.Data;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SVC207Knowledge
{
    public partial class Service207 : ServiceBase
    {
        public Service207()
        {
            InitializeComponent();
        }

        int defaulDelay = 5; //minute
        private Task serviceTask;
        private bool isRunning;
        string assemblyPath = string.Empty;

        protected override void OnStart(string[] args)
        {
            isRunning = true;
            assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            AppendRowToFileAsync("Start");
            serviceTask = Task.Run(RunTasksAsync);
        }

        protected override void OnStop()
        {
            isRunning = false;
            AppendRowToFileAsync("Stop");
            try
            {
                serviceTask.Wait();
            }
            catch (Exception ex)
            {
                AppendRowToFileAsync("Task - An error occurred: " + ex.Message);
            }
        }

        private void AppendRowToFileAsync(string msg)
        {
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DateTime.Today.ToString("yyyy"), DateTime.Today.ToString("MM"));
            Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, DateTime.Today.ToString("dd") + ".txt");

            using (StreamWriter writer = File.AppendText(filePath))
            {
                writer.WriteLine($"{DateTime.Now:hh:mm:ss tt}: {msg}");
            }
        }

        private void SaveFileHtml(string nameFile, string value)
        {
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DateTime.Today.ToString("yyyy"), DateTime.Today.ToString("MM"), DateTime.Today.ToString("dd"));
            Directory.CreateDirectory(folderPath);

            string pathFileSave = Path.Combine(folderPath, nameFile);

            File.WriteAllText(pathFileSave, value);
        }

        private async Task RunTasksAsync()
        {
            var lsTasks = new List<Task>();

            Task task1 = Task.Run(async () =>
            {
                while (isRunning)
                {
                    try
                    {
                        AppendRowToFileAsync("Recal: NotifyDocUpdated");
                        await NotifyDocUpdated();
                    }
                    catch (Exception ex)
                    {
                        AppendRowToFileAsync("Task NotifyDocUpdated - An error occurred: " + ex.Message);
                    }
                    await Task.Delay(TimeSpan.FromMinutes(defaulDelay));
                }
            });
            lsTasks.Add(task1);

            Task task2 = Task.Run(async () =>
            {
                while (isRunning)
                {
                    try
                    {
                        AppendRowToFileAsync("Recal: NotifyDocProcessing");
                        await NotifyDocProcessing();
                    }
                    catch (Exception ex)
                    {
                        AppendRowToFileAsync("Task NotifyDocProcessing - An error occurred: " + ex.Message);
                    }
                    await Task.Delay(TimeSpan.FromMinutes(defaulDelay));
                }
            });
            lsTasks.Add(task2);

            await Task.WhenAll(lsTasks);
        }

        private async Task NotifyDocUpdated()
        {
            var templateContent = File.ReadAllText($@"{AppDomain.CurrentDomain.BaseDirectory}\html\f207_DocUpdated.html");
            var template = Template.Parse(templateContent);

            using (var db = new DBDocumentManagementSystemEntities())
            {
                var lsNotifyEditDocs = db.dt207_NotifyEditDoc.Where(r => string.IsNullOrEmpty(r.TimeNotifyNotes.ToString())).ToList();
                var lsUsers = db.dm_User.ToList();
                var lsBases = db.dt207_Base.ToList();
                var lsTypes = db.dt207_Type.ToList();

                var queryDocUpdated =
                    (from data in lsNotifyEditDocs
                     join docprocess in db.dt207_DocProcessing on data.IdDocProcess equals docprocess.Id
                     select new
                     {
                         data.Id,
                         docprocess.IdKnowledgeBase,
                         docprocess.IdUserProcess,
                         data.IdUserNotify,
                         data.TimeNotify,
                         docprocess.Change,
                     } into dtdata
                     join bases in lsBases on dtdata.IdKnowledgeBase equals bases.Id
                     select new
                     {
                         dtdata,
                         bases
                     }).ToList();

                foreach (var item in queryDocUpdated)
                {
                    var displayName = item.bases.DisplayName.Split(new[] { "\r\n" }, StringSplitOptions.None);
                    var userNotify = lsUsers.First(r => r.Id == item.dtdata.IdUserNotify);
                    var UserUpload = lsUsers.First(r => r.Id == item.bases.UserUpload);
                    var UserProcess = lsUsers.First(r => r.Id == item.bases.UserProcess);
                    var UserUpdate = lsUsers.First(r => r.Id == item.dtdata.IdUserProcess);

                    var templateData = new
                    {
                        Usernotify = userNotify.DisplayName,
                        Id = item.dtdata.IdKnowledgeBase,
                        Timeupdate = $"{item.dtdata.TimeNotify:yyyy/MM/dd HH:mm:ss}",
                        Place = item.dtdata.Change,
                        Userupdate = $"{UserUpdate.IdDepartment} {UserUpdate.Id} {UserUpdate.DisplayName}",
                        Typeof = lsTypes.First(r => r.Id == item.bases.IdTypes).DisplayName,
                        Namevn = displayName.Length > 1 ? displayName[1] : "",
                        Nametw = displayName[0],
                        Userupload = $"{UserUpload.IdDepartment} {UserUpload.Id} {UserUpload.DisplayName}",
                        Userprocess = $"{UserProcess.IdDepartment} {UserProcess.Id} {UserProcess.DisplayName}",
                    };

                    var pageContent = template.Render(templateData);

                    string nameFile = $"文件更新提示 {DateTime.Now:HHmmss} {templateData.Id}.html";
                    SaveFileHtml(nameFile, pageContent);

                    // Send notes
                    Mail mailNotes = new Mail()
                    {
                        To = userNotify.Id,
                        Subject = $"通知文件 : 庫通知通知您文件「 {templateData.Id}」已完成更新。",
                        Content = pageContent
                    };
                    string res = await NotesMail.SendNoteAsync(mailNotes);

                    // Cập nhật ngày notify note len DB
                    var dataNotified = db.dt207_NotifyEditDoc.First(r => r.Id == item.dtdata.Id);
                    dataNotified.TimeNotifyNotes = DateTime.Now;
                    db.dt207_NotifyEditDoc.AddOrUpdate(dataNotified);

                    // Cập nhật log vào DB
                    sys_Log log = new sys_Log()
                    {
                        Thread = "207 KnowledgeSystem",
                        Level = "Info",
                        Logger = "Document updated",
                        Message = res
                    };
                    db.sys_Log.Add(log);

                    await db.SaveChangesAsync();
                }
            }
        }

        private async Task NotifyDocProcessing()
        {
            var templateContentOwner = File.ReadAllText($@"{AppDomain.CurrentDomain.BaseDirectory}\html\f207_DocProcessingOwner.html");
            var templateOwner = Template.Parse(templateContentOwner);

            var templateContentSigner = File.ReadAllText($@"{AppDomain.CurrentDomain.BaseDirectory}\html\f207_DocProcessingSigner.html");
            var templateSigner = Template.Parse(templateContentSigner);

            using (var db = new DBDocumentManagementSystemEntities())
            {
                var lsUsers = db.dm_User.ToList();
                var lsTypes = db.dt207_Type.ToList();

                var lsDocProcessNotifys =
                    (from data in db.dt207_DocProgressInfo.Where(r => string.IsNullOrEmpty(r.TimeNotifyNotes.ToString()))
                     join processes in db.dt207_DocProcessing on data.IdDocProgress equals processes.Id
                     select new
                     {
                         data,
                         processes
                     }).ToList();

                foreach (var item in lsDocProcessNotifys)
                {
                    var lsSteps = db.dm_StepProgress.Where(r => r.IdProgress == item.processes.IdProgress).ToList();
                    int maxStep = lsSteps.Max(r => r.IdProgress);
                    int stepNow = item.data.IndexStep;

                    string idBase = item.processes.IdKnowledgeBase;
                    var bases = db.dt207_Base.FirstOrDefault(r => r.Id == idBase);

                    if (bases == null) continue;


                    // Thông báo cho người thực hiện
                    if (stepNow < 0 || stepNow == maxStep)
                    {
                        var userNotify = lsUsers.First(r => r.Id == item.processes.IdUserProcess);
                        var userProcess = lsUsers.First(r => r.Id == item.data.IdUserProcess);

                        string events = item.data.Descriptions;
                        string detailEvents;
                        string subjectEvents;

                        if (events.StartsWith("確認"))
                        {
                            detailEvents = "已確認完畢";
                            subjectEvents = "通知文件";
                        }
                        else if (events.StartsWith("退回"))
                        {
                            detailEvents = "被退回";
                            subjectEvents = "退回文件";
                        }
                        else if (events.StartsWith("取消"))
                        {
                            detailEvents = "已取消";
                            subjectEvents = "取消文件";
                        }
                        else
                        {
                            detailEvents = string.Empty;
                            subjectEvents = string.Empty;
                        }

                        var templateData = new
                        {
                            Usernotify = userNotify.DisplayName,
                            Id = idBase,
                            Events = events,
                            Userprocess = $"{userProcess.IdDepartment} {userProcess.Id} {userProcess.DisplayName}",
                            Timeprocess = $"{item.data.TimeStep:yyyy/MM/dd HH:mm:ss}",
                            Detailevents = detailEvents
                        };

                        var pageContent = templateOwner.Render(templateData);

                        string nameFile = $"{detailEvents} {DateTime.Now:HHmmss} {templateData.Id}.html";
                        SaveFileHtml(nameFile, pageContent);

                        // Send notes
                        Mail mailNotes = new Mail()
                        {
                            To = userNotify.Id,
                            Subject = $"{subjectEvents} : 庫通知通知您文件「 {templateData.Id}」{detailEvents}。",
                            Content = pageContent
                        };
                        string res = await NotesMail.SendNoteAsync(mailNotes);

                        // Cập nhật log vào DB
                        sys_Log log = new sys_Log()
                        {
                            Thread = "207 KnowledgeSystem",
                            Level = "Info",
                            Logger = "Document processing to owner",
                            Message = res
                        };
                        db.sys_Log.Add(log);
                    }

                    // Thông báo cho chủ quản
                    if (stepNow >= 0 && stepNow < maxStep)
                    {
                        int nextStep = stepNow + 1;
                        int idGroup = lsSteps.FirstOrDefault(r => r.IndexStep == nextStep).IdGroup;

                        var lsUserSigns = db.dm_GroupUser.Where(r => r.IdGroup == idGroup).Select(r => r.IdUser).ToList();

                        var userProcess = lsUsers.First(r => r.Id == bases.UserUpload);
                        var userUpload = lsUsers.First(r => r.Id == bases.UserProcess);
                        var displayName = bases.DisplayName.Split(new[] { "\r\n" }, StringSplitOptions.None);

                        var templateData = new
                        {
                            Id = idBase,
                            Typeof = lsTypes.First(r => r.Id == bases.IdTypes).DisplayName,
                            Namevn = displayName.Length > 1 ? displayName[1] : "",
                            Nametw = displayName[0],
                            Userupload = $"{userUpload.IdDepartment} {userUpload.Id} {userUpload.DisplayName}",
                            Userprocess = $"{userProcess.IdDepartment} {userProcess.Id} {userProcess.DisplayName}",
                        };

                        var pageContent = templateSigner.Render(templateData);

                        string nameFile = $"文件審查 {DateTime.Now:HHmmss} {templateData.Id}.html";
                        SaveFileHtml(nameFile, pageContent);

                        // Send notes
                        Mail mailNotes = new Mail()
                        {
                            lsTO = lsUserSigns,
                            Subject = $"處理文件 : 庫通知通知您文件「 {templateData.Id}」需審查。",
                            Content = pageContent
                        };
                        string res = await NotesMail.SendNoteAsync(mailNotes);

                        // Cập nhật log vào DB
                        sys_Log log = new sys_Log()
                        {
                            Thread = "207 KnowledgeSystem",
                            Level = "Info",
                            Logger = "Document processing to signer",
                            Message = res
                        };
                        db.sys_Log.Add(log);
                    }

                    // Cập nhật ngày thông báo notes lên DB
                    var dataNotified = db.dt207_DocProgressInfo.First(r => r.Id == item.data.Id);
                    dataNotified.TimeNotifyNotes = DateTime.Now;

                    db.dt207_DocProgressInfo.AddOrUpdate(dataNotified);
                    await db.SaveChangesAsync();
                }
            }
        }
    }
}
