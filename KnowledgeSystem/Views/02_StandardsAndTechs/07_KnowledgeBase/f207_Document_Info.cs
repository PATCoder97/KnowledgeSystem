using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.DocumentView;
using DevExpress.Utils.About;
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
        dm_Progress progressSelect = new dm_Progress();

        int finishStep = -1;
        string events = "新增成功";

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
            cbbType.ReadOnly = isFormView;
            txbNameTW.ReadOnly = isFormView;
            txbNameVN.ReadOnly = isFormView;
            cbbUserRequest.ReadOnly = isFormView;
            cbbUserUpload.ReadOnly = isFormView;
            txbKeyword.ReadOnly = isFormView;

            btnAddFile.Enabled = !isFormView;
            btnAddPermission.Enabled = !isFormView;

            gvFiles.OptionsBehavior.ReadOnly = isFormView;
            bgvSecurity.OptionsBehavior.ReadOnly = isFormView;

            gColDelFile.Visible = !isFormView;
            gColDelPermission.Visible = !isFormView;

            btnEdit.Visibility = isFormView ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
            btnDel.Visibility = isFormView ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
            btnConfirm.Visibility = isFormView ? DevExpress.XtraBars.BarItemVisibility.Never : DevExpress.XtraBars.BarItemVisibility.Always;
            btnChangeProgress.Visibility = isFormView ? DevExpress.XtraBars.BarItemVisibility.Never : DevExpress.XtraBars.BarItemVisibility.Always;
            lcProgress.Visibility = isFormView ? DevExpress.XtraLayout.Utils.LayoutVisibility.Never : DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

            Text = isFormView ? "群組信息" : "新增、修改群組";

            btnApproved.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnDisapprove.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnCancel.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
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
            groupProgress.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            controlgroupDocument.SelectedTabPage = lcgInfo;

            // Initialize RefreshHelper
            helper = new RefreshHelper(bgvSecurity, "Id");

            // Set GridView to read-only and assign data sources to grid controls
            gvEditHistory.ReadOnlyGridView();
            gvFiles.ReadOnlyGridView();
            gvHistoryProcess.ReadOnlyGridView();
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
                progressSelect = db.dm_Progress.FirstOrDefault();

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
                lbProgress.Text = "流程：" + progressSelect.DisplayName;

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


                    var lsDocProgresses = db.dt207_DocProgress.Where(r => !r.IsComplete && r.IdKnowledgeBase == idDocument).ToList();


                    if (lsDocProgresses.Count != 0)
                    {
                        groupProgress.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        controlgroupDocument.SelectedTabPage = groupProgress;

                        int idProgressByDoc = lsDocProgresses.First().IdProgress;
                        int idDocProgress = lsDocProgresses.First().Id;// !!!!!!!!Chuyen len lam bien toan cuc!!!!!!!! Chua lam
                        var lsDMStepProgress = db.dm_StepProgress.Where(r => r.IdProgress == idProgressByDoc).ToList();
                        var lsDocProgressInfos = db.DocProgressInfoes.Where(r => r.IdDocProgress == idDocProgress).ToList();

                        finishStep = lsDMStepProgress.Max(r => r.IndexStep);

                        var lsStepProgressDoc = (from data in lsDMStepProgress
                                                 join groups in lsGroups on data.IdGroup equals groups.Id
                                                 select new { groups.DisplayName }).ToList();

                        stepProgressDoc.Items.Add(new StepProgressBarItem("經辦人"));
                        foreach (var item in lsStepProgressDoc)
                        {
                            stepProgressDoc.Items.Add(new StepProgressBarItem(item.DisplayName));
                        }

                        var stepNow = lsDocProgressInfos.OrderByDescending(r => r.TimeStep).First().IndexStep;

                        stepProgressDoc.ItemOptions.Indicator.Width = 40;
                        stepProgressDoc.SelectedItemIndex = stepNow;

                        var lsHistoryProcess = (from data in lsDocProgressInfos
                                                join users in db.Users on data.IdUserProcess equals users.Id
                                                select new
                                                {
                                                    TimeStep = data.TimeStep,
                                                    data.Descriptions,
                                                    UserProcess = $"{users.IdDepartment} | {data.IdUserProcess}/{users.DisplayName}",
                                                }).ToList();

                        gcHistoryProcess.DataSource = lsHistoryProcess;

                        if (stepNow != -1)
                        {
                            btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                            btnApproved.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                            btnDisapprove.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                        }
                        else
                        {
                            btnCancel.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                            btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                            btnApproved.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                            btnDisapprove.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                        }

                        btnDel.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                        btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    }
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
                XtraMessageBox.Show(TempDatas.NoPermission, TempDatas.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            MessageBox.Show("co quyen");
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Clear the selected row in the security grid view
            bgvSecurity.FocusedRowHandle = -1;

            // Determine whether to add or update the document and set the appropriate message
            events = idDocument == string.Empty ? TempDatas.EventNew : TempDatas.EventEdit;
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

                    List<dt207_DocProgress> lsDocProgressById = db.dt207_DocProgress.Where(r => r.IdKnowledgeBase == idDocumentToUpdate).ToList();

                    bool IsNewProgress = !lsDocProgressById.Any(r => !r.IsComplete);
                    if (IsNewProgress)
                    {
                        dt207_DocProgress docProgress = new dt207_DocProgress()
                        {
                            IdKnowledgeBase = idDocumentToUpdate,
                            IsComplete = false,
                            IsSuccess = false,
                            IdProgress = progressSelect.Id,
                            Descriptions = events,
                        };

                        db.dt207_DocProgress.Add(docProgress);
                    }
                    // Save the changes to the database
                    db.SaveChanges();

                    lsDocProgressById = db.dt207_DocProgress.Where(r => r.IdKnowledgeBase == idDocumentToUpdate).ToList();
                    int idDocProgress = lsDocProgressById.OrderByDescending(r => r.Id).FirstOrDefault().Id;

                    //List<DocProgressInfo> lsDocProgressInfos = db.DocProgressInfoes.Where(r => r.IdDocProgress == idDocProgress).ToList();
                    //int indexStep = lsDocProgressInfos.Count != 0 ? lsDocProgressInfos.OrderByDescending(r => r.Id).FirstOrDefault().IndexStep + 1 : 0;

                    DocProgressInfo progressInfo = new DocProgressInfo()
                    {
                        IdDocProgress = idDocProgress,
                        TimeStep = DateTime.Now,
                        IndexStep = 0,
                        IdUserProcess = TempDatas.LoginId,
                        Descriptions = events,
                    };

                    db.DocProgressInfoes.Add(progressInfo);

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
            if (permissionAttachments.ReadFile != true)
            {
                XtraMessageBox.Show(TempDatas.NoPermission, TempDatas.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            int forcusRow = gvFiles.FocusedRowHandle;
            if (forcusRow < 0) return;

            Attachments dataRow = gvFiles.GetRow(forcusRow) as Attachments;
            string documentsFile = Path.Combine(TempDatas.PahtDataFile, dataRow.EncryptionName);

            if (!string.IsNullOrEmpty(idDocument))
            {
                using (var db = new DBDocumentManagementSystemEntities())
                {
                    KnowledgeHistoryGetFile historyGetFile = new KnowledgeHistoryGetFile()
                    {
                        IdKnowledgeBase = idDocument,
                        idTypeHisGetFile = 1,
                        KnowledgeAttachmentName = dataRow.FileName,
                        IdUser = TempDatas.LoginId,
                        TimeGet = DateTime.Now
                    };

                    db.KnowledgeHistoryGetFiles.Add(historyGetFile);
                    db.SaveChanges();
                }
            }

            f207_ViewPdf fDocumentInfo = new f207_ViewPdf(documentsFile, idDocument);
            fDocumentInfo.Text = dataRow.FileName;
            fDocumentInfo.CanSaveFile = permissionAttachments.SaveFile;
            fDocumentInfo.CanPrintFile = permissionAttachments.PrintFile;
            fDocumentInfo.ShowDialog();
        }

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LockControl(false);
        }

        private void btnChangeProgress_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            uc207_SelectProgress ucInfo = new uc207_SelectProgress();
            if (XtraDialog.Show(ucInfo, "...流程", MessageBoxButtons.OKCancel) != DialogResult.OK) return;

            progressSelect = ucInfo.ProgressSelect;
            lbProgress.Text = "流程：" + progressSelect.DisplayName;
        }

        private void btnApproved_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (var db = new DBDocumentManagementSystemEntities())
            {
                var lsDocProgressById = db.dt207_DocProgress.Where(r => r.IdKnowledgeBase == idDocument).ToList();
                int idDocProgress = lsDocProgressById.OrderByDescending(r => r.Id).FirstOrDefault().Id;

                List<DocProgressInfo> lsDocProgressInfos = db.DocProgressInfoes.Where(r => r.IdDocProgress == idDocProgress).ToList();
                int indexStep = lsDocProgressInfos.Count != 0 ? lsDocProgressInfos.OrderByDescending(r => r.Id).FirstOrDefault().IndexStep + 1 : 0;

                string descriptions = "核准";

                if (indexStep == finishStep)
                {
                    var docProcessUpdate = db.dt207_DocProgress.Where(r => r.Id == idDocProgress).First();
                    docProcessUpdate.IsSuccess = true;
                    docProcessUpdate.IsComplete = true;

                    db.dt207_DocProgress.AddOrUpdate(docProcessUpdate);

                    descriptions = "已完成";
                }

                DocProgressInfo progressInfo = new DocProgressInfo()
                {
                    IdDocProgress = idDocProgress,
                    TimeStep = DateTime.Now,
                    IndexStep = indexStep,
                    IdUserProcess = TempDatas.LoginId,
                    Descriptions = descriptions,
                };

                db.DocProgressInfoes.Add(progressInfo);

                // Save the changes to the database
                db.SaveChanges();
            }

            Close();
        }

        private void btnDisapprove_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XtraInputBoxArgs args = new XtraInputBoxArgs();
            // set required Input Box options
            args.Caption = TempDatas.SoftNameTW;
            args.Prompt = "退回文件原因";
            args.DefaultButtonIndex = 0;
            MemoEdit editor = new MemoEdit();
            args.Editor = editor;
            // display an Input Box with the custom editor
            var result = XtraInputBox.Show(args);
            string descriptions = result == null ? "" : result.ToString();

            using (var db = new DBDocumentManagementSystemEntities())
            {
                var lsDocProgressById = db.dt207_DocProgress.Where(r => r.IdKnowledgeBase == idDocument).ToList();
                int idDocProgress = lsDocProgressById.OrderByDescending(r => r.Id).FirstOrDefault().Id;

                DocProgressInfo progressInfo = new DocProgressInfo()
                {
                    IdDocProgress = idDocProgress,
                    TimeStep = DateTime.Now,
                    IndexStep = -1,
                    IdUserProcess = TempDatas.LoginId,
                    Descriptions = string.IsNullOrEmpty(descriptions) ? "退回" : $"退回，說明：{descriptions}",
                };

                db.DocProgressInfoes.Add(progressInfo);

                // Save the changes to the database
                db.SaveChanges();
            }

            Close();
        }

        private void btnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XtraInputBoxArgs args = new XtraInputBoxArgs();
            // set required Input Box options
            args.Caption = TempDatas.SoftNameTW;
            args.Prompt = "原因";
            args.DefaultButtonIndex = 0;
            MemoEdit editor = new MemoEdit();
            args.Editor = editor;
            // display an Input Box with the custom editor
            var result = XtraInputBox.Show(args);
            string descriptions = result == null ? "" : result.ToString();

            using (var db = new DBDocumentManagementSystemEntities())
            {
                var lsDocProgressById = db.dt207_DocProgress.Where(r => r.IdKnowledgeBase == idDocument).ToList();
                int idDocProgress = lsDocProgressById.OrderByDescending(r => r.Id).FirstOrDefault().Id;


                var docProcessUpdate = db.dt207_DocProgress.Where(r => r.Id == idDocProgress).First();
                docProcessUpdate.IsSuccess = false;
                docProcessUpdate.IsComplete = true;
                db.dt207_DocProgress.AddOrUpdate(docProcessUpdate);

                DocProgressInfo progressInfo = new DocProgressInfo()
                {
                    IdDocProgress = idDocProgress,
                    TimeStep = DateTime.Now,
                    IndexStep = -1,
                    IdUserProcess = TempDatas.LoginId,
                    Descriptions = string.IsNullOrEmpty(descriptions) ? "取消" : $"取消，說明：{descriptions}",
                };

                db.DocProgressInfoes.Add(progressInfo);

                // Save the changes to the database
                db.SaveChanges();
            }

            Close();
        }
    }
}