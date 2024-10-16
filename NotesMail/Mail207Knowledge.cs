﻿using BusinessLayer;
using DataAccessLayer;
using Logger;
using Scriban;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NotesMail
{
    public class Mail207Knowledge
    {
        static TPLogger logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName);

        static dt207_BaseBUS _dt207_BaseBUS = new dt207_BaseBUS();

        public static async Task Notify207DocProcessing()
        {
            var templateContentOwner = File.ReadAllText($@"{AppDomain.CurrentDomain.BaseDirectory}\html\f207_DocProcessingOwner.html");
            var templateOwner = Template.Parse(templateContentOwner);

            var templateContentSigner = File.ReadAllText($@"{AppDomain.CurrentDomain.BaseDirectory}\html\f207_DocProcessingSigner.html");
            var templateSigner = Template.Parse(templateContentSigner);

            var lsUsers = dm_UserBUS.Instance.GetList();
            var lsTypes = dt207_TypeBUS.Instance.GetList();

            var lsDocProcessNotNotifys = dt207_DocProcessingInfoBUS.Instance.GetListNotNotify();
            var lsProcesses = dt207_DocProcessingBUS.Instance.GetList();
            var lsSteps = dm_StepProgressBUS.Instance.GetList();

            var lsDocProcessNotifys = (from data in lsDocProcessNotNotifys
                                       join processes in lsProcesses on data.IdDocProgress equals processes.Id
                                       select new
                                       {
                                           data,
                                           processes
                                       }).ToList();

            foreach (var processNotify in lsDocProcessNotifys)
            {
                var lsStepThisItem = lsSteps.Where(r => r.IdProgress == processNotify.processes.IdProgress).ToList();
                int maxStep = lsStepThisItem.Max(r => r.IndexStep);
                int stepNow = processNotify.data.IndexStep;

                string idBase = processNotify.processes.IdKnowledgeBase;
                dt207_Base bases = _dt207_BaseBUS.GetItemByOnlyId(idBase);

                if (bases == null) continue;

                // Thông báo cho người thực hiện
                if (stepNow < 0 || stepNow == maxStep)
                {
                    var userNotify = lsUsers.First(r => r.Id == processNotify.processes.IdUserProcess);
                    var userProcess = lsUsers.First(r => r.Id == processNotify.data.IdUserProcess);

                    string events = processNotify.data.Descriptions;
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
                        Timeprocess = $"{processNotify.data.TimeStep:yyyy/MM/dd HH:mm:ss}",
                        Detailevents = detailEvents
                    };

                    var pageContent = templateOwner.Render(templateData);

                    string nameFile = $"{detailEvents} {DateTime.Now:HHmmss} {templateData.Id}.html";
                    NotesMail.SaveFileHtml(nameFile, pageContent);

                    // Send notes
                    var lsTO = new List<string>() { userNotify.Id };
                    string subject = $"{subjectEvents} : 知識庫系統通知您文件「 {templateData.Id}」{detailEvents}。";

                    string res = await NotesMail.SendNoteAsync(subject, pageContent, lsTO);
                    await Console.Out.WriteLineAsync(res);
                    logger.Info(MethodBase.GetCurrentMethod().ReflectedType.Name, res);
                }

                // Thông báo cho chủ quản
                if (stepNow >= 0 && stepNow < maxStep)
                {
                    int nextStep = stepNow + 1;
                    int idGroup = lsStepThisItem.FirstOrDefault(r => r.IndexStep == nextStep).IdGroup;

                    var lsUserSigns = dm_GroupUserBUS.Instance.GetListByIdGroup(idGroup).Select(r => r.IdUser).ToList();

                    var userProcess = lsUsers.First(r => r.Id == bases.UserUpload);
                    var userUpload = lsUsers.First(r => r.Id == bases.UserProcess);
                    var displayName = bases.DisplayName.Split(new[] { "\n" }, StringSplitOptions.None);

                    var templateData = new
                    {
                        Id = idBase,
                        Typeof = lsTypes.First(r => r.Id == bases.IdTypes).DisplayName,
                        Nameen = displayName.Length > 2 ? displayName[2] : "",
                        Namevn = displayName.Length > 1 ? displayName[1] : "",
                        Nametw = displayName[0],
                        Userupload = $"{userUpload.IdDepartment} {userUpload.Id} {userUpload.DisplayName}",
                        Userprocess = $"{userProcess.IdDepartment} {userProcess.Id} {userProcess.DisplayName}",
                    };

                    var pageContent = templateSigner.Render(templateData);

                    string nameFile = $"文件審查 {DateTime.Now:HHmmss} {templateData.Id}.html";
                    NotesMail.SaveFileHtml(nameFile, pageContent);

                    // Send notes
                    var lsTO = lsUserSigns;
                    string subject = $"處理文件 : 知識庫系統通知您文件「 {templateData.Id}」需審查。";

                    string res = await NotesMail.SendNoteAsync(subject, pageContent, lsTO);
                    await Console.Out.WriteLineAsync(res);
                    logger.Info(MethodBase.GetCurrentMethod().ReflectedType.Name, res);
                }

#if DEBUG
#else
                // Cập nhật ngày thông báo notes lên DB
                var dataNotified = dt207_DocProcessingInfoBUS.Instance.GetItemById(processNotify.data.Id);
                dataNotified.TimeNotifyNotes = DateTime.Now;

                dt207_DocProcessingInfoBUS.Instance.AddOrUpdate(dataNotified);
#endif
            }
        }
    }
}
