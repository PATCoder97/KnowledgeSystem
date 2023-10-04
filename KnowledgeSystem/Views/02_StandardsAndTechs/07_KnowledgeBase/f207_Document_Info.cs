using BusinessLayer;
using DataAccessLayer;
using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.DocumentView;
using DevExpress.Security;
using DevExpress.Utils.About;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Extensions;
using DevExpress.Utils.Win.Hook;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting.Native.WebClientUIControl;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Configs;
using Newtonsoft.Json;
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
using dm_Group = DataAccessLayer.dm_Group;

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

        dm_UserBUS _dm_UserBUS = new dm_UserBUS();
        dm_GroupBUS _dm_GroupBUS = new dm_GroupBUS();
        dm_GroupUserBUS _dm_GroupUserBUS = new dm_GroupUserBUS();

        BindingSource sourceAttachments = new BindingSource();
        BindingSource sourceSecuritys = new BindingSource();

        string idDocument = string.Empty;
        string userId = string.Empty;

        List<dt207_Type> lsKnowledgeTypes = new List<dt207_Type>();
        List<dm_User> lsUsers = new List<dm_User>();
        List<dm_Group> lsGroups = new List<dm_Group>();
        List<Securityinfo> lsSecurityInfos = new List<Securityinfo>();
        List<Attachments> lsAttachments = new List<Attachments>();
        List<Securityinfo> lsIdGroupOrUser = new List<Securityinfo>();
        List<dm_GroupUser> lsGroupUser = new List<dm_GroupUser>();

        Securityinfo permissionAttachments = new Securityinfo();
        dm_Progress progressSelect = new dm_Progress();

        int idDocProgress = -1;
        int stepEnd = -1;
        string events = string.Empty;

        private class Attachments
        {
            public string FileName { get; set; }
            public string EncryptionName { get; set; }
            public string FullPath { get; set; }
        }

        private class Securityinfo : dt207_Security
        {
            public string IdDept { get; set; }
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
            bool readOnly = isFormView;
            bool enabled = !isFormView;

            cbbType.ReadOnly = readOnly;
            txbNameTW.ReadOnly = readOnly;
            txbNameVN.ReadOnly = readOnly;
            cbbUserProcess.ReadOnly = true;
            txbKeyword.ReadOnly = readOnly;
            cbbUserUpload.ReadOnly = readOnly;
            if (!string.IsNullOrEmpty(idDocument))
            {
                cbbUserUpload.ReadOnly = true;
            }

            btnAddFile.Enabled = enabled;
            btnAddPermission.Enabled = enabled;

            gvFiles.OptionsBehavior.ReadOnly = readOnly;
            bgvSecurity.OptionsBehavior.ReadOnly = readOnly;

            gColDelFile.Visible = !readOnly;
            gColDelPermission.Visible = !readOnly;

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

            if (cbbUserUpload.EditValue.ToString() == userIdLogin ||
                cbbUserProcess.EditValue.ToString() == userIdLogin)
            {
                return new Securityinfo
                {
                    DeleteInfo = true,
                    UpdateInfo = true,
                    ReadFile = true,
                    SaveFile = true,
                };
            }

            var userSecurity = lsSecurityInfos.FirstOrDefault(r => r.IdUser == userIdLogin);
            if (userSecurity != null)
            {
                return userSecurity;
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
            lcgProgress.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            lcgHistoryEdit.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            controlgroupDocument.SelectedTabPage = lcgInfo;

            // Initialize RefreshHelper
            helper = new RefreshHelper(bgvSecurity, "Id");

            // Cài các gridview readonly và set datasource
            gvEditHistory.ReadOnlyGridView();
            gvFiles.ReadOnlyGridView();
            gvHistoryProcess.ReadOnlyGridView();
            gcFiles.DataSource = sourceAttachments;
            gcSecurity.DataSource = sourceSecuritys;

            using (var db = new DBDocumentManagementSystemEntities())
            {
                // Initialize lists
                lsKnowledgeTypes = db.dt207_Type.ToList();
                lsUsers = _dm_UserBUS.GetList();
                lsGroups = _dm_GroupBUS.GetList();
                lsGroupUser = _dm_GroupUserBUS.GetList();
                progressSelect = db.dm_Progress.FirstOrDefault();

                // Create lists of Securityinfo objects from lsUsers and lsGroups
                var lsIdUsers = lsUsers.Select(r => new Securityinfo { IdDept = r.IdDepartment, IdGroupOrUser = r.Id, DisplayName = r.DisplayName }).ToList();
                var lsIdGroup = lsGroups.Select(r => new Securityinfo { IdGroupOrUser = r.Id.ToString(), DisplayName = r.DisplayName }).ToList();
                lsIdGroupOrUser = lsIdGroup.Concat(lsIdUsers).ToList();

                // Assign data source and columns to rgvGruopOrUser
                rgvGruopOrUser.DataSource = lsIdGroupOrUser;
                rgvGruopOrUser.ValueMember = "IdGroupOrUser";
                rgvGruopOrUser.DisplayMember = "DisplayName";
                rgvGruopOrUser.PopupView.Columns.AddRange(new[]
                {
                    new GridColumn { FieldName = "IdDept", VisibleIndex = 0, Caption = "部門" },
                    new GridColumn { FieldName = "IdGroupOrUser", VisibleIndex = 1, Caption = "代號" },
                    new GridColumn { FieldName = "DisplayName", VisibleIndex = 2, Caption = "名稱" }
                });

                // Load các datasource vào ComboBoxes
                cbbType.Properties.DataSource = lsKnowledgeTypes;
                cbbType.Properties.ValueMember = "Id";
                cbbType.Properties.DisplayMember = "DisplayName";

                cbbUserUpload.Properties.DataSource = lsUsers;
                cbbUserUpload.Properties.ValueMember = "Id";
                cbbUserUpload.Properties.DisplayMember = "DisplayName";

                cbbUserProcess.Properties.DataSource = lsUsers;
                cbbUserProcess.Properties.ValueMember = "Id";
                cbbUserProcess.Properties.DisplayMember = "DisplayName";

                // Cài các thông tin mặc định lên Form (Trường hợp new Doc)
                userId = TempDatas.LoginId;
                cbbType.EditValue = 1;
                cbbUserUpload.EditValue = userId;
                cbbUserProcess.EditValue = userId;
                var userLogin = lsUsers.FirstOrDefault(r => r.Id == userId);
                txbId.Text = "XXX-XXXXXXXXXX-XX";
                lbCountFile.Text = "";
                lbProgress.Text = "流程：" + progressSelect.DisplayName;

                // Nếu có idDocument, truy xuất dữ liệu từ cơ sở dữ liệu và gán cho các thành phần form
                if (!string.IsNullOrEmpty(idDocument))
                {
                    lcgHistoryEdit.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                    // Thông tin cơ bản
                    var dataBaseInfo = db.dt207_Base.FirstOrDefault(r => r.Id == idDocument);
                    txbId.Text = dataBaseInfo.Id;
                    cbbType.EditValue = dataBaseInfo.IdTypes;
                    cbbUserUpload.EditValue = dataBaseInfo.UserUpload;
                    cbbUserProcess.EditValue = dataBaseInfo.UserProcess;
                    var displayName = dataBaseInfo.DisplayName.Split(new[] { "\r\n" }, StringSplitOptions.None);
                    txbNameTW.Text = displayName[0];
                    txbNameVN.Text = displayName.Length > 1 ? displayName[1] : "";
                    txbKeyword.Text = dataBaseInfo.Keyword;

                    // Thông tin phụ kiện
                    lsAttachments.AddRange(db.dt207_Attachment.Where(r => r.IdKnowledgeBase == idDocument)
                        .Select(item => new Attachments { FileName = item.FileName, EncryptionName = item.EncryptionName }));

                    sourceAttachments.DataSource = lsAttachments;
                    lbCountFile.Text = $"共{lsAttachments.Count}個附件";

                    // Thông tin quyền hạn
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
                        IdGroupOrUser = data.IdUser ?? data.IdGroup.ToString()
                    }).ToList();
                    sourceSecuritys.DataSource = lsSecurityInfos;
                    bgvSecurity.BestFitColumns();
                    helper.LoadViewInfo();

                    // Thông tin lịch sửa cập nhật
                    var lsHisUpdateRaw = db.dt207_DocProgress.Where(r => r.IdKnowledgeBase == idDocument && r.Descriptions == TempDatas.EventEdit).ToList();
                    var DocProgressInfos =
                        (from data in db.dt207_DocProgressInfo
                         group data by data.IdDocProgress into g
                         select new
                         {
                             IdDocProgress = g.Key,
                             IndexStep = g.OrderByDescending(dpi => dpi.TimeStep).Select(dpi => dpi.IndexStep).FirstOrDefault(),
                             TimeStep = g.OrderByDescending(dpi => dpi.TimeStep).Select(dpi => dpi.TimeStep).FirstOrDefault(),
                             IdUserProcess = g.OrderBy(dpi => dpi.TimeStep).Select(dpi => dpi.IdUserProcess).FirstOrDefault()
                         }).ToList();

                    var lsHisUpdates = (from data in lsHisUpdateRaw
                                        join infos in DocProgressInfos on data.Id equals infos.IdDocProgress
                                        join users in lsUsers on data.IdUserProcess equals users.Id
                                        select new
                                        {
                                            data.IdKnowledgeBase,
                                            data.Change,
                                            infos.TimeStep,
                                            UserProcess = $"{users.IdDepartment} | {infos.IdUserProcess}/{users.DisplayName}",
                                        }).OrderByDescending(r => r.TimeStep).ToList();

                    gcEditHistory.DataSource = lsHisUpdates;

                    // Thông tin tiến trình trình ký văn kiện
                    var lsDocProcessing = db.dt207_DocProgress.Where(r => !r.IsComplete  && r.IdKnowledgeBase == idDocument).ToList();
                    if (lsDocProcessing.Count != 0)
                    {
                        lcgProgress.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        controlgroupDocument.SelectedTabPage = lcgProgress;

                        int idProgressByDoc = lsDocProcessing.First().IdProgress;
                        idDocProgress = lsDocProcessing.First().Id;
                        events = lsDocProcessing.First().Descriptions;
                        var lsDMStepProgress = db.dm_StepProgress.Where(r => r.IdProgress == idProgressByDoc).ToList();
                        var lsDocProgressInfos = db.dt207_DocProgressInfo.Where(r => r.IdDocProgress == idDocProgress).ToList();

                        stepEnd = lsDMStepProgress.Max(r => r.IndexStep);

                        var lsStepProgressDoc = (from data in lsDMStepProgress
                                                 join groups in lsGroups on data.IdGroup equals groups.Id
                                                 select new { groups.DisplayName }).ToList();

                        // Thêm danh sách các bước vào StepProgressBar
                        stepProgressDoc.Items.Add(new StepProgressBarItem("經辦人"));
                        foreach (var item in lsStepProgressDoc)
                            stepProgressDoc.Items.Add(new StepProgressBarItem(item.DisplayName));

                        var stepNow = lsDocProgressInfos.OrderByDescending(r => r.TimeStep).First().IndexStep;

                        stepProgressDoc.ItemOptions.Indicator.Width = 40;
                        stepProgressDoc.SelectedItemIndex = stepNow;

                        // Thêm lịch sử trình ký vào gridProcess
                        var lsHistoryProcess = (from data in lsDocProgressInfos
                                                join users in db.dm_User on data.IdUserProcess equals users.Id
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

            // Lấy quyền hạn của người dùng đối với văn kiện này
            permissionAttachments = GetPermission() ?? new Securityinfo
            {
                DeleteInfo = false,
                UpdateInfo = false,
                ReadFile = false,
                SaveFile = false
            };
        }

        private void btnAddFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            foreach (string fileName in openFileDialog.FileNames)
            {
                string encryptionName = GenerateEncryptionName();
                Attachments attachment = new Attachments
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

            var dialogResult = XtraMessageBox.Show($"您确定要删除该文件：{idDocument}", TempDatas.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.No)
            {
                return;
            }

            uc207_SelectProgress ucInfo = new uc207_SelectProgress();
            if (XtraDialog.Show(ucInfo, "清選流程核簽", MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                return;
            }

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
                    IdUserProcess = TempDatas.LoginId
                };

                db.dt207_DocProgress.Add(docProgress);
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
                    // Check xem văn kiện có đang trong quá trình trình ký không
                    bool IsProcessing = db.dt207_DocProgress.Any(r => r.IdKnowledgeBase == idDocument && !(r.IsComplete));

                    // Nếu là Edit Doc thì lưu lại thông tin cũ để nếu bị trả về thì update lại dữ liệu cũ
                    if (events == TempDatas.EventEdit && !IsProcessing)
                    {
                        dt207_Base_BAK base_BAK = db.dt207_Base.
                            Where(r => r.Id == idDocument).ToList().
                            Select(r => new dt207_Base_BAK()
                            {
                                Id = r.Id,
                                DisplayName = r.DisplayName,
                                UserProcess = r.UserProcess,
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
                        UserProcess = cbbUserProcess.EditValue.ToString(),
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

                    //// Nếu chưa có lưu trình xử lý của văn kiện thì thêm mới (Trigger sẽ tự thêm vào ProcessInfo)
                    //bool IsNewDocProcess = !db.dt207_DocProgress.Any(r => !r.IsComplete && r.IdKnowledgeBase == idDocument);
                    if (!IsProcessing)
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
                    else
                    {
                        dt207_DocProgressInfo progressInfo = new dt207_DocProgressInfo()
                        {
                            IdDocProgress = idDocProgress,
                            TimeStep = DateTime.Now,
                            IndexStep = 0,
                            IdUserProcess = TempDatas.LoginId,
                            Descriptions = "呈核",
                        };
                        db.dt207_DocProgressInfo.Add(progressInfo);
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
            if (dialogResult == DialogResult.No)
            {
                return;
            }

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
            if (dialogResult == DialogResult.No)
                return;

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

            int focusRow = gvFiles.FocusedRowHandle;
            if (focusRow < 0)
                return;

            Attachments dataRow = gvFiles.GetRow(focusRow) as Attachments;
            string documentsFile = Path.Combine(TempDatas.PathKnowledgeFile, dataRow.EncryptionName);

            // Lưu lại lịch sử xem file
            using (var db = new DBDocumentManagementSystemEntities())
            {
                var IsProcessing = db.dt207_DocProgress.Any(r => r.IdKnowledgeBase == idDocument && !(r.IsComplete));

                if (!string.IsNullOrEmpty(idDocument) && !IsProcessing)
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

            f207_ViewPdf fDocumentInfo = new f207_ViewPdf(documentsFile, idDocument)
            {
                Text = dataRow.FileName,
                CanSaveFile = permissionAttachments.SaveFile
            };
            fDocumentInfo.ShowDialog();
        }

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (permissionAttachments.UpdateInfo != true)
            {
                XtraMessageBox.Show(TempDatas.NoPermission, TempDatas.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            // Chuyển người process sang userLogin
            cbbUserProcess.EditValue = userId;
            controlgroupDocument.SelectedTabPage = lcgInfo;

            LockControl(false);
        }

        private void btnChangeProgress_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            uc207_SelectProgress ucInfo = new uc207_SelectProgress();
            if (XtraDialog.Show(ucInfo, "修改流程", MessageBoxButtons.OKCancel) != DialogResult.OK)
                return;

            progressSelect = ucInfo.ProgressSelect;
            lbProgress.Text = "流程：" + progressSelect.DisplayName;
        }

        private void btnApproved_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (var db = new DBDocumentManagementSystemEntities())
            {
                List<string> lsChangeDetails = new List<string>();

                var lsDocProgressById = db.dt207_DocProgress.Where(r => r.IdKnowledgeBase == idDocument).ToList();
                int idDocProgress = lsDocProgressById.OrderByDescending(r => r.Id).FirstOrDefault().Id;
                string eventProcess = lsDocProgressById.OrderByDescending(r => r.Id).FirstOrDefault().Descriptions;

                List<dt207_DocProgressInfo> lsDocProgressInfos = db.dt207_DocProgressInfo.Where(r => r.IdDocProgress == idDocProgress).ToList();
                int indexStep = lsDocProgressInfos.Count != 0 ? lsDocProgressInfos.OrderByDescending(r => r.Id).FirstOrDefault().IndexStep + 1 : 0;

                // Nếu mà Xoá thì trường IsDelete = true/ Sửa thì xoá các dữ liệu cũ đi
                switch (eventProcess)
                {
                    case TempDatas.EventDel:
                        var docBaseDelete = db.dt207_Base.First(r => r.Id == idDocument);
                        docBaseDelete.IsDelete = true;

                        db.dt207_Base.AddOrUpdate(docBaseDelete);
                        break;
                    case TempDatas.EventEdit:
                        // Kiểm tra xem văn kiện được sửa ở chỗ nào
                        // Kiểm tra thông tin văn kiện
                        var docBaseNew = db.dt207_Base.First(r => r.Id == idDocument);
                        var docBaseOld = db.dt207_Base_BAK.First(r => r.Id == idDocument);

                        if (docBaseNew.DisplayName != docBaseOld.DisplayName) lsChangeDetails.Add("名稱");
                        if (docBaseNew.IdTypes != docBaseOld.IdTypes) lsChangeDetails.Add("類別");
                        if (docBaseNew.Keyword != docBaseOld.Keyword) lsChangeDetails.Add("關鍵字");

                        // Kiểm tra phụ kiện
                        var lsAttachmentNew = db.dt207_Attachment
                            .Where(r => r.IdKnowledgeBase == idDocument)
                            .Select(r => new { r.FileName, r.EncryptionName })
                            .ToList();
                        var lsAttachmentOld = db.dt207_Attachment_BAK
                            .Where(r => r.IdKnowledgeBase == idDocument)
                            .Select(r => new { r.FileName, r.EncryptionName })
                            .ToList();

                        var jsonAttachmentNew = JsonConvert.SerializeObject(lsAttachmentNew);
                        var jsonAttachmentOld = JsonConvert.SerializeObject(lsAttachmentOld);
                        if (jsonAttachmentNew != jsonAttachmentOld)
                            lsChangeDetails.Add("附件");

                        // Kiểm tra quyền hạn
                        var lsSecurityNew = db.dt207_Security
                            .Where(r => r.IdKnowledgeBase == idDocument)
                            .Select(r => new
                            {
                                r.IdGroup,
                                r.IdUser,
                                r.ReadInfo,
                                r.UpdateInfo,
                                r.DeleteInfo,
                                r.SearchInfo,
                                r.ReadFile,
                                r.SaveFile
                            })
                            .ToList();
                        var lsSecurityOld = db.dt207_Security_BAK
                            .Where(r => r.IdKnowledgeBase == idDocument)
                            .Select(r => new
                            {
                                r.IdGroup,
                                r.IdUser,
                                r.ReadInfo,
                                r.UpdateInfo,
                                r.DeleteInfo,
                                r.SearchInfo,
                                r.ReadFile,
                                r.SaveFile
                            })
                            .ToList();

                        var jsonSecurityNew = JsonConvert.SerializeObject(lsSecurityNew);
                        var jsonSecurityOld = JsonConvert.SerializeObject(lsSecurityOld);
                        if (jsonSecurityNew != jsonSecurityOld)
                            lsChangeDetails.Add("權限");

                        // Xoá dữ liệu cũ
                        db.dt207_Base_BAK.RemoveRange(db.dt207_Base_BAK.Where(r => r.Id == idDocument));
                        db.dt207_Attachment_BAK.RemoveRange(db.dt207_Attachment_BAK.Where(r => r.IdKnowledgeBase == idDocument));
                        db.dt207_Security_BAK.RemoveRange(db.dt207_Security_BAK.Where(r => r.IdKnowledgeBase == idDocument));
                        break;
                }

                string descriptions = "核准";
                if (indexStep == stepEnd)
                {
                    var docProcessUpdate = db.dt207_DocProgress.First(r => r.Id == idDocProgress);
                    docProcessUpdate.IsSuccess = true;
                    docProcessUpdate.IsComplete = true;
                    docProcessUpdate.Change = string.Join("，", lsChangeDetails);

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

                // Save the changes to the database
                db.SaveChanges();
            }

            Close();
        }

        private void btnDisapprove_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XtraInputBoxArgs args = new XtraInputBoxArgs
            {
                Caption = TempDatas.SoftNameTW,
                Prompt = "退回文件原因",
                DefaultButtonIndex = 0,
                Editor = new MemoEdit()
            };

            var result = XtraInputBox.Show(args);
            string descriptions = result?.ToString() ?? "";

            using (var db = new DBDocumentManagementSystemEntities())
            {
                if (events == TempDatas.EventDel)
                {
                    var docProcessUpdate = db.dt207_DocProgress.First(r => r.Id == idDocProgress);
                    docProcessUpdate.IsSuccess = false;
                    docProcessUpdate.IsComplete = true;
                    db.dt207_DocProgress.AddOrUpdate(docProcessUpdate);
                }

                dt207_DocProgressInfo progressInfo = new dt207_DocProgressInfo
                {
                    IdDocProgress = idDocProgress,
                    TimeStep = DateTime.Now,
                    IndexStep = -1,
                    IdUserProcess = TempDatas.LoginId,
                    Descriptions = string.IsNullOrEmpty(descriptions) ? "退回" : $"退回，說明：{descriptions}"
                };

                db.dt207_DocProgressInfo.Add(progressInfo);

                // Save the changes to the database
                db.SaveChanges();
            }

            Close();
        }

        private void btnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Tạo textbox lấy nguyên nhân
            XtraInputBoxArgs args = new XtraInputBoxArgs
            {
                Caption = TempDatas.SoftNameTW,
                Prompt = "原因",
                DefaultButtonIndex = 0,
                Editor = new MemoEdit()
            };

            var result = XtraInputBox.Show(args);
            string descriptions = result?.ToString() ?? "";

            using (var db = new DBDocumentManagementSystemEntities())
            {
                var docProcessUpdate = db.dt207_DocProgress.First(r => r.Id == idDocProgress);
                docProcessUpdate.IsSuccess = false;
                docProcessUpdate.IsComplete = true;
                docProcessUpdate.Change = string.IsNullOrEmpty(descriptions) ? "取消" : $"取消，說明：{descriptions}";

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

                switch (events)
                {
                    // Nếu là sửa thì cập nhật lại dữ liệu cũ và xoá BAK
                    case TempDatas.EventEdit:
                        dt207_Base baseDoc = db.dt207_Base_BAK
                            .Where(r => r.Id == idDocument).ToList()
                            .Select(r => new dt207_Base
                            {
                                Id = r.Id,
                                DisplayName = r.DisplayName,
                                UserProcess = r.UserProcess,
                                IdTypes = r.IdTypes,
                                Keyword = r.Keyword,
                                UserUpload = r.UserUpload,
                                UploadDate = r.UploadDate
                            }).FirstOrDefault();
                        db.dt207_Base.AddOrUpdate(baseDoc);

                        db.dt207_Attachment.RemoveRange(db.dt207_Attachment.Where(r => r.IdKnowledgeBase == idDocument));
                        var lsAttachments = db.dt207_Attachment
                            .Where(r => r.IdKnowledgeBase == idDocument).ToList()
                            .Select(r => new dt207_Attachment
                            {
                                IdKnowledgeBase = r.IdKnowledgeBase,
                                EncryptionName = r.EncryptionName,
                                FileName = r.FileName
                            });
                        db.dt207_Attachment.AddRange(lsAttachments);

                        db.dt207_Security.RemoveRange(db.dt207_Security.Where(r => r.IdKnowledgeBase == idDocument));
                        var lsSecurities = db.dt207_Security
                            .Where(r => r.IdKnowledgeBase == idDocument).ToList()
                            .Select(r => new dt207_Security
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

                        db.dt207_Base_BAK.RemoveRange(db.dt207_Base_BAK.Where(r => r.Id == idDocument));
                        db.dt207_Attachment_BAK.RemoveRange(db.dt207_Attachment_BAK.Where(r => r.IdKnowledgeBase == idDocument));
                        db.dt207_Security_BAK.RemoveRange(db.dt207_Security_BAK.Where(r => r.IdKnowledgeBase == idDocument));
                        break;

                    // Nếu là thêm mới mà huỷ thì chuyển IsDelete = true trên Base
                    case TempDatas.EventNew:
                        var docBaseDelete = db.dt207_Base.First(r => r.Id == idDocument);
                        docBaseDelete.IsDelete = true;

                        db.dt207_Base.AddOrUpdate(docBaseDelete);
                        break;
                }

                db.SaveChanges();
            }

            Close();
        }
    }
}