using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Spreadsheet;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Security;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._06_Signature
{
    public partial class f306_NewSignDoc : DevExpress.XtraEditors.XtraForm
    {
        public f306_NewSignDoc()
        {
            InitializeComponent();
            InitializeIcon();
        }

        string idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);

        BindingSource sourceAtts = new BindingSource();
        List<Attachment> attachments;
        BindingSource sourceProgresses = new BindingSource();
        List<ProgressDetail> progresses = new List<ProgressDetail>();

        List<dm_User> users;
        List<dm_JobTitle> jobTitles;
        List<dt201_Role> roles;

        dt306_Base baseData = new dt306_Base();

        private class Attachment : dm_Attachment
        {
            public string PathFile { get; set; }
            //public dm_Attachment BaseAttachment { get; set; } = new dm_Attachment();
        }

        class ProgressDetail : dt306_Progress
        {
            public string UserName { get; set; }
            public string JobName { get; set; }
        }

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
            btnDefaulProgress.ImageOptions.SvgImage = TPSvgimages.Progress;
        }

        private void btnAddFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Pdf Files|*.pdf",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            foreach (string fileName in openFileDialog.FileNames)
            {
                string encryptionName = EncryptionHelper.EncryptionFileName(fileName);
                Attachment attachment = new Attachment
                {
                    Thread = "306",
                    ActualName = Path.GetFileName(fileName),
                    EncryptionName = $"{encryptionName}",
                    PathFile = fileName
                };
                attachments.Add(attachment);
                Thread.Sleep(5);
            }

            sourceAtts.DataSource = attachments;
            lbCountFile.Text = $"共{attachments.Count}份文件";
            gvFiles.RefreshData();
        }

        private void f306_NewSignDoc_Load(object sender, EventArgs e)
        {
            tabbedControlGroup1.SelectedTabPageIndex = 0;

            // Set datasource cho gridcontrol
            gcFiles.DataSource = sourceAtts;
            attachments = new List<Attachment>();

            users = dm_UserBUS.Instance.GetListByDept(idDept2word).Where(r => r.Status == 0).ToList();
            jobTitles = dm_JobTitleBUS.Instance.GetList();
            roles = dt201_RoleBUS.Instance.GetList().Where(r => r.Id != 0).ToList();

            // Gắn các thông số cho các combobox
            lookupUser.ValueMember = "Id";
            lookupUser.DisplayMember = "Id";
            lookupUser.PopupView.Columns.AddRange(new[]
            {
                new GridColumn { FieldName = "IdDepartment", VisibleIndex = 0, Caption = "單位", MinWidth=50, Width=50 },
                new GridColumn { FieldName = "Id", VisibleIndex = 0, Caption = "工號", MinWidth=120, Width=120 },
                new GridColumn { FieldName = "DisplayName", VisibleIndex = 2, Caption = "名稱", MinWidth=100, Width=150 },
            });
            lookupUser.DataSource = users;

            lookupRole.DataSource = roles;
            lookupRole.DisplayMember = "DisplayName";
            lookupRole.ValueMember = "Id";

            sourceProgresses.DataSource = progresses;
            gcProgress.DataSource = sourceProgresses;
        }

        private void btnDelFile_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            Attachment attachment = gvFiles.GetRow(gvFiles.FocusedRowHandle) as Attachment;

            string msg = $"您想要刪除附件：\r\n{attachment.ActualName}?";
            if (MsgTP.MsgYesNoQuestion(msg) == DialogResult.No)
            {
                return;
            }

            attachments.Remove(attachment);
            lbCountFile.Text = $"共{attachments.Count}份文件";

            int rowIndex = gvFiles.FocusedRowHandle;
            gvFiles.RefreshData();
            gvFiles.FocusedRowHandle = rowIndex;
        }

        private void gvProgress_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            if (e.Column.FieldName != "IdUsr") return;

            var usrInfo = users.FirstOrDefault(r => r.Id == e.Value?.ToString());
            string nameUser = usrInfo?.DisplayName ?? "";
            string jobName = jobTitles.FirstOrDefault(r => r.Id == usrInfo.JobCode)?.DisplayName ?? "";
            view.SetRowCellValue(e.RowHandle, view.Columns["UserName"], nameUser);
            view.SetRowCellValue(e.RowHandle, view.Columns["JobName"], jobName);
        }

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {


        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            baseData.DisplayName = txbTitle.Text.Trim();
            baseData.UploadUsr = TPConfigs.LoginUser.Id;
            baseData.UploadDate = DateTime.Today;
            baseData.IsProcess = true;
            baseData.IsCancel = false;
            baseData.NextStepProg = progresses.FirstOrDefault().IdUsr;

            int idBase = dt306_BaseBUS.Instance.Add(baseData);

            foreach (var item in attachments)
            {
                var att = new dm_Attachment()
                {
                    Thread = item.Thread,
                    ActualName = item.ActualName,
                    EncryptionName = item.EncryptionName
                };

                var idAtt = dm_AttachmentBUS.Instance.Add(att);

                var baseAtt = new dt306_BaseAtts()
                {
                    IdBase = idBase,
                    IdAtt = idAtt,
                    DisplayName = item.ActualName,
                    IsCancel = false
                };

                string folderDest = Path.Combine(TPConfigs.Folder306, idBase.ToString());
                if (!Directory.Exists(folderDest)) Directory.CreateDirectory(folderDest);

                File.Copy(item.PathFile, Path.Combine(folderDest, item.EncryptionName), true);

                dt306_BaseAttsBUS.Instance.Add(baseAtt);
            }

            var progs = progresses.Select(r => new dt306_Progress() { IdBase = idBase, IdUsr = r.IdUsr, IdRole = r.IdRole }).ToList();
            dt306_ProgressBUS.Instance.AddRange(progs);

            Close();
        }
    }
}