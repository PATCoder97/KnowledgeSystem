using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._07_KnowledgeBase
{
    public partial class f207_Document_ViewOnly : DevExpress.XtraEditors.XtraForm
    {
        RefreshHelper helper;

        public f207_Document_ViewOnly()
        {
            InitializeComponent();
            LockControl(false);
        }

        public f207_Document_ViewOnly(dt207_DocProgress docProcess_)
        {
            InitializeComponent();
            docProcess = docProcess_;

            LockControl(true);
        }

        #region parameters

        BindingSource sourceAttachments = new BindingSource();
        BindingSource sourceSecuritys = new BindingSource();

        string idDocument = string.Empty;
        dt207_DocProgress docProcess = new dt207_DocProgress();
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

        private void LockControl(bool isFormView = true)
        {
            cbbType.ReadOnly = isFormView;
            txbNameTW.ReadOnly = isFormView;
            txbNameVN.ReadOnly = isFormView;
            cbbUserRequest.ReadOnly = isFormView;
            cbbUserUpload.ReadOnly = isFormView;
            txbKeyword.ReadOnly = isFormView;

            gvFiles.OptionsBehavior.ReadOnly = isFormView;
            bgvSecurity.OptionsBehavior.ReadOnly = isFormView;

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

        private void f207_Document_ViewOnly_Load(object sender, EventArgs e)
        {
            // Set GridView to read-only and assign data sources to grid controls
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
                //progressSelect = db.dm_Progress.FirstOrDefault();

                idDocument = docProcess.IdKnowledgeBase;

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
                    lsAttachments.AddRange(db.KnowledgeAttachments.Where(r => r.IdKnowledgeBase == idDocument)
                        .Select(item => new Attachments { FileName = item.FileName, EncryptionName = item.EncryptionName }));

                    sourceAttachments.DataSource = lsAttachments;
                    lbCountFile.Text = $"共{lsAttachments.Count}個附件";

                    // Retrieve knowledge security information from database and assign to sourceSecuritys
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


                    controlgroupDocument.SelectedTabPage = groupProgress;

                    int idProgressByDoc = docProcess.IdProgress;
                    int idDocProgress = docProcess.Id;
                    var lsDMStepProgress = db.dm_StepProgress.Where(r => r.IdProgress == idProgressByDoc).ToList();
                    var lsDocProgressInfos = db.DocProgressInfoes.Where(r => r.IdDocProgress == idDocProgress).ToList();

                    //finishStep = lsDMStepProgress.Max(r => r.IndexStep);

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
                }
            }

            permissionAttachments = GetPermission();
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
    }
}