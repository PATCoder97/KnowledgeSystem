using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout;
using DevExpress.XtraRichEdit.Import.Html;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
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

        BindingSource sourceAtts = new BindingSource();
        List<Attachment> baseAtts;

        List<LayoutControlItem> lcControls;
        List<LayoutControlItem> lcImpControls;

        string baseFilePath = "";

        private class Attachment : dt306_BaseAtts
        {
            public string EncryptName { get; set; }
            public dt306_BaseAtts BaseAtt { get; set; }
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

        private void gvDocs_DoubleClick(object sender, EventArgs e)
        {
            //GridView view = sender as GridView;
            //var pt = view.GridControl.PointToClient(System.Windows.Forms.Control.MousePosition);
            //GridHitInfo hitInfo = view.CalcHitInfo(pt);
            //if (!hitInfo.InRowCell) return;

            //int idAtt = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColIdAtt));
            //var attProgress = dm_AttachmentBUS.Instance.GetItemById(idAtt);
            //string fileName = attProgress?.EncryptionName ?? "";

            //string sourceFolder = Path.Combine(TPConfigs.Folder306, idBase.ToString());
            //string sourcePath = Path.Combine(sourceFolder, fileName);
            //string destPath = Path.Combine(TPConfigs.TempFolderData, $"sign_{DateTime.Now:yyyyMMddHHmmss}.pdf");

            //if (!Directory.Exists(TPConfigs.TempFolderData))
            //    Directory.CreateDirectory(TPConfigs.TempFolderData);

            //File.Copy(sourcePath, destPath, true);

            //switch (idRoleConfirm)
            //{
            //    case 1:
            //        f00_PdfTools pdfTools = new f00_PdfTools(destPath, sourceFolder);
            //        pdfTools.ShowDialog();

            //        // Truy cập giá trị chuỗi trả về sau khi Form đã đóng
            //        string encrytFileName = pdfTools.OutFileName;
            //        string describe = pdfTools.Describe;

            //        Attachment itemToUpdate = baseAtts.SingleOrDefault(item => item.BaseAtt.IdAtt == idAtt);
            //        if (string.IsNullOrEmpty(encrytFileName))
            //        {
            //            itemToUpdate.BaseAtt.Desc = describe;
            //            itemToUpdate.BaseAtt.UsrCancel = TPConfigs.LoginUser.Id;
            //            itemToUpdate.BaseAtt.IsCancel = true;
            //            itemToUpdate.EncryptName = null;
            //        }
            //        else
            //        {
            //            itemToUpdate.BaseAtt.Desc = "已簽名";
            //            itemToUpdate.EncryptName = encrytFileName;
            //        }

            //        gvDocs.RefreshData();

            //        // Kiểm tra xem có văn kiện nào được đi tiếp không, Nếu có mới hiện nút Approval
            //        bool CanConfirm = baseAtts.Any(r => r.EncryptName != null);
            //        btnApproval.Visibility = CanConfirm ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
            //        break;
            //    case 2:
            //        f00_VIewFile fView = new f00_VIewFile(destPath, false, true);
            //        fView.ShowDialog();

            //        // Truy cập giá trị chuỗi trả về sau khi Form đã đóng
            //        bool IsConfirm = fView.IsConfirm;
            //        describe = fView.Describe;

            //        if (IsConfirm != true) return;// Kiểm tra xem đã xác nhận hay chưa

            //        itemToUpdate = baseAtts.SingleOrDefault(item => item.BaseAtt.IdAtt == idAtt);
            //        if (string.IsNullOrEmpty(describe))
            //        {
            //            itemToUpdate.BaseAtt.Desc = "已確認";
            //        }
            //        else
            //        {
            //            itemToUpdate.BaseAtt.Desc = describe;
            //            itemToUpdate.BaseAtt.UsrCancel = TPConfigs.LoginUser.Id;
            //            itemToUpdate.BaseAtt.IsCancel = true;
            //            itemToUpdate.EncryptName = null;
            //        }

            //        gvDocs.RefreshData();

            //        // Kiểm tra xem có văn kiện nào được đi tiếp không, Nếu có mới hiện nút Approval
            //        CanConfirm = baseAtts.Any(r => r.BaseAtt.IsCancel != true);
            //        btnConfirm.Visibility = CanConfirm ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
            //        break;
            //}
        }

        private void btnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XtraInputBoxArgs args = new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                Prompt = "退回文件原因",
                DefaultButtonIndex = 0,
                Editor = new MemoEdit(),
                DefaultResponse = ""
            };

            var result = XtraInputBox.Show(args);
            if (result == null) return;
            string describe = result?.ToString() ?? "";

            var baseData = dt306_BaseBUS.Instance.GetItemById(idBase);
            baseData.NextStepProg = "";
            baseData.IsProcess = false;
            baseData.IsCancel = true;
            baseData.Desc = $"被{TPConfigs.LoginUser.DisplayName}退回，說明：{describe}";

            dt306_BaseBUS.Instance.AddOrUpdate(baseData);

            // Đưa tất cả các file ra ngoài folder để sau này xem lại vì sao bị trả về
            foreach (var item in baseAtts)
            {
                item.UsrCancel = TPConfigs.LoginUser.Id;
                dt306_BaseAttsBUS.Instance.AddOrUpdate(item.BaseAtt);
            }

            var allAtts = dt306_BaseAttsBUS.Instance.GetListByIdBase(idBase);
            foreach (var item in allAtts)
            {
                var att = dm_AttachmentBUS.Instance.GetItemById(item.IdAtt);
                string sourceFile = Path.Combine(TPConfigs.Folder306, idBase.ToString(), att.EncryptionName);
                string destFile = Path.Combine(TPConfigs.Folder306, att.EncryptionName);

                File.Copy(sourceFile, destFile, true);
            }

            // Các bước trước nếu chưa gửi note thì khỏi gửi luôn
            var progInfoSendNote = dt306_ProgInfoBUS.Instance.GetListByIdBase(idBase)
                .Where(r => string.IsNullOrEmpty(r.SendNoteTime.ToString())).ToList();
            foreach (var item in progInfoSendNote)
            {
                item.SendNoteTime = DateTime.Now;
                dt306_ProgInfoBUS.Instance.AddOrUpdate(item);
            }

            // Gửi bước cuối cùng
            dt306_ProgInfo info = new dt306_ProgInfo()
            {
                IdBase = idBase,
                IdUsr = TPConfigs.LoginUser.Id,
                RespTime = DateTime.Now,
                Desc = "退回"
            };

            dt306_ProgInfoBUS.Instance.Add(info);

            Close();
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

                        result = dt204_InternalDocMgmtBUS.Instance.Add(dt204Base);

                        break;
                    case EventFormInfo.Update:

                        if (fileName != "...")
                        {
                            HandleBaseAttachment();
                        }

                        result = dt204_InternalDocMgmtBUS.Instance.AddOrUpdate(dt204Base);

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
                Filter = TPConfigs.FilterFile,
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
    }
}