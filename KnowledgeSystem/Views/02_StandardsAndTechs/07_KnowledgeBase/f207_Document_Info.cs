using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Win.Hook;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting.Native.WebClientUIControl;
using DevExpress.XtraSplashScreen;
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
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._07_KnowledgeBase
{
    public partial class f207_Document_Info : DevExpress.XtraEditors.XtraForm
    {
        RefreshHelper helper;

        public f207_Document_Info()
        {
            InitializeComponent();
            LockControl(false);
        }

        public f207_Document_Info(string idDocument_)
        {
            InitializeComponent();
            idDocument = idDocument_;

            LockControl(true);
        }

        #region parameters

        BindingSource sourceAttachments = new BindingSource();
        BindingSource sourceSecuritys = new BindingSource();

        string idDocument = string.Empty;
        string userId = string.Empty;

        List<KnowledgeType> lsKnowledgeTypes = new List<KnowledgeType>();
        List<User> lsUsers = new List<User>();
        List<Group> lsGroups = new List<Group>();
        List<Securityinfo> lsSecurityInfos = new List<Securityinfo>();
        List<Attachments> lsAttachments = new List<Attachments>();
        List<Securityinfo> lsIdGroupOrUser = new List<Securityinfo>();
        List<GroupUser> lsGroupUser = new List<GroupUser>();

        Securityinfo permissionAttachments = new Securityinfo();

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
        }

        #endregion

        #region methods

        private string GenerateEncryptionName()
        {
            var userLogin = lsUsers.FirstOrDefault(u => u.Id == userId);
            if (userLogin == null)
            {
                throw new Exception($"User with Id {userId} not found.");
            }

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var randomString = new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)])
                .ToArray());

            return $"{userLogin.Id}-{userLogin.IdDepartment.Substring(0, 3)}-{DateTime.Now:MMddhhmmss}-{randomString}.pdf";
        }

        private string GenerateIdDocument(int indexId = 1, string startIdStr = "")
        {
            var userLogin = lsUsers.FirstOrDefault(u => u.Id == userId);
            if (userLogin == null)
            {
                throw new Exception($"User with Id {userId} not found.");
            }

            if (string.IsNullOrEmpty(startIdStr))
            {
                startIdStr = $"{userLogin.IdDepartment.Substring(0, 3)}-{DateTime.Now.ToString("yyMMddHHmm")}-";
            }

            string tempId = $"{startIdStr}{indexId:d2}";
            using (var db = new DBDocumentManagementSystemEntities())
            {
                bool isExistsId = db.KnowledgeBases.Any(kb => kb.Id == tempId);
                if (!isExistsId)
                {
                    return tempId;
                }
            }

            return GenerateIdDocument(indexId + 1, startIdStr);
        }

        public static string convertToUnSign3(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        private void LockControl(bool isFormView = true)
        {
            //txbName.ReadOnly = isFormView;
            //txbDescribe.ReadOnly = isFormView;

            //btnAddUser.Enabled = !isFormView;
            //btnDelUser.Enabled = !isFormView;

            btnEdit.Visibility = isFormView ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
            btnDel.Visibility = isFormView ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
            btnConfirm.Visibility = isFormView ? DevExpress.XtraBars.BarItemVisibility.Never : DevExpress.XtraBars.BarItemVisibility.Always;

            Text = isFormView ? "群組信息" : "新增、修改群組";
        }

        private Securityinfo GetPermission()
        {
            string userIdLogin = TempDatas.LoginId;

            if (cbbUserUpload.EditValue.ToString() == userIdLogin)
            {
                return new Securityinfo()
                {
                    ChangePermision = true,
                    DeleteInfo = true,
                    UpdateInfo = true,
                    ReadFile = true,
                    SaveFile = true,
                    PrintFile = true
                };
            }

            if (lsSecurityInfos.Any(r => r.IdUser == userIdLogin))
            {
                return lsSecurityInfos.FirstOrDefault(r => r.IdUser == userIdLogin);
            }

            return (from groups in lsGroups.OrderBy(g => g.Prioritize)
                    join securitys in lsSecurityInfos on groups.Id equals securitys.IdGroup
                    join gruopUser in lsGroupUser.Where(r => r.IdUser == userIdLogin) on groups.Id equals gruopUser.IdGroup
                    select new Securityinfo
                    {
                        ChangePermision = securitys.ChangePermision,
                        DeleteInfo = securitys.DeleteInfo,
                        UpdateInfo = securitys.UpdateInfo,
                        ReadFile = securitys.ReadFile,
                        PrintFile = securitys.PrintFile,
                        SaveFile = securitys.SaveFile,
                    }).FirstOrDefault();
        }

        #endregion

        private void f207_DocumentInfo_Load(object sender, EventArgs e)
        {
            // Initialize RefreshHelper
            helper = new RefreshHelper(bgvSecurity, "Id");

            controlgroupDocument.SelectedTabPage = lcgInfo;

            // Set GridView to read-only and assign data sources to grid controls
            gvEditHistory.ReadOnlyGridView();
            gvFiles.ReadOnlyGridView();
            gcFiles.DataSource = sourceAttachments;
            gcSecurity.DataSource = sourceSecuritys;

            // Query the database and assign data sources to ComboBoxes and GridView
            using (var db = new DBDocumentManagementSystemEntities())
            {
                // Initialize lists
                lsKnowledgeTypes = db.KnowledgeTypes.ToList();
                lsUsers = db.Users.ToList();
                lsGroups = db.Groups.ToList();
                lsGroupUser = db.GroupUsers.ToList();

                // Create lists of Securityinfo objects from lsUsers and lsGroups
                var lsIdUsers = lsUsers.Select(r => new Securityinfo { IdGroupOrUser = r.Id, DisplayName = r.DisplayName }).ToList();
                var lsIdGroup = lsGroups.Select(r => new Securityinfo { IdGroupOrUser = r.Id.ToString(), DisplayName = r.DisplayName }).ToList();
                lsIdGroupOrUser = lsIdGroup.Concat(lsIdUsers).ToList();

                // Assign data source and columns to rgvGruopOrUser
                rgvGruopOrUser.DataSource = lsIdGroupOrUser;
                rgvGruopOrUser.ValueMember = "IdGroupOrUser";
                rgvGruopOrUser.DisplayMember = "DisplayName";
                rgvGruopOrUser.PopupView.Columns.AddRange(new[]
                {
                    new GridColumn { FieldName = "IdGroupOrUser", VisibleIndex = 0, Caption = "代號" },
                    new GridColumn { FieldName = "DisplayName", VisibleIndex = 1, Caption = "名稱" }
                });

                // Assign data sources to ComboBoxes
                cbbType.Properties.DataSource = lsKnowledgeTypes;
                cbbType.Properties.ValueMember = "Id";
                cbbType.Properties.DisplayMember = "DisplayName";
                cbbUserRequest.Properties.DataSource = lsUsers;
                cbbUserRequest.Properties.ValueMember = "Id";
                cbbUserRequest.Properties.DisplayMember = "DisplayName";
                cbbUserUpload.Properties.DataSource = lsUsers;
                cbbUserUpload.Properties.ValueMember = "Id";
                cbbUserUpload.Properties.DisplayMember = "DisplayName";

                // Set default values
                userId = TempDatas.LoginId;
                cbbType.EditValue = 1;
                cbbUserRequest.EditValue = userId;
                cbbUserUpload.EditValue = userId;
                var userLogin = lsUsers.FirstOrDefault(r => r.Id == userId);
                txbId.Text = "XXX-XXXXXXXXXX-XX";
                lbCountFile.Text = "";

                // If idDocument is not empty, retrieve data from the database and assign to form elements
                if (!string.IsNullOrEmpty(idDocument))
                {
                    var dataView = db.KnowledgeBases.FirstOrDefault(r => r.Id == idDocument);
                    txbId.Text = dataView.Id;
                    cbbType.EditValue = dataView.IdTypes;
                    cbbUserRequest.EditValue = dataView.UserRequest;
                    cbbUserUpload.EditValue = dataView.UserUpload;
                    var displayName = dataView.DisplayName.Split(new[] { "\r\n" }, StringSplitOptions.None);
                    txbNameTW.Text = displayName[0];
                    if (displayName.Length > 1)
                    {
                        txbNameVN.Text = displayName[1];
                    }
                    txbKeyword.Text = dataView.Keyword;

                    // Retrieve attachments from database and assign to sourceAttachments
                    lsAttachments.AddRange(db.KnowledgeAttachments.Where(r => r.IdKnowledgeBase == idDocument)
                        .Select(item => new Attachments { FileName = item.FileName, EncryptionName = item.EncryptionName }));

                    sourceAttachments.DataSource = lsAttachments;
                    lbCountFile.Text = $"共{lsAttachments.Count}個附件";

                    // Retrieve knowledge security information from database and assign to sourceSecuritys
                    helper.SaveViewInfo();
                    var lsKnowledgeSecuritys = db.KnowledgeSecurities.Where(r => r.IdKnowledgeBase == idDocument).ToList();
                    lsSecurityInfos = lsKnowledgeSecuritys.Select(data => new Securityinfo
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
                        IdGroupOrUser = !string.IsNullOrEmpty(data.IdUser) ? data.IdUser : data.IdGroup.ToString()
                    }).ToList();
                    sourceSecuritys.DataSource = lsSecurityInfos;
                    bgvSecurity.BestFitColumns();
                    helper.LoadViewInfo();
                }
            }

            permissionAttachments = GetPermission();
        }

        private void btnAddFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            foreach (string fileName in openFileDialog.FileNames)
            {
                string encryptionName = GenerateEncryptionName();
                Attachments attachment = new Attachments()
                {
                    FullPath = fileName,
                    FileName = Path.GetFileName(fileName),
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
            if (permissionAttachments.DeleteInfo != true)
            {
                MessageBox.Show(TempDatas.NoPermission);
                return;
            }

            MessageBox.Show("co quyen");
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Clear the selected row in the security grid view
            bgvSecurity.FocusedRowHandle = -1;

            // Determine whether to add or update the document and set the appropriate message
            string events = idDocument == string.Empty ? "新增成功" : "修改成功";
            string idDocumentToUpdate = string.IsNullOrEmpty(idDocument) ? GenerateIdDocument() : idDocument;

            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                // Create a new KnowledgeBase object with the relevant data
                KnowledgeBase knowledge = new KnowledgeBase()
                {
                    Id = idDocumentToUpdate,
                    DisplayName = $"{convertToUnSign3(txbNameTW.Text)}\r\n{txbNameVN.Text}",
                    UserRequest = cbbUserRequest.EditValue.ToString(),
                    IdTypes = (int)cbbType.EditValue,
                    Keyword = txbKeyword.Text.Trim(),
                    UserUpload = cbbUserUpload.EditValue.ToString(),
                    UploadDate = DateTime.Now
                };

                // Use a new instance of the database context to add or update the knowledge base object and its attachments and security information
                using (var db = new DBDocumentManagementSystemEntities())
                {
                    // Add or update the knowledge base object
                    db.KnowledgeBases.AddOrUpdate(knowledge);

                    // If updating an existing document, remove its existing attachments
                    if (!string.IsNullOrEmpty(idDocument))
                    {
                        db.KnowledgeAttachments.RemoveRange(db.KnowledgeAttachments.Where(r => r.IdKnowledgeBase == idDocument));
                    }

                    // Add the new attachments
                    foreach (Attachments item in lsAttachments)
                    {
                        KnowledgeAttachment attachment = new KnowledgeAttachment()
                        {
                            IdKnowledgeBase = idDocumentToUpdate,
                            EncryptionName = item.EncryptionName,
                            FileName = item.FileName
                        };
                        db.KnowledgeAttachments.AddOrUpdate(attachment);

                        if (!string.IsNullOrEmpty(item.FullPath))
                        {
                            string sourceFileName = item.FullPath;
                            string destFileName = Path.Combine(TempDatas.PahtDataFile, item.EncryptionName);

                            File.Copy(sourceFileName, destFileName, true);
                        }
                    }

                    // Remove the existing security information and add the new information
                    db.KnowledgeSecurities.RemoveRange(db.KnowledgeSecurities.Where(r => r.IdKnowledgeBase == idDocument));
                    foreach (var item in lsSecurityInfos)
                    {
                        KnowledgeSecurity dataAdd = new KnowledgeSecurity
                        {
                            Id = item.Id,
                            IdKnowledgeBase = idDocumentToUpdate,
                            IdGroup = item.IdGroupOrUser.StartsWith("VNW") ? null : (short?)Convert.ToInt16(item.IdGroupOrUser),
                            IdUser = item.IdGroupOrUser.StartsWith("VNW") ? item.IdGroupOrUser : null,
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

                    // Save the changes to the database
                    db.SaveChanges();
                }
            }

            // Show a message box with the appropriate message and close the form
            XtraMessageBox.Show($"{events}!\r\n文件編號:{idDocumentToUpdate}", TempDatas.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            DialogResult dialogResult = XtraMessageBox.Show($"您想要刪除附件：{attachment.FileName}?", TempDatas.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.No) return;

            lsAttachments.Remove(attachment);
            lbCountFile.Text = $"共{lsAttachments.Count}個附件";

            int rowIndex = gvFiles.FocusedRowHandle;
            gvFiles.RefreshData();
            gvFiles.FocusedRowHandle = rowIndex;
        }

        private void btnDelPermission_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            Securityinfo permission = bgvSecurity.GetRow(bgvSecurity.FocusedRowHandle) as Securityinfo;
            permission.DisplayName = lsIdGroupOrUser.FirstOrDefault(r => r.IdGroupOrUser == permission.IdGroupOrUser)?.DisplayName;

            DialogResult dialogResult = XtraMessageBox.Show($"您想要刪除權限：{permission.DisplayName}?", TempDatas.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.No) return;
            lsSecurityInfos.Remove(permission);
            bgvSecurity.RefreshData();
        }

        private void gcFiles_DoubleClick(object sender, EventArgs e)
        {
            int forcusRow = gvFiles.FocusedRowHandle;
            if (forcusRow < 0) return;

            Attachments dataRow = gvFiles.GetRow(forcusRow) as Attachments;
            string documentsFile = Path.Combine(TempDatas.PahtDataFile, dataRow.EncryptionName);

            f207_ViewPdf fDocumentInfo = new f207_ViewPdf(documentsFile);
            fDocumentInfo.Text = dataRow.FileName;
            fDocumentInfo.CanSaveFile = permissionAttachments.SaveFile;
            fDocumentInfo.CanPrintFile = permissionAttachments.PrintFile;
            fDocumentInfo.ShowDialog();
        }

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LockControl(false);
        }
    }
}