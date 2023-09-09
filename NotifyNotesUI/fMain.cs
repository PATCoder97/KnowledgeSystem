using DataEF;
using Scriban;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Migrations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NotifyNotesUI
{
    public partial class fMain : Form
    {
        public fMain()
        {
            InitializeComponent();
            TrayMenuContext();
        }

        private Task serviceTask;
        private bool isRunning;

        private void TrayMenuContext()
        {
            notifyIcon1.ContextMenuStrip = new ContextMenuStrip();
            notifyIcon1.ContextMenuStrip.Items.Add("Show", null, this.MenuShow_Click);
            notifyIcon1.ContextMenuStrip.Items.Add("Exit", null, this.MenuExit_Click);
        }

        private void MenuExit_Click(object sender, EventArgs e)
        {
            isRunning = false;
            serviceTask.Wait();
            Application.Exit();
        }

        private void MenuShow_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
            Show();
        }

        private void fMain_Load(object sender, EventArgs e)
        {
            //notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;

            //notifyIcon1.BalloonTipText = "I am a NotifyIcon Balloon";

            //notifyIcon1.BalloonTipTitle = "Welcome Message";



            //notifyIcon1.ShowBalloonTip(1000);

            isRunning = true;
            AddItemToListBox("Start...");
            serviceTask = Task.Run(RunTasksAsync);
        }

        private void fMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;
                ShowInTaskbar = false;
                Hide();
            }
        }

        private void AddItemToListBox(string item)
        {
            listLogs.Invoke(new Action(() =>
            {
                listLogs.Items.Add(item);
            }));
        }

        string pathFolderSave = "C:\\Users\\ANHTUAN\\Desktop\\New folder";

        private async Task RunTasksAsync()
        {
            Task task1 = Task.Run(async () =>
            {
                while (isRunning)
                {
                    try
                    {
                        AddItemToListBox("NotifyDocUpdated recall");
                        await NotifyDocUpdated();
                        await Task.Delay(TimeSpan.FromSeconds(10));
                    }
                    catch (Exception ex)
                    {
                        AddItemToListBox("Task NotifyDocUpdated - An error occurred: " + ex.Message);
                    }
                }
            });

            Task task2 = Task.Run(async () =>
            {
                while (isRunning)
                {
                    try
                    {
                        AddItemToListBox("NotifyDocProcessing recall");
                        await NotifyDocProcessing();
                        await Task.Delay(TimeSpan.FromSeconds(10));
                    }
                    catch (Exception ex)
                    {
                        AddItemToListBox("Task Task 2 - An error occurred: " + ex.Message);
                    }
                }
            });

            await Task.WhenAll(task1, task2);
        }

        private async Task NotifyDocUpdated()
        {
            var templateContent = File.ReadAllText($@"{AppDomain.CurrentDomain.BaseDirectory}\html\f207_DocUpdated.html");
            var template = Template.Parse(templateContent);

            using (var db = new DBDocumentManagementSystemEntities())
            {
                var lsNotifyEditDocs = db.dt207_NotifyEditDoc.Where(r => string.IsNullOrEmpty(r.TimeNotifyNotes.ToString())).ToList();
                var lsUsers = db.Users.ToList();
                var lsBases = db.dt207_Base.ToList();
                var lsTypes = db.dt207_Type.ToList();

                var queryDocUpdated =
                    (from data in lsNotifyEditDocs
                     join docprocess in db.dt207_DocProgress on data.IdDocProcess equals docprocess.Id
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
                    File.WriteAllText(Path.Combine(pathFolderSave, nameFile), pageContent);

                    // Cập nhật ngày notify note len DB
                    var dataNotified = db.dt207_NotifyEditDoc.First(r => r.Id == item.dtdata.Id);
                    dataNotified.TimeNotifyNotes = DateTime.Now;

                    db.dt207_NotifyEditDoc.AddOrUpdate(dataNotified);
                    await db.SaveChangesAsync();

                    string logs = $"{templateData.Id} {templateData.Namevn} {templateData.Nametw} {dataNotified.TimeNotifyNotes}";
                    AddItemToListBox(logs);
                }
            }
        }

        private async Task NotifyDocProcessing()
        {
            var templateContent = File.ReadAllText($@"{AppDomain.CurrentDomain.BaseDirectory}\html\f207_DocProcessing.html");
            var template = Template.Parse(templateContent);

            using (var db = new DBDocumentManagementSystemEntities())
            {
                var lsUsers = db.Users.ToList();
                var lsTypes = db.dt207_Type.ToList();

                var lsDocProcessNotifys =
                    (from data in db.dt207_DocProgressInfo.Where(r => string.IsNullOrEmpty(r.TimeNotifyNotes.ToString()))
                     join processes in db.dt207_DocProgress on data.IdDocProgress equals processes.Id
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
                            subjectEvents = "通知文件：";
                        }
                        else if (events.StartsWith("退回"))
                        {
                            detailEvents = "被退回";
                            subjectEvents = "退回文件：";
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

                        var pageContent = template.Render(templateData);

                        string nameFile = $"{detailEvents} {DateTime.Now:HHmmss} {templateData.Id}.html";
                        File.WriteAllText(Path.Combine(pathFolderSave, nameFile), pageContent);
                    }

                    // Thông báo cho chủ quản
                    if (stepNow >= 0 && stepNow < maxStep)
                    {
                        int nextStep = stepNow + 1;
                        int idGroup = lsSteps.FirstOrDefault(r => r.IndexStep == nextStep).IdGroup;

                        var lsUserSigns = db.GroupUsers.Where(r => r.IdGroup == idGroup).Select(r => r.IdUser).ToList();

                        foreach (var userSign in lsUserSigns)
                        {
                            var userNotify = lsUsers.First(r => r.Id == userSign);
                            var userProcess = lsUsers.First(r => r.Id == bases.UserUpload);
                            var userUpload = lsUsers.First(r => r.Id == bases.UserProcess);
                            var displayName = bases.DisplayName.Split(new[] { "\r\n" }, StringSplitOptions.None);

                            var templateData = new
                            {
                                Usernotify = userNotify.DisplayName,
                                Id = idBase,
                                Typeof = lsTypes.First(r => r.Id == bases.IdTypes).DisplayName,
                                Namevn = displayName.Length > 1 ? displayName[1] : "",
                                Nametw = displayName[0],
                                Userupload = $"{userUpload.IdDepartment} {userUpload.Id} {userUpload.DisplayName}",
                                Userprocess = $"{userProcess.IdDepartment} {userProcess.Id} {userProcess.DisplayName}",
                            };

                            var pageContent = template.Render(templateData);

                            string nameFile = $"文件審查 {DateTime.Now:HHmmss} {templateData.Id}.html";
                            File.WriteAllText(Path.Combine(pathFolderSave, nameFile), pageContent);
                        }
                    }

                    var dataNotified = db.dt207_DocProgressInfo.First(r => r.Id == item.data.Id);
                    dataNotified.TimeNotifyNotes = DateTime.Now;

                    db.dt207_DocProgressInfo.AddOrUpdate(dataNotified);
                    await db.SaveChangesAsync();
                }
            }
        }
    }
}
