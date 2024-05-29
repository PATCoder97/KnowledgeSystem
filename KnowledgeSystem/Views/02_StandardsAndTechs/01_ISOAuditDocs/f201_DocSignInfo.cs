using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Extensions;
using DevExpress.XtraEditors;
using DocumentFormat.OpenXml.Spreadsheet;
using iTextSharp.text.pdf;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._01_ISOAuditDocs
{
    public partial class f201_DocSignInfo : DevExpress.XtraEditors.XtraForm
    {
        public f201_DocSignInfo()
        {
            InitializeComponent();
        }

        public EventFormInfo eventInfo = EventFormInfo.Create;
        public string formName = "";
        public int idBaseForm = -1;
        string idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);

        int idRoleConfirm = -1;

        List<dm_JobTitle> jobTitles;
        List<dt201_Role> roleConfirms;

        List<dt201_ProgInfo> progInfos;
        List<dt201_Progress> progress;
        dt201_Forms baseForm;

        bool IsLastStep = false;

        private void f201_DocSignInfo_Load(object sender, EventArgs e)
        {
            baseForm = dt201_FormsBUS.Instance.GetItemById(idBaseForm);
            jobTitles = dm_JobTitleBUS.Instance.GetList();
            roleConfirms = dt201_RoleBUS.Instance.GetList();

            var users = dm_UserBUS.Instance.GetList();
            progress = dt201_ProgressBUS.Instance.GetListByIdForm(idBaseForm);

            var progressInfo = (from data in progress
                                join usr in users on data.IdUser equals usr.Id
                                select new { data, usr }).ToList();

            // Thêm danh sách các bước vào StepProgressBar
            foreach (var item in progressInfo)
            {
                var barItem = new StepProgressBarItem();
                barItem.ContentBlock1.Caption = $"{item.usr.IdDepartment} {item.usr.DisplayName}";
                barItem.ContentBlock1.Description = $"{item.usr.Id}\r\n{jobTitles.FirstOrDefault(r => r.Id == item.usr.JobCode).DisplayName}";
                barItem.ContentBlock2.Caption = roleConfirms.FirstOrDefault(r => r.Id == item.data.IdRole)?.DisplayName;
                stepProgressDoc.Items.Add(barItem);
            }
            stepProgressDoc.ItemOptions.Indicator.Width = 40;

            progInfos = dt201_ProgInfoBUS.Instance.GetListByIdForm(idBaseForm);
            var progNow = progInfos.OrderByDescending(r => r.RespTime).FirstOrDefault();

            int stepNow = progNow != null ? progress.IndexOf(progress.First(r => r.IdUser == progNow.IdUser)) : -1;
            stepProgressDoc.SelectedItemIndex = stepNow; // Focus đến bước hiện tại

            var nextStepUsr = progress[stepNow + 1].IdUser;

            IsLastStep = stepNow == progress.Count() - 2;

            // Thêm lịch sử trình ký vào gridProcess
            var lsHistoryProcess = (from data in progInfos
                                    join usr in users on data.IdUser equals usr.Id
                                    join job in jobTitles on usr.JobCode equals job.Id
                                    select new
                                    {
                                        data,
                                        usr,
                                        job,
                                        DisplayName = $"{usr.Id} LG{usr.IdDepartment}/{usr.DisplayName}"
                                    }).ToList();

            gcHistoryProcess.DataSource = lsHistoryProcess;

            gvHistoryProcess.ReadOnlyGridView();

            idRoleConfirm = progress.FirstOrDefault(r => r.IdUser == TPConfigs.LoginUser.Id)?.IdRole ?? -1;

            btnSign.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnCancel.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

            if (nextStepUsr == TPConfigs.LoginUser.Id)
            {
                switch (idRoleConfirm)
                {
                    case 1:
                        btnSign.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                        btnCancel.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                        break;
                    case 2:
                        btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                        btnCancel.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                        break;

                }
            }
        }

        private void btnSign_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            int idAtt = baseForm.AttId ?? -1;

            var attProgress = dm_AttachmentBUS.Instance.GetItemById(idAtt);
            string fileName = attProgress?.EncryptionName ?? "";

            string sourcePath = Path.Combine(TPConfigs.Folder201, idAtt.ToString(), fileName);
            string destPath = Path.Combine(TPConfigs.TempFolderData, $"sign_{DateTime.Now:yyyyMMddHHmmss}");

            if (!Directory.Exists(TPConfigs.TempFolderData))
                Directory.CreateDirectory(TPConfigs.TempFolderData);

            File.Copy(sourcePath, destPath, true);

            f00_PdfTools pdfTools = new f00_PdfTools(destPath, Path.Combine(TPConfigs.Folder201, idAtt.ToString()));
            pdfTools.ShowDialog();

            // Truy cập giá trị chuỗi trả về sau khi Form đã đóng
            string encrytFileName = pdfTools.OutFileName;
            if (encrytFileName == null) return;

            dt201_ProgInfo info = new dt201_ProgInfo()
            {
                IdAtt = idAtt,
                IdForm = idBaseForm,
                IdUser = TPConfigs.LoginUser.Id,
                RespTime = DateTime.Now,
                Note = "簽名"
            };

            dt201_ProgInfoBUS.Instance.Add(info);

            attProgress.EncryptionName = encrytFileName;
            dm_AttachmentBUS.Instance.AddOrUpdate(attProgress);

            if (IsLastStep)
            {
                baseForm.IsProcessing = false;
                dt201_FormsBUS.Instance.AddOrUpdate(baseForm);

                string sourceFile = Path.Combine(TPConfigs.Folder201, idAtt.ToString(), encrytFileName);
                string destFile = Path.Combine(TPConfigs.Folder201, encrytFileName);

                File.Copy(sourceFile, destFile, true);
            }

            Close();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string msg = "您確定核准該文件嗎？";
            if (MsgTP.MsgYesNoQuestion(msg) != DialogResult.Yes) return;

            int idAtt = baseForm.AttId ?? -1;
            dt201_ProgInfo info = new dt201_ProgInfo()
            {
                IdAtt = idAtt,
                IdForm = idBaseForm,
                IdUser = TPConfigs.LoginUser.Id,
                RespTime = DateTime.Now,
                Note = "核准"
            };

            dt201_ProgInfoBUS.Instance.Add(info);

            if (IsLastStep)
            {
                var attProgress = dm_AttachmentBUS.Instance.GetItemById(idAtt);
                string encrytFileName = attProgress.EncryptionName;

                baseForm.IsProcessing = false;
                dt201_FormsBUS.Instance.AddOrUpdate(baseForm);

                string sourceFile = Path.Combine(TPConfigs.Folder201, idAtt.ToString(), encrytFileName);
                string destFile = Path.Combine(TPConfigs.Folder201, encrytFileName);

                File.Copy(sourceFile, destFile, true);
            }

            Close();
        }
    }
}