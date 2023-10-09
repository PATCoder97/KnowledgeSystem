using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraPrinting.Native;
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
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Microsoft.IO.RecyclableMemoryStreamManager;
using dm_Group = DataAccessLayer.dm_Group;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._07_KnowledgeBase
{
    public partial class f207_Document_ViewOnly : DevExpress.XtraEditors.XtraForm
    {
        public f207_Document_ViewOnly()
        {
            InitializeComponent();
            LockControl(false);
        }

        public f207_Document_ViewOnly(dt207_DocProcessing docProcess_)
        {
            InitializeComponent();
            docProcess = docProcess_;
            idDocument = docProcess_.IdKnowledgeBase;
            LockControl(true);
        }

        #region parameters

        dm_UserBUS _dm_UserBUS = new dm_UserBUS();
        dm_GroupBUS _dm_GroupBUS = new dm_GroupBUS();
        dm_GroupUserBUS _dm_GroupUserBUS = new dm_GroupUserBUS();

        BindingSource sourceAttachments = new BindingSource();
        BindingSource sourceSecuritys = new BindingSource();

        string idDocument = string.Empty;
        dt207_DocProcessing docProcess = new dt207_DocProcessing();

        List<dt207_Type> lsKnowledgeTypes = new List<dt207_Type>();
        List<dm_User> lsUsers = new List<dm_User>();
        List<dm_Group> lsGroups = new List<dm_Group>();
        List<Securityinfo> lsSecurityInfos = new List<Securityinfo>();
        List<Attachments> lsAttachments = new List<Attachments>();
        List<Securityinfo> lsIdGroupOrUser = new List<Securityinfo>();
        List<dm_GroupUser> lsGroupUser = new List<dm_GroupUser>();

        Securityinfo permissionAttachments = new Securityinfo();

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

        private void LockControl(bool isFormView = true)
        {
            cbbType.ReadOnly = isFormView;
            txbNameTW.ReadOnly = isFormView;
            txbNameVN.ReadOnly = isFormView;
            cbbUserUpload.ReadOnly = isFormView;
            cbbUserProcess.ReadOnly = isFormView;
            txbKeyword.ReadOnly = isFormView;

            gvFiles.OptionsBehavior.ReadOnly = isFormView;
            bgvSecurity.OptionsBehavior.ReadOnly = isFormView;

            Text = "群組信息";
        }

        private Securityinfo GetPermission()
        {
            string userIdLogin = TPConfigs.LoginUser.Id;

            if (cbbUserProcess.EditValue.ToString() == userIdLogin)
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

        private void f207_Document_ViewOnly_Load(object sender, EventArgs e)
        {
            controlgroupDocument.SelectedTabPage = groupProgress;

            // Cài các gridview readonly và set datasource
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

                // Truy xuất dữ liệu từ cơ sở dữ liệu và gán cho các thành phần form
                if (!string.IsNullOrEmpty(idDocument))
                {
                    // Thông tin tiến trình trình ký văn kiện
                    bool IsComplete = docProcess.IsComplete;
                    int idProgressByDoc = docProcess.IdProgress;
                    int idDocProgress = docProcess.Id;
                    var lsDMStepProgress = db.dm_StepProgress.Where(r => r.IdProgress == idProgressByDoc).ToList();
                    var lsDocProgressInfos = db.dt207_DocProcessingInfo.Where(r => r.IdDocProgress == idDocProgress).ToList();

                    var lsStepProgressDoc = (from data in lsDMStepProgress
                                             join groups in lsGroups on data.IdGroup equals groups.Id
                                             select new { groups.DisplayName }).ToList();

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

                    if (IsComplete)
                    {
                        lcgInfo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        lcgFile.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        lcgPermission.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }
                    else
                    {
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

                        permissionAttachments = GetPermission();
                    }
                }
            }
        }

        private void gcFiles_DoubleClick(object sender, EventArgs e)
        {
            if (permissionAttachments.ReadFile != true)
            {
                XtraMessageBox.Show(TPConfigs.NoPermission, TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            int focusRow = gvFiles.FocusedRowHandle;
            if (focusRow < 0)
                return;

            Attachments dataRow = gvFiles.GetRow(focusRow) as Attachments;
            string documentsFile = Path.Combine(TPConfigs.PathKnowledgeFile, dataRow.EncryptionName);

            string pathDocTemp = Path.Combine(TPConfigs.TempFolderData, $"{DateTime.Now:MMddhhmmss} {dataRow.FileName}");
            using (var handle = SplashScreenManager.ShowOverlayForm(gcFiles))
            {
                // Copy file về thư mục tạm để xem
                if (!Directory.Exists(TPConfigs.TempFolderData))
                    Directory.CreateDirectory(TPConfigs.TempFolderData);

                File.Copy(documentsFile, pathDocTemp, true);

                f207_ViewFile fDocumentInfo = new f207_ViewFile(pathDocTemp, "")
                {
                    Text = dataRow.FileName,
                    CanSaveFile = permissionAttachments.SaveFile
                };
                fDocumentInfo.ShowDialog();
            }
        }
    }
}