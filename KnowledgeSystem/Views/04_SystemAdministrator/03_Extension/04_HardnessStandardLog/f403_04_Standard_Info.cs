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
using System.Windows.Media.Media3D;
using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Html.Internal;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace KnowledgeSystem.Views._04_SystemAdministrator._03_Extension._04_HardnessStandardLog
{
    public partial class f403_04_Standard_Info : DevExpress.XtraEditors.XtraForm
    {
        public f403_04_Standard_Info()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public EventFormInfo eventInfo = EventFormInfo.Create;
        public string formName = "";
        public string snBase = "";
        public string idDeptGetData = "";
        string baseFilePath = "";

        dt403_04_StandardInfo standardInfo;

        List<LayoutControlItem> lcControls;
        List<LayoutControlItem> lcImpControls;

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
            txbSN.Enabled = false;
            txbMethod.Enabled = _enable;
            txbError.Enabled = _enable;
            txbValue.Enabled = _enable;
            txbExpDate.Enabled = _enable;
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
                    txbSN.Enabled = true;
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

        private void f403_04_Standar_Info_Load(object sender, EventArgs e)
        {
            lcControls = new List<LayoutControlItem>() { lcSN, lcMethod, lcValue, lcError };
            lcImpControls = new List<LayoutControlItem>() { lcSN, lcMethod, lcValue, lcError };
            foreach (var item in lcControls)
            {
                item.AllowHtmlStringInCaption = true;
                item.Text = $"<color=#000000>{item.Text}</color>";
            }

            switch (eventInfo)
            {
                case EventFormInfo.Create:

                    standardInfo = new dt403_04_StandardInfo();

                    break;
                case EventFormInfo.View:

                    standardInfo = dt403_04_StandardInfoBUS.Instance.GetItemById(snBase);

                    txbSN.EditValue = standardInfo.SN;
                    txbMethod.EditValue = standardInfo.Method;
                    txbValue.EditValue = standardInfo.Value;
                    txbError.EditValue = standardInfo.Error;
                    txbExpDate.EditValue = standardInfo.ExpDate;
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

        private void HandleAttachment()
        {
            var baseAtt = new dm_Attachment()
            {
                ActualName = Path.GetFileName(baseFilePath),
                EncryptionName = EncryptionHelper.EncryptionFileName(baseFilePath),
                Thread = "40304"
            };

            var idAtt = dm_AttachmentBUS.Instance.Add(baseAtt);
            standardInfo.IdAtt = idAtt;

            if (!Directory.Exists(TPConfigs.Folder40304))
                Directory.CreateDirectory(TPConfigs.Folder40304);

            File.Copy(baseFilePath, Path.Combine(TPConfigs.Folder40304, baseAtt.EncryptionName));
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Kiểm tra xem đã điền đầy đủ thông tin yêu cầu hay chưa
            bool IsValidate = true;

            foreach (var item in lcImpControls)
            {
                if (item.Control is BaseEdit baseEdit)
                {
                    if (string.IsNullOrEmpty(baseEdit.EditValue?.ToString()))
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

            var sn = txbSN.EditValue?.ToString();
            var method = txbMethod.EditValue?.ToString();
            var value = Convert.ToDouble(txbValue.EditValue);
            var error = Convert.ToDouble(txbError.EditValue);
            var expDate = txbExpDate.DateTime;
            string fileName = txbFilePath.Text.Trim();

            var result = false;
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                standardInfo.Method = method;
                standardInfo.Value = value;
                standardInfo.Error = error;
                standardInfo.ExpDate = expDate;
                standardInfo.IdDept = idDeptGetData;

                switch (eventInfo)
                {
                    case EventFormInfo.Create:

                        if (fileName != "..." && !string.IsNullOrEmpty(fileName))
                        {
                            if (standardInfo.IdAtt.HasValue)
                                dm_AttachmentBUS.Instance.RemoveById(standardInfo.IdAtt.Value);

                            HandleAttachment();
                        }
                        standardInfo.SN = sn;

                        result = dt403_04_StandardInfoBUS.Instance.Add(standardInfo);
                        break;
                    case EventFormInfo.Update:

                        if (fileName != "...")
                        {
                            if (standardInfo.IdAtt.HasValue)
                                dm_AttachmentBUS.Instance.RemoveById(standardInfo.IdAtt.Value);

                            HandleAttachment();
                        }

                        result = dt403_04_StandardInfoBUS.Instance.AddOrUpdate(standardInfo);
                        break;
                    case EventFormInfo.Delete:

                        var dialogResult = XtraMessageBox.Show($"您確認要刪除{formName}:\r\n{standardInfo.SN}", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult != DialogResult.Yes) return;
                        result = dt403_04_StandardInfoBUS.Instance.RemoveById(standardInfo.SN);

                        if (standardInfo.IdAtt.HasValue)
                            dm_AttachmentBUS.Instance.RemoveById(standardInfo.IdAtt.Value);

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

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            eventInfo = EventFormInfo.Update;
            LockControl();
        }

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MsgTP.MsgConfirmDel();

            eventInfo = EventFormInfo.Delete;
            LockControl();
        }

        private void txbFilePath_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
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
                        txbFilePath.Text = Path.GetFileName(baseFilePath);
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
                    txbFilePath.Text = Path.GetFileName(baseFilePath);

                    break;
            }
        }
    }
}