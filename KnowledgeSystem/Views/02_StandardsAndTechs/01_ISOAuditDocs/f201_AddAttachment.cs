﻿using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;
using GridView = DevExpress.XtraGrid.Views.Grid.GridView;
using DataAccessLayer;
using KnowledgeSystem.Helpers;
using BusinessLayer;
using DevExpress.XtraGrid.Columns;
using DocumentFormat.OpenXml.Presentation;
using System.Web.Security;
using System.Runtime.InteropServices;
using DevExpress.XtraSplashScreen;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.IO;
using DocumentFormat.OpenXml.Spreadsheet;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraGrid;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._01_ISOAuditDocs
{
    public partial class f201_AddAttachment : DevExpress.XtraEditors.XtraForm
    {
        public f201_AddAttachment()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public EventFormInfo eventInfo = EventFormInfo.Create;
        public string formName = "";
        public int idBase = -1;
        public dt201_Forms baseForm = null;
        public List<dt201_Progress> baseProgresses;
        string idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);

        List<dm_User> users;
        List<dm_JobTitle> jobTitles;
        List<ProgressDetail> progresses = new List<ProgressDetail>();
        List<dt201_Role> roles;
        Attachment attachment = new Attachment();

        BindingSource sourceProgresses = new BindingSource();

        class ProgressDetail : dt201_Progress
        {
            public string UserName { get; set; }
            public string JobName { get; set; }
        }

        private class Attachment : dm_Attachment
        {
            public string FullPath { get; set; }
        }

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void EnabledController(bool _enable = true)
        {
            txbDocCode.Enabled = _enable;
            txbDisplayName.Enabled = _enable;
            txbAtt.Enabled = _enable;
        }

        private void LockControl()
        {
            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    Text = $"新增{formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    EnabledController();
                    break;
                case EventFormInfo.Update:
                    Text = $"更新{formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    EnabledController();
                    break;
                case EventFormInfo.Delete:
                    Text = $"刪除{formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    EnabledController(false);
                    break;
                case EventFormInfo.View:
                    Text = $"{formName}信息";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

                    EnabledController(false);
                    break;
                default:
                    break;
            }
        }

        private void f201_AddAttachment_Load(object sender, EventArgs e)
        {
            lcDefaultProgress.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            lcProgress.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            Size = new System.Drawing.Size(Size.Width, Size.Height - 207);

            LockControl();

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

            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    baseForm = new dt201_Forms();

                    ckSignOrPaper.SelectedIndex = 0;

                    break;
                case EventFormInfo.View:
                    break;
                case EventFormInfo.Update:
                    break;
                case EventFormInfo.Delete:
                    break;
                case EventFormInfo.ViewOnly:
                    break;
                default:
                    break;
            }
        }

        private void gvProgress_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            if (e.Column.FieldName != "IdUser") return;

            var usrInfo = users.FirstOrDefault(r => r.Id == e.Value?.ToString());
            string nameUser = usrInfo?.DisplayName ?? "";
            string jobName = jobTitles.FirstOrDefault(r => r.Id == usrInfo.JobCode)?.DisplayName ?? "";
            view.SetRowCellValue(e.RowHandle, view.Columns["UserName"], nameUser);
            view.SetRowCellValue(e.RowHandle, view.Columns["JobName"], jobName);
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Đưa focused ra khỏi bảng để cập nhật lên source
            gvProgress.FocusedRowHandle = GridControl.AutoFilterRowHandle;

            string code = txbDocCode.Text.Trim();
            string displayName = txbDisplayName.Text.Trim();
            string displayNameVN = txbDisplayNameVN.Text.Trim();

            bool isProcess = ckSignOrPaper.SelectedIndex == 1;

            bool isProgressError = (progresses.Any(r => r.IdUser == TPConfigs.LoginUser.Id) || progresses.GroupBy(x => x.IdUser).Any(g => g.Count() > 1));
            if (isProgressError)
            {
                XtraMessageBox.Show("流程重複或包含您", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //if (string.IsNullOrEmpty(ursId) || string.IsNullOrEmpty(course))
            //{
            //    XtraMessageBox.Show("請填寫所有信息", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            var result = false;
            string msg = "";
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                baseForm.Code = code;
                baseForm.DisplayName = displayName;
                baseForm.DisplayNameVN = displayNameVN;
                baseForm.IdBase = idBase;
                baseForm.UploadTime = DateTime.Now;
                baseForm.UploadUser = TPConfigs.LoginUser.Id;
                baseForm.IsProcessing = isProcess;
                baseForm.DigitalSign = isProcess;

                msg = $"{baseForm.Code} {baseForm.DisplayName}";
                switch (eventInfo)
                {
                    case EventFormInfo.Create:

                        dm_Attachment att = new dm_Attachment()
                        {
                            Thread = attachment.Thread,
                            ActualName = attachment.ActualName,
                            EncryptionName = attachment.EncryptionName
                        };
                        int idAtt = dm_AttachmentBUS.Instance.Add(att);
                        baseForm.AttId = idAtt;

                        string folderDest = Path.Combine(TPConfigs.Folder201, idAtt.ToString());
                        if (!Directory.Exists(folderDest)) Directory.CreateDirectory(folderDest);

                        File.Copy(attachment.FullPath, Path.Combine(folderDest, attachment.EncryptionName), true);

                        int idForm = dt201_FormsBUS.Instance.Add(baseForm);
                        result = idForm > 0;

                        progresses.Insert(0, new ProgressDetail() { IdUser = TPConfigs.LoginUser.Id, IdRole = 0 });
                        baseProgresses = (from data in progresses
                                          select new dt201_Progress
                                          {
                                              IdAtt = idAtt,
                                              IdForm = idForm,
                                              IdUser = data.IdUser,
                                              IdRole = data.IdRole
                                          }).ToList();
                        dt201_ProgressBUS.Instance.AddRange(baseProgresses);

                        dt201_ProgInfo info = new dt201_ProgInfo()
                        {
                            IdAtt = idAtt,
                            IdForm = idForm,
                            IdUser = TPConfigs.LoginUser.Id,
                            RespTime = DateTime.Now,
                            Note = "呈核"
                        };

                        dt201_ProgInfoBUS.Instance.Add(info);

                        break;
                    case EventFormInfo.Update:
                        result = dt201_FormsBUS.Instance.AddOrUpdate(baseForm);
                        break;
                    //case EventFormInfo.Delete:
                    //    var dialogResult = XtraMessageBox.Show($"您確認要刪除{_formName}: {_base.IdJobTitle} {_base.IdCourse} {_base.DateReceipt}", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    //    if (dialogResult != DialogResult.Yes) return;
                    //    result = dt301_BaseBUS.Instance.Remove(_base.Id);
                    //    break;
                    default:
                        break;
                }
            }

            if (result)
            {
                //switch (_eventInfo)
                //{
                //    case EventFormInfo.Update:
                //        logger.Info(_eventInfo.ToString(), msg);
                //        break;
                //    case EventFormInfo.Delete:
                //        logger.Warning(_eventInfo.ToString(), msg);
                //        break;
                //}
                Close();
            }
            else
            {
                MsgTP.MsgErrorDB();
            }
        }

        private void txbAtt_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "PDF | *.pdf" };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            string fileName = openFileDialog.FileName;
            string encryptionName = EncryptionHelper.EncryptionFileName(fileName);
            string actualName = Path.GetFileName(fileName);

            txbAtt.Text = actualName;
            attachment = new Attachment()
            {
                Thread = "201",
                EncryptionName = encryptionName,
                ActualName = actualName,
                FullPath = fileName
            };
        }

        private void ckSignOrPaper_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ckSignOrPaper.SelectedIndex == 0)
            {
                lcProgress.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lcDefaultProgress.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                Size = new Size(Size.Width, Size.Height - 207);
            }
            else
            {
                Size = new Size(Size.Width, Size.Height + 207);
                lcProgress.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                lcDefaultProgress.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            }
        }
    }
}