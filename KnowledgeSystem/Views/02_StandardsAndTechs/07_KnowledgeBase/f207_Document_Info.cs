using DevExpress.XtraEditors;
using KnowledgeSystem.Configs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._07_KnowledgeBase
{
    public partial class f207_Document_Info : DevExpress.XtraEditors.XtraForm
    {
        public f207_Document_Info()
        {
            InitializeComponent();
        }

        public f207_Document_Info(int idType_)
        {
            InitializeComponent();
            idType = idType_;
        }

        public f207_Document_Info(string idDocumet_)
        {
            InitializeComponent();
            idDocument = idDocumet_;
        }

        BindingSource sourceAttachments = new BindingSource();

        string idDocument = string.Empty;
        string UserId = string.Empty;
        int idType = 0;

        List<KnowledgeType> lsKnowledgeTypes = new List<KnowledgeType>();
        List<User> lsUsers = new List<User>();

        List<Attachments> lsAttachments = new List<Attachments>();

        private class Attachments
        {
            public string FileName { get; set; }
            public string EncryptionName { get; set; }
            public string FullPath { get; set; }
        }

        private string GenerateEncryptionName()
        {
            var userLogin = lsUsers.Where(r => r.Id == UserId).FirstOrDefault();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var randomString = new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());

            return $"{userLogin.Id}-{userLogin.IdDepartment.Substring(0, 3)}-{DateTime.Now.ToString("MMddhhmmss")}-{randomString}";
        }

        private string GenerateIdDocument(int indexId = 1, string startIdstr_ = "")
        {
            int startNum = indexId;
            var IsExistsId = false;
            var userLogin = lsUsers.Where(r => r.Id == UserId).FirstOrDefault();
            string startIdStr = startIdstr_;

            if (startIdStr == "")
                startIdStr = $"{userLogin.IdDepartment.Substring(0, 3)}-{DateTime.Now.ToString("yyMMddHHmm")}-";

            string tempId = $"{startIdStr}{startNum:d2}";
            using (var db = new DBDocumentManagementSystemEntities())
            {
                var lsKnowledgeBases = db.KnowledgeBases.ToList();
                IsExistsId = lsKnowledgeBases.Any(r => r.Id == tempId);
            }

            if (!IsExistsId)
            {
                return tempId;
            }
            else
            {
                startNum++;
                return GenerateIdDocument(startNum, startIdStr);
            }
        }

        public static string convertToUnSign3(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        private void f207_DocumentInfo_Load(object sender, EventArgs e)
        {
            using (var db = new DBDocumentManagementSystemEntities())
            {
                lsKnowledgeTypes = db.KnowledgeTypes.ToList();
                lsUsers = db.Users.ToList();
            }

            // Gán data cho các combobox
            cbbType.Properties.DataSource = lsKnowledgeTypes;
            cbbType.Properties.ValueMember = "Id";
            cbbType.Properties.DisplayMember = "DisplayName";

            cbbUserRequest.Properties.DataSource = lsUsers;
            cbbUserRequest.Properties.ValueMember = "Id";
            cbbUserRequest.Properties.DisplayMember = "DisplayName";

            cbbUserUpload.Properties.DataSource = lsUsers;
            cbbUserUpload.Properties.ValueMember = "Id";
            cbbUserUpload.Properties.DisplayMember = "DisplayName";

            // Cài thông tin mặc định
            UserId = "VNW0014732";

            cbbType.EditValue = idType;
            cbbUserRequest.EditValue = UserId;
            cbbUserUpload.EditValue = UserId;

            gcFiles.DataSource = sourceAttachments;

            var userLogin = lsUsers.Where(r => r.Id == UserId).FirstOrDefault();
            txbId.Text = "XXX-XXXXXXXXXX-XX";

            lbCountFile.Text = "";

            if (idDocument != string.Empty)
            {
                using (var db = new DBDocumentManagementSystemEntities())
                {
                    var dataView = db.KnowledgeBases.Where(r => r.Id == idDocument).FirstOrDefault();

                    txbId.Text = dataView.Id;
                    cbbType.EditValue = dataView.IdTypes;
                    cbbUserRequest.EditValue = dataView.UserRequest;
                    cbbUserUpload.EditValue = dataView.UserUpload;

                    string[] displayName = dataView.DisplayName.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                    txbNameTW.Text = displayName[0];
                    if (displayName.Count() > 1)
                    {
                        txbNameVN.Text = displayName[1];
                    }
                    txbKeyword.Text = dataView.Keyword;

                    var lsAttachmentsDB = db.KnowledgeAttachments.Where(r => r.IdKnowledgeBase == idDocument).ToList();
                    foreach (var item in lsAttachmentsDB)
                    {
                        lsAttachments.Add(new Attachments() { FileName = item.FileName, EncryptionName = item.EncryptionName });
                    }

                    sourceAttachments.DataSource = lsAttachments;
                    lbCountFile.Text = $"共{lsAttachments.Count}個附件";
                }
            }
        }

        private void btnAddFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFiles = new OpenFileDialog();
            openFiles.Filter = "PDF|*.pdf";
            openFiles.Multiselect = true;
            if (openFiles.ShowDialog() != DialogResult.OK) return;

            lsAttachments = new List<Attachments>();
            foreach (var item in openFiles.FileNames)
            {
                string encryptionName = GenerateEncryptionName();
                Attachments attachment = new Attachments()
                {
                    FullPath = item,
                    FileName = Path.GetFileName(item),
                    EncryptionName = encryptionName
                };
                lsAttachments.Add(attachment);
                Thread.Sleep(5);
            }

            sourceAttachments.DataSource = lsAttachments;
            lbCountFile.Text = $"共{lsAttachments.Count}個附件";
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            string IdDocument = GenerateIdDocument();

            KnowledgeBase knowledge = new KnowledgeBase()
            {
                Id = IdDocument,
                DisplayName = $"{convertToUnSign3(txbNameTW.Text)}\r\n{txbNameVN.Text}",
                UserRequest = cbbUserRequest.EditValue.ToString(),
                IdTypes = (int)cbbType.EditValue,
                Keyword = txbKeyword.Text.Trim(),
                UserUpload = cbbUserUpload.EditValue.ToString(),
                UploadDate = DateTime.Now
            };

            using (var db = new DBDocumentManagementSystemEntities())
            {
                db.KnowledgeBases.Add(knowledge);

                foreach (Attachments item in lsAttachments)
                {
                    KnowledgeAttachment attachment = new KnowledgeAttachment()
                    {
                        IdKnowledgeBase = IdDocument,
                        EncryptionName = item.EncryptionName,
                        FileName = item.FileName
                    };
                    db.KnowledgeAttachments.Add(attachment);
                }

                db.SaveChanges();
            }

            XtraMessageBox.Show($"新增成功!\r\n文件編號:{IdDocument}", TempDatas.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }

        private void f207_DocumentInfo_Shown(object sender, EventArgs e)
        {
            txbNameTW.Focus();
        }
    }
}