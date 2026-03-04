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

namespace KnowledgeSystem.Views._04_SystemAdministrator._03_Extension._05_CalipStandardMgmt
{
    public partial class f403_05_UpdateStandar : DevExpress.XtraEditors.XtraForm
    {
        public f403_05_UpdateStandar()
        {
            InitializeComponent();
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

        private void f403_05_UpdateStandar_Load(object sender, EventArgs e)
        {
            lcControls = new List<LayoutControlItem>() {lcFilePath, lcMaGCN, lcNextCalibrationDate, lcStandardlink, lcĐKĐBĐ };
            lcImpControls = new List<LayoutControlItem>() {lcFilePath, lcMaGCN, lcNextCalibrationDate, lcStandardlink, lcĐKĐBĐ };

            foreach (var item in lcControls)
            {
                item.AllowHtmlStringInCaption = true;
                item.Text = $"<color=#000000>{item.Text}</color>";
            }

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
            //switch (eventInfo)
            //{
            //    case EventFormInfo.Update:
            //        break;
            //}

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

            var dt403_05_standardAtt = dt403_05_StandardAttBUS.Instance.GetItemById(idBase);
            //if (dt403_05_standardAtt == null)
            //{
                dt403_05_standardAtt = new dt403_05_StandardAtt();
                dt403_05_standardAtt.StandardId = idBase;
                dt403_05_standardAtt.UploadDate = DateTime.Now;
                dt403_05_standardAtt.MaGCN = txbMaGCN.Text;
                dt403_05_standardAtt.NextCalibrationDate = txbNextCalibrationDate.DateTime;
                dt403_05_standardAtt.ĐKĐBĐ = txbĐKĐBĐ.Text;
                dt403_05_standardAtt.Standardlink = txbStandardlink.Text;

                // Không hỏi Yes/No vì đây là tạo mới
            //}
            //else
            //{
            //    // Nếu có rồi → hỏi xác nhận
            //    DateTime oldVer = dt403_05_standardAtt.UploadDate;
            //    DateTime newVer = DateTime.Now;
            //    dt403_05_standardAtt.MaGCN = txbMaGCN.Text;
            //    dt403_05_standardAtt.NextCalibrationDate = txbNextCalibrationDate.DateTime;
            //    dt403_05_standardAtt.ĐKĐBĐ = txbĐKĐBĐ.Text;
            //    dt403_05_standardAtt.Standardlink = txbStandardlink.Text;
            //    if (MsgTP.MsgYesNoQuestion(
            //        $"你確定要將文件從版本 <color=red>{oldVer}</color> 更新為版本 <color=red>{newVer}</color> 嗎？") != DialogResult.Yes)
            //        return;

            //    dt403_05_standardAtt.UploadDate = newVer;
            //}
            //    if (MsgTP.MsgYesNoQuestion($"你確定要將文件「<color=red>{dt403_05_standardAtt.UploadDate}</color>」從版本<color=red></color>更新為版本<color=red></color>嗎？") != DialogResult.Yes)
            //    return;

            dt403_05_standardAtt.StandardId = idBase;

            var baseAtt = new dm_Attachment()
            {
                ActualName = Path.GetFileName(baseFilePath),
                EncryptionName = EncryptionHelper.EncryptionFileName(baseFilePath),
                Thread = "40305"
            };

            var idAtt = dm_AttachmentBUS.Instance.Add(baseAtt);
            dt403_05_standardAtt.AttId = idAtt;

            if (Directory.Exists(TPConfigs.Folder40305))
                Directory.CreateDirectory(TPConfigs.Folder40305);

            File.Copy(baseFilePath, Path.Combine(TPConfigs.Folder40305, baseAtt.EncryptionName));

            var result = dt403_05_StandardAttBUS.Instance.AddOrUpdate(dt403_05_standardAtt);

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