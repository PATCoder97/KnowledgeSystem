using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraPrinting.Drawing;
using DevExpress.XtraSplashScreen;
using iTextSharp.text.pdf;
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
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._05_IATF16949
{
    public partial class f205_AddAtts : DevExpress.XtraEditors.XtraForm
    {
        public f205_AddAtts()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public EventFormInfo eventInfo = EventFormInfo.Create;
        public string formName = "";
        public dt205_Form baseForm = null;
        public dt205_Base currentData = new dt205_Base();
        public dt205_Base parentData = new dt205_Base();
        public int idForm = -1;
        Attachment attachment = new Attachment();

        List<LayoutControlItem> lcControls;
        List<LayoutControlItem> lcImpControls;

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

        private void f205_AddAtts_Load(object sender, EventArgs e)
        {
            lcControls = new List<LayoutControlItem>() { lcDocCode, lcDisplayName, lcAtt };
            lcImpControls = new List<LayoutControlItem>() { lcDisplayName, lcAtt };
            foreach (var item in lcControls)
            {
                item.AllowHtmlStringInCaption = true;
                item.Text = $"<color=#000000>{item.Text}</color>";
            }

            switch (eventInfo)
            {
                case EventFormInfo.Create:

                    baseForm = new dt205_Form();

                    break;
                case EventFormInfo.View:
                    break;
                case EventFormInfo.Update:

                    baseForm = dt205_FormBUS.Instance.GetItemById(idForm);

                    txbDocCode.EditValue = baseForm.Code;
                    txbDisplayName.EditValue = baseForm.DisplayName;
                    txbAtt.EditValue = "...";

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

        private void HandleCopyFile(string sourcePath, string destPath)
        {
            DirectoryInfo parentDir = Directory.GetParent(destPath);
            if (parentDir != null && !Directory.Exists(parentDir.FullName))
                Directory.CreateDirectory(parentDir.FullName);

            File.Copy(sourcePath, destPath, true);
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

            var code = txbDocCode.EditValue?.ToString();
            var displayname = txbDisplayName.EditValue?.ToString();

            var result = false;
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                baseForm.Code = code;
                baseForm.DisplayName = displayname;

                switch (eventInfo)
                {
                    case EventFormInfo.Create:

                        var att = new dm_Attachment
                        {
                            Thread = attachment.Thread,
                            ActualName = attachment.ActualName,
                            EncryptionName = attachment.EncryptionName
                        };

                        int idAtt = dm_AttachmentBUS.Instance.Add(att);
                        baseForm.AttId = idAtt;
                        baseForm.BaseId = currentData.Id;
                        baseForm.CreateAt = DateTime.Now;
                        baseForm.CreateBy = TPConfigs.LoginUser.Id;

                        HandleCopyFile(attachment.FullPath, Path.Combine(TPConfigs.Folder205, attachment.EncryptionName));

                        result = dt205_FormBUS.Instance.Add(baseForm);

                        break;
                    case EventFormInfo.Update:

                        if (txbAtt.EditValue?.ToString() != "...")
                        {
                            att = new dm_Attachment
                            {
                                Thread = attachment.Thread,
                                ActualName = attachment.ActualName,
                                EncryptionName = attachment.EncryptionName
                            };

                            idAtt = dm_AttachmentBUS.Instance.Add(att);
                            baseForm.AttId = idAtt;
                            HandleCopyFile(attachment.FullPath, Path.Combine(TPConfigs.Folder205, attachment.EncryptionName));
                        }

                        result = dt205_FormBUS.Instance.AddOrUpdate(baseForm);

                        break;
                    case EventFormInfo.Delete:

                        //var dialogResult = XtraMessageBox.Show($"您確認要刪除{formName}\r\n{cbbFunc.Text}：{cbbUsr.Text}", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        //if (dialogResult != DialogResult.Yes) return;
                        //result = dt310_EHSFunctionBUS.Instance.RemoveById(EHSFunc.Id, TPConfigs.LoginUser.Id);

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
            string fileName = "";
            string encryptionName = "";
            string actualName = "";

            switch (e.Button.Caption)
            {
                case "Paste":

                    if (Clipboard.ContainsFileDropList())
                    {
                        var files = Clipboard.GetFileDropList();
                        var correctFiles = new List<string>();

                        foreach (var file in files)
                        {
                            var extensions = TPConfigs.FilterFile
                                .Split('|')[1]
                                .Split(';')
                                .Select(ext => ext.TrimStart('*').ToLower())
                                .ToList();

                            if (File.Exists(file))
                            {
                                string fileExt = Path.GetExtension(file).ToLower();
                                if (extensions.Contains(fileExt))
                                {
                                    correctFiles.Add(file);
                                }
                            }
                        }

                        if (correctFiles.Count != 1)
                        {
                            XtraMessageBox.Show("請選擇一個檔案", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        fileName = correctFiles.First();
                        encryptionName = EncryptionHelper.EncryptionFileName(fileName);
                        actualName = Path.GetFileName(fileName);

                        txbAtt.Text = actualName;
                        attachment = new Attachment()
                        {
                            Thread = "205",
                            EncryptionName = encryptionName,
                            ActualName = actualName,
                            FullPath = fileName
                        };
                    }
                    else
                    {
                        XtraMessageBox.Show("請選擇一個PDF檔案", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    break;
                default:
                    OpenFileDialog openFileDialog = new OpenFileDialog { Filter = TPConfigs.FilterFile };

                    if (openFileDialog.ShowDialog() != DialogResult.OK)
                        return;

                    fileName = openFileDialog.FileName;
                    encryptionName = EncryptionHelper.EncryptionFileName(fileName);
                    actualName = Path.GetFileName(fileName);

                    txbAtt.Text = actualName;
                    attachment = new Attachment()
                    {
                        Thread = "205",
                        EncryptionName = encryptionName,
                        ActualName = actualName,
                        FullPath = fileName
                    };
                    break;
            }
        }
    }
}