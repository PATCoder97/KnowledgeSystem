using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DocumentFormat.OpenXml.Spreadsheet;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;

namespace KnowledgeSystem.Views._03_DepartmentManage._06_Signature
{
    public partial class f306_NewSignDoc : DevExpress.XtraEditors.XtraForm
    {
        public f306_NewSignDoc()
        {
            InitializeComponent();
            InitializeIcon();
        }

        Font fontUI14 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);

        string idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);

        BindingSource sourceAtts = new BindingSource();
        List<Attachment> attachments;
        BindingSource sourceProgresses = new BindingSource();
        List<ProgressDetail> progresses = new List<ProgressDetail>();

        List<dm_User> users;
        List<dm_JobTitle> jobTitles;
        List<dt306_SignRole> roles;

        dt306_Base baseData = new dt306_Base();

        private class Attachment : dm_Attachment
        {
            public string PathFile { get; set; }
            //public dm_Attachment BaseAttachment { get; set; } = new dm_Attachment();
        }

        private class ProgressDetail : dt306_Progress
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

        private bool ValidateData()
        {
            bool IsOK = true;
            string msg = "請提供以下補充資訊：";
            if (string.IsNullOrEmpty(txbTitle.EditValue?.ToString()))
            {
                msg += "</br> •文件名稱";
                IsOK = false;
            }

            if (string.IsNullOrEmpty(txbType.EditValue?.ToString()))
            {
                msg += "</br> •類別";
                IsOK = false;
            }

            if (progresses.Any(r => r.IdRole == 0 || string.IsNullOrEmpty(r.IdUsr)) || progresses.Count() == 0)
            {
                msg += "</br> •核簽流程";
                IsOK = false;
            }

            if (attachments.Count == 0)
            {
                msg += "</br> •文件";
                IsOK = false;
            }

            if (!IsOK)
            {
                msg = $"<font='Microsoft JhengHei UI' size=14>{msg}</font>";
                MsgTP.MsgShowInfomation(msg);
            }

            return IsOK;
        }

        bool cal(Int32 _Width, GridView _View)
        {
            _View.IndicatorWidth = _View.IndicatorWidth < _Width ? _Width : _View.IndicatorWidth;
            return true;
        }

        void IndicatorDraw(RowIndicatorCustomDrawEventArgs e, Color color)
        {
            e.Info.Appearance.Font = fontUI14;
            e.Info.Appearance.ForeColor = color;
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

        private void gridView_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;

            Color color = view.Appearance.HeaderPanel.ForeColor;

            if (!view.IsGroupRow(e.RowHandle))
            {
                if (e.Info.IsRowIndicator)
                {
                    if (e.RowHandle < 0)
                    {
                        e.Info.ImageIndex = 0;
                        e.Info.DisplayText = string.Empty;
                    }
                    else
                    {
                        e.Info.ImageIndex = -1;
                        e.Info.DisplayText = (e.RowHandle + 1).ToString();
                    }
                    IndicatorDraw(e, color);
                    SizeF _Size = e.Graphics.MeasureString(e.Info.DisplayText, fontUI14);
                    Int32 _Width = Convert.ToInt32(_Size.Width) + 20;
                    BeginInvoke(new MethodInvoker(delegate { cal(_Width, view); }));
                }
            }
            else
            {
                e.Info.ImageIndex = -1;
                e.Info.DisplayText = string.Format("[{0}]", (e.RowHandle * -1));
                IndicatorDraw(e, color);
                SizeF _Size = e.Graphics.MeasureString(e.Info.DisplayText, fontUI14);
                Int32 _Width = Convert.ToInt32(_Size.Width) + 20;
                BeginInvoke(new MethodInvoker(delegate { cal(_Width, view); }));
            }
        }

        private void f306_NewSignDoc_Load(object sender, EventArgs e)
        {
            btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

            Text = "上傳文件";
            tabbedControlGroup1.SelectedTabPageIndex = 0;

            // Set datasource cho gridcontrol
            gcFiles.DataSource = sourceAtts;
            attachments = new List<Attachment>();

            users = dm_UserBUS.Instance.GetListByDept(idDept2word).Where(r => r.Status == 0).ToList();
            jobTitles = dm_JobTitleBUS.Instance.GetList();
            roles = dt306_SignRoleBUS.Instance.GetList().Where(r => r.Id != 0).ToList();

            var dmType = dt306_TypeBUS.Instance.GetList();

            // Gắn các thông số cho các combobox
            lookupUser.ValueMember = "Id";
            lookupUser.DisplayMember = "Id";
            lookupUser.PopupView.Columns.AddRange(new[]
            {
                new GridColumn { FieldName = "IdDepartment", VisibleIndex = 0, Caption = "單位", MinWidth = 50, Width = 50 },
                new GridColumn { FieldName = "Id", VisibleIndex = 0, Caption = "工號", MinWidth = 120, Width = 120 },
                new GridColumn { FieldName = "DisplayName", VisibleIndex = 2, Caption = "名稱", MinWidth = 100, Width = 150 },
            });
            lookupUser.DataSource = users;

            lookupRole.DataSource = roles;
            lookupRole.DisplayMember = "DisplayName";
            lookupRole.ValueMember = "Id";

            txbType.Properties.DataSource = dmType;
            txbType.Properties.DisplayMember = "DisplayName";
            txbType.Properties.ValueMember = "Id";

            sourceProgresses.DataSource = progresses;
            gcProgress.DataSource = sourceProgresses;
        }

        private void btnDelFile_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            GridView view = gvFiles;
            Attachment attachment = view.GetRow(view.FocusedRowHandle) as Attachment;

            string msg = $"您想要刪除附件：\r\n{attachment.ActualName}?";
            if (MsgTP.MsgYesNoQuestion(msg) == DialogResult.No)
            {
                return;
            }

            attachments.Remove(attachment);
            lbCountFile.Text = $"共{attachments.Count}份文件";

            int rowIndex = gvFiles.FocusedRowHandle;
            view.RefreshData();
            view.FocusedRowHandle = rowIndex;
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
            // Đưa focused ra khỏi bảng để cập nhật lên source
            gvProgress.FocusedRowHandle = GridControl.AutoFilterRowHandle;

            bool IsValidate = ValidateData();
            if (!IsValidate) return;

            bool isProgressError = (progresses.GroupBy(x => x.IdUsr).Any(g => g.Count() > 1));
            if (isProgressError)
            {
                MsgTP.MsgError("流程重複！");
                return;
            }

            baseData.DisplayName = txbTitle.Text.Trim();
            baseData.IdType = (int)txbType.EditValue;
            baseData.UploadUsr = TPConfigs.LoginUser.Id;
            baseData.UploadDate = DateTime.Today;
            baseData.IsProcess = true;
            baseData.IsCancel = false;
            baseData.NextStepProg = progresses.FirstOrDefault().IdUsr;
            baseData.Confidential = ckConfidential.Checked;

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

            // Tạo 1 progStep là văn kiện đã đưa lên
            dt306_ProgInfo info = new dt306_ProgInfo()
            {
                IdBase = idBase,
                IdUsr = "VNW0000000",
                RespTime = DateTime.Now,
                Desc = "呈核"
            };

            dt306_ProgInfoBUS.Instance.Add(info);

            Close();
        }

        private void btnDefaulProgress_Click(object sender, EventArgs e)
        {
            f00_SelectFixedProg fProg = new f00_SelectFixedProg();
            fProg.ShowDialog();

            int idFixedProg = fProg.Id;

            progresses.Clear();
            string defaulProg = dm_FixedProgressBUS.Instance.GetItemById(idFixedProg)?.Progress;
            if (string.IsNullOrEmpty(defaulProg)) return;

            string[] defaulProgs = defaulProg.Split(';');

            foreach (var item in defaulProgs)
            {
                var usrInfo = users.FirstOrDefault(r => r.Id == item?.ToString());
                string nameUser = usrInfo?.DisplayName ?? "";
                string jobName = jobTitles.FirstOrDefault(r => r.Id == usrInfo.JobCode)?.DisplayName ?? "";
                progresses.Add(new ProgressDetail() { IdUsr = item, JobName = jobName, UserName = nameUser });
            }

            gcProgress.RefreshDataSource();
        }

        private void btnDelProg_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            GridView view = gvProgress;
            ProgressDetail prog = view.GetRow(view.FocusedRowHandle) as ProgressDetail;
            progresses.Remove(prog);

            int rowIndex = view.FocusedRowHandle;
            view.RefreshData();
            view.FocusedRowHandle = rowIndex;
        }
    }
}