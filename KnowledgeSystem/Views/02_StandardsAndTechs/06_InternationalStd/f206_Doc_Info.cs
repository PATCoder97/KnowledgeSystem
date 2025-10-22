using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
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
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._06_InternationalStd
{
    public partial class f206_Doc_Info : DevExpress.XtraEditors.XtraForm
    {
        public f206_Doc_Info()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public EventFormInfo eventInfo = EventFormInfo.Create;
        public string formName = "";
        public int idBase = -1;
        string idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);

        dt206_Documents dt206Base;

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
            cbbCategory.Enabled = _enable;
            txbDocCode.Enabled = _enable;
            txbDisplayName.Enabled = _enable;
            txbDisplayNameVN.Enabled = _enable;
            txbDisplayNameEN.Enabled = _enable;
            txbAtt.Enabled = _enable;
            ckHasCopyRight.Enabled = _enable;
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
                if (item.Control.Enabled || (item.Control as BaseEdit).Properties.ReadOnly)
                {
                    item.Text += "<color=red>*</color>";
                }
                else
                {
                    item.Text = item.Text.Replace("<color=red>*</color>", "");
                }
            }
        }

        private void f206_Doc_Info_Load(object sender, EventArgs e)
        {
            lcControls = new List<LayoutControlItem>() { lcCategory, lcDocCode, lcVersion, lcDisplayName, lcDisplayNameVN, lcDisplayNameEN, lcAtt };
            lcImpControls = new List<LayoutControlItem>() { lcCategory, lcDocCode, lcVersion, lcAtt };
            foreach (var item in lcControls)
            {
                item.AllowHtmlStringInCaption = true;
                item.Text = $"<color=#000000>{item.Text}</color>";
            }

            var docCatoraries = dt206_DocCategoriesBUS.Instance.GetList();
            cbbCategory.Properties.DataSource = docCatoraries;
            cbbCategory.Properties.DisplayMember = "DisplayName";
            cbbCategory.Properties.ValueMember = "Id";

            txbVersion.Enabled = false;

            switch (eventInfo)
            {
                case EventFormInfo.Create:

                    txbVersion.Enabled = true;

                    dt206Base = new dt206_Documents();

                    break;
                case EventFormInfo.View:

                    dt206Base = dt206_DocumentsBUS.Instance.GetItemById(idBase);

                    cbbCategory.EditValue = dt206Base.IdCategory;
                    txbDocCode.EditValue = dt206Base.DocumentCode;
                    txbVersion.EditValue = dt206Base.VersionNo;
                    txbDisplayName.EditValue = dt206Base.DisplayNameZH;
                    txbDisplayNameVN.EditValue = dt206Base.DisplayNameVN;
                    txbDisplayNameEN.EditValue = dt206Base.DisplayNameEN;
                    ckHasCopyRight.CheckState = dt206Base.HasCopyright == true ? CheckState.Checked : CheckState.Unchecked;
                    txbAtt.EditValue = "...";

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

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            eventInfo = EventFormInfo.Update;
            LockControl();
        }

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            eventInfo = EventFormInfo.Delete;
            LockControl();
        }

        private void HandleBaseAttachment()
        {
            var baseAtt = new dm_Attachment()
            {
                ActualName = Path.GetFileName(baseFilePath),
                EncryptionName = EncryptionHelper.EncryptionFileName(baseFilePath),
                Thread = "206"
            };

            var idAtt = dm_AttachmentBUS.Instance.Add(baseAtt);
            dt206Base.IdAttachment = idAtt;

            if (!Directory.Exists(TPConfigs.Folder206))
                Directory.CreateDirectory(TPConfigs.Folder206);

            File.Copy(baseFilePath, Path.Combine(TPConfigs.Folder206, baseAtt.EncryptionName));
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Kiểm tra xem đã điền đầy đủ thông tin yêu cầu hay chưa
            bool IsValidate = true;
            foreach (var item in lcImpControls)
            {
                if (item.Control is BaseEdit baseEdit)
                {
                    if (string.IsNullOrEmpty(baseEdit.EditValue?.ToString()) && item.Control.Enabled)
                    {
                        IsValidate = false;
                        break; // Dừng vòng lặp ngay khi phát hiện lỗi
                    }
                }
            }

            if (!IsValidate)
            {
                MsgTP.MsgError("請填寫所有信息<color=red>(*)</color>");
                return;
            }

            var idDocCategory = Convert.ToInt16(cbbCategory.EditValue);
            string docCode = txbDocCode.Text;
            string docVersion = txbVersion.Text.Trim();
            string displayNameZH = txbDisplayName.Text.Trim();
            string displayNameVN = txbDisplayNameVN.Text.Trim();
            string displayNameEN = txbDisplayNameEN.Text.Trim();
            bool hasCopyRight = ckHasCopyRight.CheckState == CheckState.Checked;
            string fileName = txbAtt.Text.Trim();

            if (StringHelper.CheckUpcase(displayNameVN, 33) && displayNameVN.Length > 20 && StringHelper.CheckUpcase(displayNameEN, 33) && displayNameEN.Length > 20)
            {
                XtraMessageBox.Show("Tắt CAPSLOCK đi！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var result = false;
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                dt206Base.IdCategory = idDocCategory;
                dt206Base.DocumentCode = docCode;
                dt206Base.VersionNo = docVersion;
                dt206Base.DisplayNameZH = displayNameZH;
                dt206Base.DisplayNameVN = displayNameVN;
                dt206Base.DisplayNameEN = displayNameEN;
                dt206Base.HasCopyright = hasCopyRight;

                switch (eventInfo)
                {
                    case EventFormInfo.Create:

                        HandleBaseAttachment();

                        dt206Base.CreateAt = DateTime.Now;
                        dt206Base.CreateBy = TPConfigs.LoginUser.Id;
                        dt206Base.IdDept = TPConfigs.LoginUser.IdDepartment;
                        result = dt206_DocumentsBUS.Instance.Add(dt206Base);

                        break;
                    case EventFormInfo.Update:

                        if (fileName != "...")
                        {
                            HandleBaseAttachment();
                        }

                        result = dt206_DocumentsBUS.Instance.AddOrUpdate(dt206Base);

                        break;
                    case EventFormInfo.Delete:

                        var dialogResult = XtraMessageBox.Show($"您確認要刪除{formName}: {dt206Base.DocumentCode}", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult != DialogResult.Yes) return;
                        result = dt206_DocumentsBUS.Instance.RemoveById(dt206Base.Id, TPConfigs.LoginUser.Id);

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

        private void txbAtt_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            switch (e.Button.Caption)
            {
                case "Paste":

                    if (Clipboard.ContainsFileDropList())
                    {
                        var files = Clipboard.GetFileDropList();
                        var pdfFiles = new List<string>();

                        foreach (var file in files)
                        {
                            if (file.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) && File.Exists(file))
                            {
                                pdfFiles.Add(file);
                            }
                        }

                        if (pdfFiles.Count != 1)
                        {
                            XtraMessageBox.Show("請選擇一個PDF檔案", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        baseFilePath = pdfFiles.First();
                        txbAtt.Text = Path.GetFileName(baseFilePath);
                    }
                    else
                    {
                        XtraMessageBox.Show("請選擇一個PDF檔案", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    break;
                default:
                    OpenFileDialog dialog = new OpenFileDialog
                    {
                        Filter = "PDF Files (*.pdf)|*.pdf",
                        FilterIndex = 1
                    };

                    if (dialog.ShowDialog() != DialogResult.OK) return;
                    baseFilePath = dialog.FileName;
                    txbAtt.Text = Path.GetFileName(baseFilePath);

                    break;
            }
        }
    }
}