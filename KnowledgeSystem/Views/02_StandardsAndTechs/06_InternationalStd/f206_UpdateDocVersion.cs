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

namespace KnowledgeSystem.Views._02_StandardsAndTechs._06_InternationalStd
{
    public partial class f206_UpdateDocVersion : DevExpress.XtraEditors.XtraForm
    {
        public f206_UpdateDocVersion()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public EventFormInfo eventInfo = EventFormInfo.Create;
        public string formName = "";
        public int idBase = -1;

        string baseFilePath = "";

        List<LayoutControlItem> lcControls;
        List<LayoutControlItem> lcImpControls;

        private class Attachment : dm_Attachment
        {
            public string FilePath { get; set; }
        }

        private void InitializeIcon()
        {
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void f206_UpdateDocVersion_Load(object sender, EventArgs e)
        {
            lcControls = new List<LayoutControlItem>() { lcDocVersion, lcFilePath };
            lcImpControls = new List<LayoutControlItem>() { lcDocVersion, lcFilePath };

            foreach (var item in lcControls)
            {
                item.AllowHtmlStringInCaption = true;
                item.Text = $"<color=#000000>{item.Text}</color>";
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

                    using (var handle = SplashScreenManager.ShowOverlayForm(this))
                    {
                        if (dialog.ShowDialog() != DialogResult.OK) return;
                        baseFilePath = dialog.FileName;
                        txbAtt.Text = Path.GetFileName(baseFilePath);
                    }

                    break;
            }
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

            var dt206Base = dt206_DocumentsBUS.Instance.GetItemById(idBase);

            string oldVer = dt206Base.VersionNo;
            string newVer = txbDocVersion.Text.Trim();

            if (MsgTP.MsgYesNoQuestion($"你確定要將文件「<color=red>{dt206Base.DocumentCode}</color>」從版本<color=red>{oldVer}</color>更新為版本<color=red>{newVer}</color>嗎？") != DialogResult.Yes)
                return;

            dt206Base.VersionNo = txbDocVersion.Text.Trim();
            dt206Base.CreateBy = TPConfigs.LoginUser.Id;
            dt206Base.CreateAt = DateTime.Now;

            var baseAtt = new dm_Attachment()
            {
                ActualName = Path.GetFileName(baseFilePath),
                EncryptionName = EncryptionHelper.EncryptionFileName(baseFilePath),
                Thread = "206"
            };

            var idAtt = dm_AttachmentBUS.Instance.Add(baseAtt);
            dt206Base.IdAttachment = idAtt;

            if (Directory.Exists(TPConfigs.Folder206))
                Directory.CreateDirectory(TPConfigs.Folder206);

            File.Copy(baseFilePath, Path.Combine(TPConfigs.Folder206, baseAtt.EncryptionName));

            var result = dt206_DocumentsBUS.Instance.AddOrUpdate(dt206Base);

            if (result)
            {
                Close();
            }
            else
            {
                MsgTP.MsgErrorDB();
            }
        }
    }
}