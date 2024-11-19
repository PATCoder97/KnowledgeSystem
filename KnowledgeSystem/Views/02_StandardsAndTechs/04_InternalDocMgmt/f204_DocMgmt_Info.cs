using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout;
using DevExpress.XtraRichEdit.Import.Html;
using DevExpress.XtraSplashScreen;
using DocumentFormat.OpenXml.Spreadsheet;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using KnowledgeSystem.Views._03_DepartmentManage._07_Quiz;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._04_InternalDocMgmt
{
    public partial class f204_DocMgmt_Info : DevExpress.XtraEditors.XtraForm
    {
        public f204_DocMgmt_Info()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public EventFormInfo eventInfo = EventFormInfo.Create;
        public string formName = "";
        public int idBase = -1;
        string idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);

        dt204_InternalDocMgmt dt204Base = new dt204_InternalDocMgmt();

        BindingSource sourceFormAtts = new BindingSource();
        BindingSource sourceRelatedDoc = new BindingSource();
        List<Attachment> baseFormAtts = new List<Attachment>();
        List<dt204_InternalDocMgmt> relatedDocs = new List<dt204_InternalDocMgmt>();

        List<LayoutControlItem> lcControls;
        List<LayoutControlItem> lcImpControls;

        string baseFilePath = "";

        private class Attachment : dm_Attachment
        {
            public string FilePath { get; set; }
        }

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void EnabledController(bool _enable = true)
        {
            cbbDocCatorary.Enabled = _enable;
            cbbFuncCatorary.Enabled = _enable;
            cbbDocLevel.Enabled = _enable;
            txbCode.Enabled = _enable;
            txbDocVersion.Enabled = _enable;
            txbDisplayName.Enabled = _enable;
            txbDisplayNameVN.Enabled = _enable;
            txbDeployDate.Enabled = _enable;
            txbPeriodNotify.Enabled = _enable;
            txbIdFounder.Enabled = _enable;
            txbFilePath.Enabled = _enable;

            btnAddFile.Enabled = _enable;
            gColDelFile.Visible = _enable;
            btnAddRelated.Enabled = _enable;
            gColDelDocRelated.Visible = _enable;
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

            foreach (var item in lcControls)
            {
                string colorHex = item.Control.Enabled ? "000000" : "000000";
                item.Text = item.Text.Replace("000000", colorHex);
            }

            // Các thông tin phải điền có thêm dấu * màu đỏ
            foreach (var item in lcImpControls)
            {
                if (item.Control.Enabled)
                {
                    item.Text += "<color=red>*</color>";
                }
                else
                {
                    item.Text = item.Text.Replace("<color=red>*</color>", "");
                }
            }
        }

        private void f204_DocMgmt_Info_Load(object sender, EventArgs e)
        {
            tabbedControlGroup1.SelectedTabPageIndex = 0;

            gvForm.ReadOnlyGridView();
            gvForm.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvRelatedDoc.ReadOnlyGridView();
            gvRelatedDoc.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            lcControls = new List<LayoutControlItem>() { lcDocCatorary, lcFuncCatorary, lcDocLevel, lcCode, lcDocVersion, lcDisplayName, lcDisplayNameVN, lcDeployDate, lcPeriodNotify, lcIdFounder, lcFilePath };
            lcImpControls = new List<LayoutControlItem>() { lcDocCatorary, lcFuncCatorary, lcDocLevel, lcCode, lcDocVersion, lcDisplayName, lcDeployDate, lcPeriodNotify, lcIdFounder, lcFilePath };
            foreach (var item in lcControls)
            {
                item.AllowHtmlStringInCaption = true;
                item.Text = $"<color=#000000>{item.Text}</color>";
            }

            var docCatoraries = dt204_DocCatoraryBUS.Instance.GetList();
            cbbDocCatorary.Properties.DataSource = docCatoraries;
            cbbDocCatorary.Properties.DisplayMember = "DisplayName";
            cbbDocCatorary.Properties.ValueMember = "Id";

            var funcCatoraries = dt204_FuncCatoraryBUS.Instance.GetList();
            cbbFuncCatorary.Properties.DataSource = funcCatoraries;
            cbbFuncCatorary.Properties.DisplayMember = "DisplayName";
            cbbFuncCatorary.Properties.ValueMember = "Id";

            cbbDocLevel.Properties.Items.AddRange(TPConfigs.DocTypes201.Split(';').ToList());

            var lsUsers = dm_UserBUS.Instance.GetListByDept(idDept2word).Select(r => new dm_User
            {
                Id = r.Id,
                IdDepartment = r.IdDepartment,
                DisplayName = r.DisplayName,
                JobCode = r.JobCode
            }).ToList();
            txbIdFounder.Properties.DataSource = lsUsers;
            txbIdFounder.Properties.DisplayMember = "DisplayName";
            txbIdFounder.Properties.ValueMember = "Id";
            txbIdFounder.Properties.BestFitWidth = 110;

            gcForm.DataSource = sourceFormAtts;
            gcRelatedDoc.DataSource = sourceRelatedDoc;

            switch (eventInfo)
            {
                case EventFormInfo.Create:

                    dt204Base = new dt204_InternalDocMgmt();

                    break;
                case EventFormInfo.View:

                    dt204Base = dt204_InternalDocMgmtBUS.Instance.GetItemById(idBase);

                    cbbDocCatorary.EditValue = dt204Base.IdDocCatorary;
                    cbbFuncCatorary.EditValue = dt204Base.IdFuncCatorary;
                    cbbDocLevel.EditValue = dt204Base.DocLevel;
                    txbCode.EditValue = dt204Base.Code;
                    txbDocVersion.EditValue = dt204Base.DocVersion;
                    txbDisplayName.EditValue = dt204Base.DisplayName;
                    txbDisplayNameVN.EditValue = dt204Base.DisplayNameVN;
                    txbDeployDate.EditValue = dt204Base.DeployDate;
                    txbPeriodNotify.EditValue = dt204Base.PeriodNotify;
                    txbIdFounder.EditValue = dt204Base.IdFounder;
                    txbFilePath.EditValue = "...";

                    var attForms = dt204_FormBUS.Instance.GetListByIdBase(dt204Base.Id);
                    foreach (var item in attForms)
                    {
                        var att = dm_AttachmentBUS.Instance.GetItemById(item.IdAtt);
                        baseFormAtts.Add(new Attachment()
                        {
                            ActualName = att.ActualName,
                            EncryptionName = att.EncryptionName,
                            Thread = att.Thread,
                            Id = att.Id
                        });
                    }
                    sourceFormAtts.DataSource = baseFormAtts;
                    lbCountFile.Text = $"共{baseFormAtts.Count}個表單";
                    gvForm.RefreshData();

                    var idsRelatedDoc = dt204_RelatedDocBUS.Instance.GetListByIdBase(dt204Base.Id);
                    foreach (var item in idsRelatedDoc)
                    {
                        relatedDocs.Add(dt204_InternalDocMgmtBUS.Instance.GetItemById(item.IdRelatedDoc));
                    }
                    sourceRelatedDoc.DataSource = relatedDocs;
                    lbRelated.Text = $"共{relatedDocs.Count}個關聯文件";
                    gvRelatedDoc.RefreshData();

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

            LockControl();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Kiểm tra xem đã điền đầy đủ thông tin yêu cầu hay chưa
            bool IsValidate = true;
            foreach (var item in lcImpControls)
            {
                if (string.IsNullOrEmpty(item.Control.Text)) IsValidate = false;
            }

            if (!IsValidate)
            {
                MsgTP.MsgError("請填寫所有信息<color=red>(*)</color>");
                return;
            }

            var result = false;
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                var idDocCatorary = Convert.ToInt16(cbbDocCatorary.EditValue);
                var idFuncCatorary = Convert.ToInt16(cbbFuncCatorary.EditValue);
                string docLevel = cbbDocLevel.Text;
                string code = txbCode.Text.Trim();
                string docVersion = txbDocVersion.Text.Trim();
                string displayName = txbDisplayName.Text.Trim();
                string displayNameVN = txbDisplayNameVN.Text.Trim();
                DateTime deployDate = txbDeployDate.DateTime;
                int periodNotify = Convert.ToInt16(txbPeriodNotify.EditValue);
                string idFounder = txbIdFounder.EditValue.ToString();
                string fileName = txbFilePath.Text.Trim();

                dt204Base.IdDept = TPConfigs.LoginUser.IdDepartment;
                dt204Base.IdDocCatorary = idDocCatorary;
                dt204Base.IdFuncCatorary = idFuncCatorary;
                dt204Base.DocLevel = docLevel;
                dt204Base.Code = code;
                dt204Base.DocVersion = docVersion;
                dt204Base.DisplayName = displayName;
                dt204Base.DisplayNameVN = displayNameVN;
                dt204Base.DeployDate = deployDate;
                dt204Base.PeriodNotify = periodNotify;
                dt204Base.IdFounder = idFounder;
                dt204Base.IdUsrUpload = TPConfigs.LoginUser.Id;
                dt204Base.UploadDate = DateTime.Now;
                dt204Base.IsDel = false;

                switch (eventInfo)
                {
                    case EventFormInfo.Create:

                        HandleBaseAttachment();

                        int idDt204Base = dt204_InternalDocMgmtBUS.Instance.Add(dt204Base);
                        result = idDt204Base != -1;

                        Handle204Form(idDt204Base);
                        HandleDocsRelated(idDt204Base);

                        break;
                    case EventFormInfo.Update:

                        if (fileName != "...")
                        {
                            HandleBaseAttachment();
                        }

                        result = dt204_InternalDocMgmtBUS.Instance.AddOrUpdate(dt204Base);

                        Handle204Form(dt204Base.Id);
                        HandleDocsRelated(dt204Base.Id);

                        break;
                    case EventFormInfo.Delete:
                        var dialogResult = XtraMessageBox.Show($"您確認要刪除{formName}: {dt204Base.Code} {dt204Base.DisplayName}", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult != DialogResult.Yes) return;
                        result = dt204_InternalDocMgmtBUS.Instance.RemoveById(dt204Base.Id, TPConfigs.LoginUser.Id);
                        break;
                    default:
                        break;
                }
            }

            if (result)
            {
                Close();
            }
            else
            {
                MsgTP.MsgErrorDB();
            }
        }

        private void HandleDocsRelated(int idDt204Base)
        {
            dt204_RelatedDocBUS.Instance.RemoveByIdBase(idDt204Base);

            dt204_RelatedDocBUS.Instance.AddRange(relatedDocs.Select(r => new dt204_RelatedDoc() { IdBase = idDt204Base, IdRelatedDoc = r.Id }).ToList());
        }

        private void Handle204Form(int idDt204Base)
        {
            dt204_FormBUS.Instance.RemoveByIdBase(idDt204Base);

            foreach (var item in baseFormAtts)
            {
                if (Directory.Exists(TPConfigs.Folder204))
                    Directory.CreateDirectory(TPConfigs.Folder204);

                var idAtt = item.Id;

                if (!string.IsNullOrEmpty(item.FilePath))
                {
                    File.Copy(item.FilePath, Path.Combine(TPConfigs.Folder204, item.EncryptionName));

                    idAtt = dm_AttachmentBUS.Instance.Add(new dm_Attachment()
                    {
                        ActualName = item.ActualName,
                        EncryptionName = item.EncryptionName,
                        Thread = item.Thread
                    });
                }

                dt204_FormBUS.Instance.Add(new dt204_Form()
                {
                    IdBase = idDt204Base,
                    IdAtt = idAtt,
                    DisplayName = item.ActualName
                });
            }
        }

        private void HandleBaseAttachment()
        {
            var baseAtt = new dm_Attachment()
            {
                ActualName = Path.GetFileName(baseFilePath),
                EncryptionName = EncryptionHelper.EncryptionFileName(baseFilePath),
                Thread = "204"
            };

            var idAtt = dm_AttachmentBUS.Instance.Add(baseAtt);
            dt204Base.IdAtt = idAtt;

            if (Directory.Exists(TPConfigs.Folder204))
                Directory.CreateDirectory(TPConfigs.Folder204);

            File.Copy(baseFilePath, Path.Combine(TPConfigs.Folder204, baseAtt.EncryptionName));
        }

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            eventInfo = EventFormInfo.Update;
            LockControl();
        }

        private void txbFilePath_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                FilterIndex = 1
            };

            if (dialog.ShowDialog() != DialogResult.OK) return;
            baseFilePath = dialog.FileName;
            txbFilePath.Text = Path.GetFileName(baseFilePath);
        }

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            eventInfo = EventFormInfo.Delete;
            LockControl();
        }

        private void btnAddFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = TPConfigs.FilterFile,
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            foreach (string fileName in openFileDialog.FileNames)
            {
                string encryptionName = EncryptionHelper.EncryptionFileName(fileName);
                Attachment attachment = new Attachment
                {
                    FilePath = fileName,
                    ActualName = Path.GetFileName(fileName),
                    EncryptionName = encryptionName,
                    Thread = "204"
                };
                baseFormAtts.Add(attachment);
                Thread.Sleep(5);
            }

            sourceFormAtts.DataSource = baseFormAtts;
            lbCountFile.Text = $"共{baseFormAtts.Count}個表單";
            gvForm.RefreshData();
        }

        private void btnDelFile_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            GridView view = gvForm;
            Attachment attachment = view.GetRow(view.FocusedRowHandle) as Attachment;

            string msg = $"您想要刪除表單：\r\n{attachment.ActualName}?";
            if (MsgTP.MsgYesNoQuestion(msg) == DialogResult.No)
            {
                return;
            }

            baseFormAtts.Remove(attachment);
            lbCountFile.Text = $"共{baseFormAtts.Count}個表單";

            int rowIndex = view.FocusedRowHandle;
            view.RefreshData();
            view.FocusedRowHandle = rowIndex;
        }

        private void btnAddRelated_Click(object sender, EventArgs e)
        {
            f204_ChooseDocRelated fData = new f204_ChooseDocRelated();
            fData.DocsInput = relatedDocs;
            fData.idBaseDoc = dt204Base.Id;
            fData.ShowDialog();

            if (fData.DocsOutput == null) return;

            relatedDocs.AddRange(fData.DocsOutput);

            sourceRelatedDoc.DataSource = relatedDocs;
            lbRelated.Text = $"共{relatedDocs.Count}個關聯文件";
            gvRelatedDoc.RefreshData();
        }

        private void ibtnDelDocRelated_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            GridView view = gvRelatedDoc;
            dt204_InternalDocMgmt relatedDoc = view.GetRow(view.FocusedRowHandle) as dt204_InternalDocMgmt;

            string msg = $"您想要刪除關聯文件：\r\n{relatedDoc.DisplayName}?";
            if (MsgTP.MsgYesNoQuestion(msg) == DialogResult.No)
            {
                return;
            }

            relatedDocs.Remove(relatedDoc);
            lbRelated.Text = $"共{relatedDocs.Count}個關聯文件";

            int rowIndex = view.FocusedRowHandle;
            view.RefreshData();
            view.FocusedRowHandle = rowIndex;
        }
    }
}