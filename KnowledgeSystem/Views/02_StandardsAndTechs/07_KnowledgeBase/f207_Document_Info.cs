using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.DocumentView;
using DevExpress.Security;
using DevExpress.Utils.About;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Win.Hook;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
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

        List<dt207_Type> lsKnowledgeTypes = new List<dt207_Type>();
        List<User> lsUsers = new List<User>();
        List<Group> lsGroups = new List<Group>();
        List<Securityinfo> lsSecurityInfos = new List<Securityinfo>();
        List<Attachments> lsAttachments = new List<Attachments>();
        List<Securityinfo> lsIdGroupOrUser = new List<Securityinfo>();
        List<GroupUser> lsGroupUser = new List<GroupUser>();

        Securityinfo permissionAttachments = new Securityinfo();
        dm_Progress progressSelect = new dm_Progress();

        int finishStep = -1;
        string events = string.Empty;

        private class Attachments
        {
            public string FileName { get; set; }
            public string EncryptionName { get; set; }
            public string FullPath { get; set; }
        }

        private class Securityinfo : dt207_Security
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
                bool isExistsId = db.dt207_Base.Any(kb => kb.Id == tempId);
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
                    DeleteInfo = true,
                    UpdateInfo = true,
                    ReadFile = true,
                    SaveFile = true,
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
                        DeleteInfo = securitys.DeleteInfo,
                        UpdateInfo = securitys.UpdateInfo,
                        ReadFile = securitys.ReadFile,
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
                lsKnowledgeTypes = db.dt207_Type.ToList();
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
                    var dataView = db.dt207_Base.FirstOrDefault(r => r.Id == idDocument);
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
                    lsAttachments.AddRange(db.dt207_Attachment.Where(r => r.IdKnowledgeBase == idDocument)
                        .Select(item => new Attachments { FileName = item.FileName, EncryptionName = item.EncryptionName }));

                    sourceAttachments.DataSource = lsAttachments;
                    lbCountFile.Text = $"共{lsAttachments.Count}個附件";

                    // Retrieve knowledge security information from database and assign to sourceSecuritys
                    helper.SaveViewInfo();
                    var lsKnowledgeSecuritys = db.dt207_Security.Where(r => r.IdKnowledgeBase == idDocument).ToList();
                    lsSecurityInfos = lsKnowledgeSecuritys.Select(data => new Securityinfo
                    {
                        IdKnowledgeBase = data.IdKnowledgeBase,
                        IdGroup = data.IdGroup,
                        IdUser = data.IdUser,
                        ReadInfo = data.ReadInfo,
                        UpdateInfo = data.UpdateInfo,
                        DeleteInfo = data.DeleteInfo,
                        SearchInfo = data.SearchInfo,
                        ReadFile = data.ReadFile,
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
                        var lsDocProgressInfos = db.dt207_DocProgressInfo.Where(r => r.IdDocProgress == idDocProgress).ToList();

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

            var dialog = XtraMessageBox.Show($"您确定要删除该文件：{idDocument}", TempDatas.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialog == DialogResult.No) return;

            uc207_SelectProgress ucInfo = new uc207_SelectProgress();
            if (XtraDialog.Show(ucInfo, "清選流程核簽", MessageBoxButtons.OKCancel) != DialogResult.OK) return;

            progressSelect = ucInfo.ProgressSelect;

            using (var db = new DBDocumentManagementSystemEntities())
            {
                dt207_DocProgress docProgress = new dt207_DocProgress()
                {
                    IdKnowledgeBase = idDocument,
                    IsComplete = false,
                    IsSuccess = false,
                    IdProgress = progressSelect.Id,
                    Descriptions = TempDatas.EventDel,
                };

                db.dt207_DocProgress.Add(docProgress);
                // Save the changes to the database
                db.SaveChanges();

                var lsDocProgressById = db.dt207_DocProgress.Where(r => r.IdKnowledgeBase == idDocument).ToList();
                int idDocProcess = lsDocProgressById.OrderByDescending(r => r.Id).FirstOrDefault().Id;

                dt207_DocProgressInfo progressInfo = new dt207_DocProgressInfo()
                {
                    IdDocProgress = idDocProcess,
                    TimeStep = DateTime.Now,
                    IndexStep = 0,
                    IdUserProcess = TempDatas.LoginId,
                    Descriptions = TempDatas.EventDel,
                };

                db.dt207_DocProgressInfo.Add(progressInfo);

                // Save the changes to the database
                db.SaveChanges();
            }

            Close();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Đưa focused ra khỏi bảng để cập nhật lên source
            bgvSecurity.FocusedRowHandle = GridControl.AutoFilterRowHandle;

            // Tạo ra IdDoc nếu là chức năng thêm mới
            events = TempDatas.EventEdit;
            if (string.IsNullOrEmpty(idDocument))
            {
                events = TempDatas.EventNew;
                idDocument = GenerateIdDocument();
            }

            using (var db = new DBDocumentManagementSystemEntities())
            {
                using (var handle = SplashScreenManager.ShowOverlayForm(this))
                {
                    // Nếu là Edit Doc thì lưu lại thông tin cũ để nếu bị trả về thì update lại dữ liệu cũ
                    if (events == TempDatas.EventEdit)
                    {
                        dt207_Base_BAK base_BAK = db.dt207_Base.
                            Where(r => r.Id == idDocument).ToList().
                            Select(r => new dt207_Base_BAK()
                            {
                                Id = r.Id,
                                DisplayName = r.DisplayName,
                                UserRequest = r.UserRequest,
                                IdTypes = r.IdTypes,
                                Keyword = r.Keyword,
                                UserUpload = r.UserUpload,
                                UploadDate = r.UploadDate
                            }).FirstOrDefault();
                        db.dt207_Base_BAK.Add(base_BAK);

                        var lsAttachments_BAK = db.dt207_Attachment.
                            Where(r => r.IdKnowledgeBase == idDocument).ToList().
                            Select(r => new dt207_Attachment_BAK()
                            {
                                IdKnowledgeBase = r.IdKnowledgeBase,
                                EncryptionName = r.EncryptionName,
                                FileName = r.FileName
                            });
                        db.dt207_Attachment_BAK.AddRange(lsAttachments_BAK);

                        var lsSecurities_BAK = db.dt207_Security.
                            Where(r => r.IdKnowledgeBase == idDocument).ToList().
                            Select(r => new dt207_Security_BAK()
                            {
                                IdKnowledgeBase = r.IdKnowledgeBase,
                                IdGroup = r.IdGroup,
                                IdUser = r.IdUser,
                                ReadInfo = r.ReadInfo,
                                UpdateInfo = r.UpdateInfo,
                                DeleteInfo = r.DeleteInfo,
                                SearchInfo = r.SearchInfo,
                                ReadFile = r.ReadFile,
                                SaveFile = r.SaveFile
                            });
                        db.dt207_Security_BAK.AddRange(lsSecurities_BAK);
                    }

                    // Tạo ra DocBase từ các thông tin trên form, thêm vào DB
                    dt207_Base knowledge = new dt207_Base()
                    {
                        Id = idDocument,
                        DisplayName = $"{convertToUnSign3(txbNameTW.Text.Trim())}\r\n{txbNameVN.Text.Trim()}",
                        UserRequest = cbbUserRequest.EditValue.ToString(),
                        IdTypes = (int)cbbType.EditValue,
                        Keyword = txbKeyword.Text.Trim(),
                        UserUpload = cbbUserUpload.EditValue.ToString(),
                        UploadDate = DateTime.Now
                    };
                    db.dt207_Base.AddOrUpdate(knowledge);

                    // Xoá tất cả các phụ kiện trước đó để thêm lại (Không xoá file cứng, định kỳ làm sạch file cứng nếu không dùng đến)
                    db.dt207_Attachment.RemoveRange(db.dt207_Attachment.Where(r => r.IdKnowledgeBase == idDocument));
                    foreach (Attachments item in lsAttachments)
                    {
                        dt207_Attachment attachment = new dt207_Attachment()
                        {
                            IdKnowledgeBase = idDocument,
                            EncryptionName = item.EncryptionName,
                            FileName = item.FileName
                        };
                        db.dt207_Attachment.AddOrUpdate(attachment);

                        // Thêm các file cứng vào Server nếu là file mới
                        if (!string.IsNullOrEmpty(item.FullPath))
                        {
                            string sourceFileName = item.FullPath;
                            string destFileName = Path.Combine(TempDatas.PathKnowledgeFile, item.EncryptionName);

                            File.Copy(sourceFileName, destFileName, true);
                        }
                    }

                    // Xoá tất cả các dữ liệu quyền hạn để thêm lại dựa vào dữ liệu trên form
                    db.dt207_Security.RemoveRange(db.dt207_Security.Where(r => r.IdKnowledgeBase == idDocument));
                    foreach (var item in lsSecurityInfos)
                    {
                        dt207_Security dataAdd = new dt207_Security
                        {
                            Id = item.Id,
                            IdKnowledgeBase = idDocument,
                            IdGroup = item.IdGroupOrUser.StartsWith("VNW") ? null : (short?)Convert.ToInt16(item.IdGroupOrUser),
                            IdUser = item.IdGroupOrUser.StartsWith("VNW") ? item.IdGroupOrUser : null,
                            ReadInfo = item.ReadInfo,
                            UpdateInfo = item.UpdateInfo,
                            DeleteInfo = item.DeleteInfo,
                            SearchInfo = item.SearchInfo,
                            ReadFile = item.ReadFile,
                            SaveFile = item.SaveFile
                        };
                        db.dt207_Security.Add(dataAdd);
                    }

                    // Nếu chưa có lưu trình xử lý của văn kiện thì thêm mới (Trigger sẽ tự thêm vào ProcessInfo)
                    bool IsNewDocProcess = !db.dt207_DocProgress.Any(r => !r.IsComplete && r.IdKnowledgeBase == idDocument);
                    if (IsNewDocProcess)
                    {
                        dt207_DocProgress docProgress = new dt207_DocProgress()
                        {
                            IdKnowledgeBase = idDocument,
                            IsComplete = false,
                            IsSuccess = false,
                            IdProgress = progressSelect.Id,
                            Descriptions = events,
                            IdUserProcess = TempDatas.LoginId,
                        };

                        db.dt207_DocProgress.Add(docProgress);
                    }

                    // Save the changes to the database
                    db.SaveChanges();
                }
            }

            // Show a message box with the appropriate message and close the form
            XtraMessageBox.Show($"{events}!\r\n文件編號:{idDocument}", TempDatas.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }

        private void btnAddPermission_Click(object sender, EventArgs e)
        {
            lsSecurityInfos.Add(new Securityinfo()
            {
                IdKnowledgeBase = idDocument,
                ReadInfo = true,
                UpdateInfo = false,
                DeleteInfo = false,
                SearchInfo = true,
                ReadFile = false,
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
            string documentsFile = Path.Combine(TempDatas.PathKnowledgeFile, dataRow.EncryptionName);

            // Lưu lại lịch sử xem file
            if (!string.IsNullOrEmpty(idDocument))
            {
                using (var db = new DBDocumentManagementSystemEntities())
                {
                    dt207_HistoryGetFile historyGetFile = new dt207_HistoryGetFile()
                    {
                        IdKnowledgeBase = idDocument,
                        idTypeHisGetFile = 1,
                        KnowledgeAttachmentName = dataRow.FileName,
                        IdUser = TempDatas.LoginId,
                        TimeGet = DateTime.Now
                    };

                    db.dt207_HistoryGetFile.Add(historyGetFile);
                    db.SaveChanges();
                }
            }

            f207_ViewPdf fDocumentInfo = new f207_ViewPdf(documentsFile, idDocument);
            fDocumentInfo.Text = dataRow.FileName;
            fDocumentInfo.CanSaveFile = permissionAttachments.SaveFile;
            fDocumentInfo.ShowDialog();
        }

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (permissionAttachments.UpdateInfo != true)
            {
                XtraMessageBox.Show(TempDatas.NoPermission, TempDatas.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            LockControl(false);
        }

        private void btnChangeProgress_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            uc207_SelectProgress ucInfo = new uc207_SelectProgress();
            if (XtraDialog.Show(ucInfo, "修改流程", MessageBoxButtons.OKCancel) != DialogResult.OK) return;

            progressSelect = ucInfo.ProgressSelect;
            lbProgress.Text = "流程：" + progressSelect.DisplayName;
        }

        private void btnApproved_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (var db = new DBDocumentManagementSystemEntities())
            {
                var lsDocProgressById = db.dt207_DocProgress.Where(r => r.IdKnowledgeBase == idDocument).ToList();
                int idDocProgress = lsDocProgressById.OrderByDescending(r => r.Id).FirstOrDefault().Id;
                string eventProcess = lsDocProgressById.OrderByDescending(r => r.Id).FirstOrDefault().Descriptions;

                List<dt207_DocProgressInfo> lsDocProgressInfos = db.dt207_DocProgressInfo.Where(r => r.IdDocProgress == idDocProgress).ToList();
                int indexStep = lsDocProgressInfos.Count != 0 ? lsDocProgressInfos.OrderByDescending(r => r.Id).FirstOrDefault().IndexStep + 1 : 0;

                string descriptions = "核准";
                if (indexStep == finishStep)
                {
                    var docProcessUpdate = db.dt207_DocProgress.Where(r => r.Id == idDocProgress).First();
                    docProcessUpdate.IsSuccess = true;
                    docProcessUpdate.IsComplete = true;

                    db.dt207_DocProgress.AddOrUpdate(docProcessUpdate);

                    descriptions = "確認完畢";
                }

                dt207_DocProgressInfo progressInfo = new dt207_DocProgressInfo()
                {
                    IdDocProgress = idDocProgress,
                    TimeStep = DateTime.Now,
                    IndexStep = indexStep,
                    IdUserProcess = TempDatas.LoginId,
                    Descriptions = descriptions,
                };
                db.dt207_DocProgressInfo.Add(progressInfo);

                // Nếu mà Xoá thì trường IsDelete = true/ Sửa thì xoá các dữ liệu cũ đi
                switch (eventProcess)
                {
                    case TempDatas.EventDel:
                        var docBaseDelete = db.dt207_Base.First(r => r.Id == idDocument);
                        docBaseDelete.IsDelete = true;

                        db.dt207_Base.AddOrUpdate(docBaseDelete);
                        break;
                    case TempDatas.EventEdit:
                        db.dt207_Base_BAK.RemoveRange(db.dt207_Base_BAK.Where(r => r.Id == idDocument));
                        db.dt207_Attachment_BAK.RemoveRange(db.dt207_Attachment_BAK.Where(r => r.IdKnowledgeBase == idDocument));
                        db.dt207_Security_BAK.RemoveRange(db.dt207_Security_BAK.Where(r => r.IdKnowledgeBase == idDocument));
                        break;
                }

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

                dt207_DocProgressInfo progressInfo = new dt207_DocProgressInfo()
                {
                    IdDocProgress = idDocProgress,
                    TimeStep = DateTime.Now,
                    IndexStep = -1,
                    IdUserProcess = TempDatas.LoginId,
                    Descriptions = string.IsNullOrEmpty(descriptions) ? "退回" : $"退回，說明：{descriptions}",
                };

                db.dt207_DocProgressInfo.Add(progressInfo);

                // Save the changes to the database
                db.SaveChanges();
            }

            Close();
        }

        private void btnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Tạo textbox lấy nguyên nhân huỷ sửa văn kiện
            XtraInputBoxArgs args = new XtraInputBoxArgs();
            args.Caption = TempDatas.SoftNameTW;
            args.Prompt = "原因";
            args.DefaultButtonIndex = 0;
            MemoEdit editor = new MemoEdit();
            args.Editor = editor;
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

                dt207_DocProgressInfo progressInfo = new dt207_DocProgressInfo()
                {
                    IdDocProgress = idDocProgress,
                    TimeStep = DateTime.Now,
                    IndexStep = -1,
                    IdUserProcess = TempDatas.LoginId,
                    Descriptions = string.IsNullOrEmpty(descriptions) ? "取消" : $"取消，說明：{descriptions}",
                };
                db.dt207_DocProgressInfo.Add(progressInfo);

                // Cập nhật lại các dữ liệu cũ trước khi sửa
                dt207_Base baseDoc = db.dt207_Base_BAK.
                            Where(r => r.Id == idDocument).ToList().
                            Select(r => new dt207_Base()
                            {
                                Id = r.Id,
                                DisplayName = r.DisplayName,
                                UserRequest = r.UserRequest,
                                IdTypes = r.IdTypes,
                                Keyword = r.Keyword,
                                UserUpload = r.UserUpload,
                                UploadDate = r.UploadDate
                            }).FirstOrDefault();
                db.dt207_Base.AddOrUpdate(baseDoc);

                // Xoá các tệp phụ kiện và thêm lại phụ kiện cũ
                db.dt207_Attachment.RemoveRange(db.dt207_Attachment.Where(r => r.IdKnowledgeBase == idDocument));
                var lsAttachments = db.dt207_Attachment.Where(r => r.IdKnowledgeBase == idDocument).ToList().
                    Select(r => new dt207_Attachment()
                    {
                        IdKnowledgeBase = r.IdKnowledgeBase,
                        EncryptionName = r.EncryptionName,
                        FileName = r.FileName
                    });
                db.dt207_Attachment.AddRange(lsAttachments);

                // Xoá các quyền hạn mới và thêm lại quyền hạn cũ
                db.dt207_Security.RemoveRange(db.dt207_Security.Where(r => r.IdKnowledgeBase == idDocument));
                var lsSecurities = db.dt207_Security.Where(r => r.IdKnowledgeBase == idDocument).ToList().
                    Select(r => new dt207_Security()
                    {
                        IdKnowledgeBase = r.IdKnowledgeBase,
                        IdGroup = r.IdGroup,
                        IdUser = r.IdUser,
                        ReadInfo = r.ReadInfo,
                        UpdateInfo = r.UpdateInfo,
                        DeleteInfo = r.DeleteInfo,
                        SearchInfo = r.SearchInfo,
                        ReadFile = r.ReadFile,
                        SaveFile = r.SaveFile
                    });
                db.dt207_Security.AddRange(lsSecurities);

                // Xoá các dữ liệu cũ BAK
                db.dt207_Base_BAK.RemoveRange(db.dt207_Base_BAK.Where(r => r.Id == idDocument));
                db.dt207_Attachment_BAK.RemoveRange(db.dt207_Attachment_BAK.Where(r => r.IdKnowledgeBase == idDocument));
                db.dt207_Security_BAK.RemoveRange(db.dt207_Security_BAK.Where(r => r.IdKnowledgeBase == idDocument));

                // Save the changes to the database
                db.SaveChanges();
            }

            Close();
        }
    }
}