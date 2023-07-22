using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Utils.Win.Hook;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting.Native.WebClientUIControl;
using KnowledgeSystem.Configs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Migrations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._07_KnowledgeBase
{
    public partial class f207_Document_Info : DevExpress.XtraEditors.XtraForm
    {
        RefreshHelper helper;

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

        #region parameters

        BindingSource sourceAttachments = new BindingSource();
        BindingSource sourceSecuritys = new BindingSource();

        string idDocument = string.Empty;
        string UserId = string.Empty;
        int idType = 0;

        List<KnowledgeType> lsKnowledgeTypes = new List<KnowledgeType>();
        List<User> lsUsers = new List<User>();
        List<Group> lsGroups = new List<Group>();
        List<KnowledgeSecurity> lsKnowledgeSecuritys = new List<KnowledgeSecurity>();
        List<Securityinfo> lsSecurityInfos = new List<Securityinfo>();

        List<Attachments> lsAttachments = new List<Attachments>();

        private class Attachments
        {
            public string FileName { get; set; }
            public string EncryptionName { get; set; }
            public string FullPath { get; set; }
        }

        private class Securityinfo : KnowledgeSecurity
        {
            public string IdGroupOrUser { get; set; }
            public string DisplayName { get; set; }
            public bool IsUser { get; set; }
        }

        #endregion

        #region methods

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

        #endregion

        private void f207_DocumentInfo_Load(object sender, EventArgs e)
        {
            helper = new RefreshHelper(bgvSecurity, "Id");

            gvEditHistory.ReadOnlyGridView();
            //gvFiles.ReadOnlyGridView();
            //bgvSecurity.ReadOnlyGridView();

            gcFiles.DataSource = sourceAttachments;
            gcSecurity.DataSource = sourceSecuritys;

            using (var db = new DBDocumentManagementSystemEntities())
            {
                lsKnowledgeTypes = db.KnowledgeTypes.ToList();
                lsUsers = db.Users.ToList();
                lsGroups = db.Groups.ToList();

                var lsIdUsers = lsUsers.Select(r => new Securityinfo { IdGroupOrUser = r.Id, DisplayName = r.DisplayName }).ToList();
                var lsIdGroup = lsGroups.Select(r => new Securityinfo { IdGroupOrUser = r.Id.ToString(), DisplayName = r.DisplayName }).ToList();

                var lsIdGroupOrUser = new List<Securityinfo>();
                lsIdGroupOrUser.AddRange(lsIdGroup);
                lsIdGroupOrUser.AddRange(lsIdUsers);

                rgvGruopOrUser.DataSource = lsIdGroupOrUser;
                rgvGruopOrUser.ValueMember = "IdGroupOrUser";
                rgvGruopOrUser.DisplayMember = "DisplayName";

                // Add two columns in the dropdown:
                GridColumn col1 = rgvGruopOrUser.PopupView.Columns.AddField("IdGroupOrUser");
                col1.VisibleIndex = 0;
                col1.Caption = "代號";
                GridColumn col2 = rgvGruopOrUser.PopupView.Columns.AddField("DisplayName");
                col2.VisibleIndex = 1;
                col2.Caption = "名稱";
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

                    // KnowledgeSecurity
                    helper.SaveViewInfo();
                    lsKnowledgeSecuritys = db.KnowledgeSecurities.Where(r => r.IdKnowledgeBase == idDocument).ToList();

                    lsSecurityInfos = (from data in lsKnowledgeSecuritys
                                       select new Securityinfo
                                       {
                                           IdKnowledgeBase = data.IdKnowledgeBase,
                                           IdGroup = data.IdGroup,
                                           IdUser = data.IdUser,
                                           ChangePermision = data.ChangePermision,
                                           ReadInfo = data.ReadInfo,
                                           UpdateInfo = data.UpdateInfo,
                                           DeleteInfo = data.DeleteInfo,
                                           SearchInfo = data.SearchInfo,
                                           ReadFile = data.ReadFile,
                                           PrintFile = data.PrintFile,
                                           SaveFile = data.SaveFile,
                                           IsUser = !string.IsNullOrEmpty(data.IdUser),
                                           IdGroupOrUser = !string.IsNullOrEmpty(data.IdUser) ? data.IdUser : data.IdGroup.ToString(),
                                       }).ToList();
                }
            }

            sourceSecuritys.DataSource = lsSecurityInfos;
            bgvSecurity.BestFitColumns();
            helper.LoadViewInfo();
        }

        private void btnAddFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFiles = new OpenFileDialog();
            openFiles.Filter = "PDF|*.pdf";
            openFiles.Multiselect = true;
            if (openFiles.ShowDialog() != DialogResult.OK) return;

            //lsAttachments = new List<Attachments>();
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
            gvFiles.RefreshData();
        }

        private void f207_DocumentInfo_Shown(object sender, EventArgs e)
        {
            txbNameTW.Focus();
        }

        private void btnDel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            bgvSecurity.FocusedRowHandle = -1;

            string events = "新增成功";
            string IdDocument = GenerateIdDocument();
            if (idDocument != string.Empty)
            {
                IdDocument = idDocument;
                events = "修改成功";
            }

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
                db.KnowledgeBases.AddOrUpdate(knowledge);

                db.KnowledgeAttachments.RemoveRange(db.KnowledgeAttachments.Where(r => r.IdKnowledgeBase == idDocument));
                var lsFileOld = lsAttachments.Where(r => string.IsNullOrEmpty(r.FullPath)).ToList();
                var lsFileNew = lsAttachments.Where(r => !string.IsNullOrEmpty(r.FullPath)).ToList();
                foreach (Attachments item in lsFileOld)
                {
                    KnowledgeAttachment attachment = new KnowledgeAttachment()
                    {
                        IdKnowledgeBase = IdDocument,
                        EncryptionName = item.EncryptionName,
                        FileName = item.FileName
                    };
                    db.KnowledgeAttachments.AddOrUpdate(attachment);
                }

                foreach (Attachments item in lsFileNew)
                {
                    KnowledgeAttachment attachment = new KnowledgeAttachment()
                    {
                        IdKnowledgeBase = IdDocument,
                        EncryptionName = item.EncryptionName,
                        FileName = item.FileName
                    };
                    db.KnowledgeAttachments.AddOrUpdate(attachment);

                    // Them file vao o chung tai day
                }

                db.KnowledgeSecurities.RemoveRange(db.KnowledgeSecurities.Where(r => r.IdKnowledgeBase == idDocument));
                foreach (var item in lsSecurityInfos)
                {
                    if (item.IdGroupOrUser.StartsWith("VNW"))
                    {
                        item.IdUser = item.IdGroupOrUser;
                    }
                    else
                    {
                        item.IdGroup = Convert.ToInt16(item.IdGroupOrUser);
                    }

                    KnowledgeSecurity dataAdd = new KnowledgeSecurity
                    {
                        Id = item.Id,
                        IdKnowledgeBase = IdDocument,
                        IdGroup = item.IdGroup,
                        IdUser = item.IdUser,
                        ChangePermision = item.ChangePermision,
                        ReadInfo = item.ReadInfo,
                        UpdateInfo = item.UpdateInfo,
                        DeleteInfo = item.DeleteInfo,
                        SearchInfo = item.SearchInfo,
                        ReadFile = item.ReadFile,
                        PrintFile = item.PrintFile,
                        SaveFile = item.SaveFile
                    };

                    db.KnowledgeSecurities.Add(dataAdd);
                }


                db.SaveChanges();
            }

            XtraMessageBox.Show($"{events}!\r\n文件編號:{IdDocument}", TempDatas.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }

        private void btnAddPermission_Click(object sender, EventArgs e)
        {
            lsSecurityInfos.Add(new Securityinfo()
            {
                IdKnowledgeBase = idDocument,
                ChangePermision = false,
                ReadInfo = true,
                UpdateInfo = false,
                DeleteInfo = false,
                SearchInfo = true,
                ReadFile = false,
                PrintFile = false,
                SaveFile = false,
            });

            sourceSecuritys.DataSource = lsSecurityInfos;
            bgvSecurity.RefreshData();
        }

        private void btnDelFile_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            Attachments attachment = gvFiles.GetRow(gvFiles.FocusedRowHandle) as Attachments;

            DialogResult dialog = XtraMessageBox.Show($"Bạn muốn xoá phụ kiện: {attachment.FileName}?", TempDatas.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialog == DialogResult.No) return;

            lsAttachments.Remove(attachment);
            lbCountFile.Text = $"共{lsAttachments.Count}個附件";

            var index = gvFiles.FocusedRowHandle;
            gvFiles.RefreshData();
            gvFiles.FocusedRowHandle = index;
        }

        private void btnDelPermission_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            Securityinfo permission = bgvSecurity.GetRow(bgvSecurity.FocusedRowHandle) as Securityinfo;

            DialogResult dialog = XtraMessageBox.Show($"Bạn muốn xoá phụ kiện: {permission.DisplayName}?", TempDatas.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialog == DialogResult.No) return;

            lsSecurityInfos.Remove(permission);

            //var index = gvFiles.FocusedRowHandle;
            bgvSecurity.RefreshData();
            //gvFiles.FocusedRowHandle = index;
        }
    }
}