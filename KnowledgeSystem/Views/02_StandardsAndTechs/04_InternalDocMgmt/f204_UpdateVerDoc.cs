using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
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
using System.Windows.Forms;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._04_InternalDocMgmt
{
    public partial class f204_UpdateVerDoc : DevExpress.XtraEditors.XtraForm
    {
        public f204_UpdateVerDoc()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public EventFormInfo eventInfo = EventFormInfo.Create;
        public string formName = "";
        public int idBase = -1;

        string baseFilePath = "";

        BindingSource sourceFormAtts = new BindingSource();
        List<Attachment> baseFormAtts = new List<Attachment>();

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

        private void f204_UpdateVerDoc_Load(object sender, EventArgs e)
        {
            gcForm.DataSource = sourceFormAtts;

            lcControls = new List<LayoutControlItem>() { lcDocVersion, lcFilePath, lcDeployDate, lcIdFounder };
            lcImpControls = new List<LayoutControlItem>() { lcDocVersion, lcFilePath, lcDeployDate, lcIdFounder };

            foreach (var item in lcControls)
            {
                item.AllowHtmlStringInCaption = true;
                item.Text = $"<color=#000000>{item.Text}</color>";
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

            var lsUsers = dm_UserBUS.Instance.GetList()
                .Where(r => r.Status == 0)
                .Select(r => new dm_User
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

            var dt204Base = dt204_InternalDocMgmtBUS.Instance.GetItemById(idBase);

            string oldVer = dt204Base.DocVersion;
            string newVer = txbDocVersion.Text.Trim();

            if (MsgTP.MsgYesNoQuestion($"你確定要將文件「<color=red>{dt204Base.DisplayName}</color>」從版本<color=red>{oldVer}</color>更新為版本<color=red>{newVer}</color>嗎？") != DialogResult.Yes)
                return;

            dt204Base.DocVersion = txbDocVersion.Text.Trim();
            dt204Base.IdUsrUpload = TPConfigs.LoginUser.Id;
            dt204Base.UploadDate = DateTime.Now;
            dt204Base.PauseNotify = null;
            dt204Base.IsDel = false;
            dt204Base.DeployDate = txbDeployDate.DateTime;
            dt204Base.IdFounder = txbIdFounder.EditValue.ToString();

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

            var result = dt204_InternalDocMgmtBUS.Instance.AddOrUpdate(dt204Base);

            Handle204Form(dt204Base.Id);

            if (result)
            {
                Close();
            }
            else
            {
                MsgTP.MsgErrorDB();
            }
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

        private void btnPasteFile_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsFileDropList())
            {
                var files = Clipboard.GetFileDropList();

                // Lấy danh sách các phần mở rộng từ bộ lọc
                var extensions = TPConfigs.FilterFile.Split('|')[1].Split(';')
                                       .Select(ext => ext.TrimStart('*').ToLower()).ToList();

                // Lọc file dựa trên phần mở rộng
                var filteredFiles = files.Cast<string>()
                                         .Where(file => extensions.Any(ext => file.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                                         .ToList();

                foreach (string fileName in filteredFiles)
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
            else
            {
                XtraMessageBox.Show("請選表單", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}