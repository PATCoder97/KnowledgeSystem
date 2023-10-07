using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Layout;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Configs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using dm_Group = DataAccessLayer.dm_Group;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._07_KnowledgeBase
{
    public partial class f207_Document_Info : DevExpress.XtraEditors.XtraForm
    {
        RefreshHelper helper;
        public Event207DocInfo _event207 = Event207DocInfo.Create;

        public f207_Document_Info()
        {
            InitializeComponent();
            InitializeControl();
        }

        public f207_Document_Info(string idDocument_)
        {
            InitializeComponent();
            InitializeControl();
            _idBaseDocument = idDocument_;
        }

        #region parameters

        // Khai báo các BUS
        dm_UserBUS _dm_UserBUS = new dm_UserBUS();
        dm_GroupBUS _dm_GroupBUS = new dm_GroupBUS();
        dm_GroupUserBUS _dm_GroupUserBUS = new dm_GroupUserBUS();
        dm_ProgressBUS _dm_ProgressBUS = new dm_ProgressBUS();
        dm_StepProgressBUS _dm_StepProgressBUS = new dm_StepProgressBUS();
        dt207_TypeBUS _dt207_TypeBUS = new dt207_TypeBUS();
        dt207_BaseBUS _dt207_BaseBUS = new dt207_BaseBUS();
        dt207_Base_BAKBUS _dt207_Base_BAKBUS = new dt207_Base_BAKBUS();
        dt207_AttachmentBUS _dt207_AttachmentBUS = new dt207_AttachmentBUS();
        dt207_Attachment_BAKBUS _dt207_Attachment_BAKBUS = new dt207_Attachment_BAKBUS();
        dt207_SecurityBUS _dt207_SecurityBUS = new dt207_SecurityBUS();
        dt207_Security_BAKBUS _dt207_Security_BAKBUS = new dt207_Security_BAKBUS();
        dt207_DocProgressBUS _dt207_DocProgressBUS = new dt207_DocProgressBUS();
        dt207_DocProgressInfoBUS _dt207_DocProgressInfoBUS = new dt207_DocProgressInfoBUS();
        dt207_HistoryGetFileBUS _dt207_HistoryGetFileBUS = new dt207_HistoryGetFileBUS();

        // Khai báo các source
        BindingSource _BSAttachments = new BindingSource();
        BindingSource _BSSecuritys = new BindingSource();

        string _idBaseDocument = string.Empty;
        string _userId = string.Empty;

        List<dt207_Type> lsKnowledgeTypes = new List<dt207_Type>();
        List<dm_User> lsUsers = new List<dm_User>();
        List<dm_Group> lsGroups = new List<dm_Group>();
        List<Securityinfo> lsSecurityInfos = new List<Securityinfo>();
        List<Attachments> lsAttachments = new List<Attachments>();
        List<Securityinfo> lsIdGroupOrUser = new List<Securityinfo>();
        List<dm_GroupUser> lsGroupUser = new List<dm_GroupUser>();
        List<LayoutView> lsLayoutViewed;

        Securityinfo permissionAttachments = new Securityinfo();
        dm_Progress progressSelect = new dm_Progress();

        int _idDocProcessing = -1;
        int _stepEnd = -1;
        Event207DocInfo _eventApproved = Event207DocInfo.Update;

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

        private class LayoutView
        {
            public string Name { get; set; }
            public string Text { get; set; }
            public bool Confirm { get; set; }
        }

        #endregion

        #region methods

        private void InitializeControl()
        {
            // Cài mặc định các thông số
            helper = new RefreshHelper(bgvSecurity, "Id");
            lcgProgress.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            lcgHistoryEdit.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

            btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            btnDel.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            btnChangeProgress.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            btnApproved.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnDisapprove.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnCancel.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnViewed.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnViewed.ImageOptions.SvgImage = TPSvgimages.UncheckedRadio;
            btnViewed.ItemAppearance.Normal.ForeColor = Color.Red;

            cbbUserProcess.Enabled = false;
            txbId.Enabled = false;

            controlgroupDocument.SelectedTabPage = lcgInfo;

            // Gắn các thông số cho các combobox
            rgvGruopOrUser.ValueMember = "IdGroupOrUser";
            rgvGruopOrUser.DisplayMember = "DisplayName";
            rgvGruopOrUser.PopupView.Columns.AddRange(new[]
            {
                    new GridColumn { FieldName = "IdDept", VisibleIndex = 0, Caption = "部門" },
                    new GridColumn { FieldName = "IdGroupOrUser", VisibleIndex = 1, Caption = "代號" },
                    new GridColumn { FieldName = "DisplayName", VisibleIndex = 2, Caption = "名稱" }
            });

            cbbType.Properties.ValueMember = "Id";
            cbbType.Properties.DisplayMember = "DisplayName";

            cbbUserUpload.Properties.ValueMember = "Id";
            cbbUserUpload.Properties.DisplayMember = "DisplayName";

            cbbUserProcess.Properties.ValueMember = "Id";
            cbbUserProcess.Properties.DisplayMember = "DisplayName";

            // Cài các gridview readonly
            gvEditHistory.ReadOnlyGridView();
            gvFiles.ReadOnlyGridView();
            gvHistoryProcess.ReadOnlyGridView();

            // Load các dữ liệu LIST ban đầu
            lsKnowledgeTypes = _dt207_TypeBUS.GetList();
            lsUsers = _dm_UserBUS.GetList();
            lsGroups = _dm_GroupBUS.GetList();
            lsGroupUser = _dm_GroupUserBUS.GetList();
            progressSelect = _dm_ProgressBUS.GetList().FirstOrDefault();

            // Create lists of Securityinfo objects from lsUsers and lsGroups
            var lsIdUsers = lsUsers.Select(r => new Securityinfo { IdDept = r.IdDepartment, IdGroupOrUser = r.Id, DisplayName = r.DisplayName }).ToList();
            var lsIdGroup = lsGroups.Select(r => new Securityinfo { IdGroupOrUser = r.Id.ToString(), DisplayName = r.DisplayName }).ToList();
            lsIdGroupOrUser = lsIdGroup.Concat(lsIdUsers).ToList();

            // Assign data source and columns to rgvGruopOrUser
            rgvGruopOrUser.DataSource = lsIdGroupOrUser;
            cbbType.Properties.DataSource = lsKnowledgeTypes;
            cbbUserUpload.Properties.DataSource = lsUsers;
            cbbUserProcess.Properties.DataSource = lsUsers;
        }

        private void LockControl()
        {
            switch (_event207)
            {
                case Event207DocInfo.Create:
                    EnableControls(true);
                    ShowControls(true);
                    btnCancel.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    break;
                case Event207DocInfo.View:
                    Text = "群組信息";
                    EnableControls(false);
                    ShowControls(false);
                    break;
                case Event207DocInfo.Update:
                    goto case Event207DocInfo.Create;
                case Event207DocInfo.Delete:
                    break;
                case Event207DocInfo.Approval:
                    Text = "群組信息";
                    EnableControls(false);
                    ShowControls(false);
                    HideOptions();
                    break;
                default:
                    Text = "群組信息";
                    EnableControls(false);
                    ShowControls(false);
                    HideOptions();
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    break;
            }
        }

        private void EnableControls(bool enabled)
        {
            cbbType.Enabled = enabled;
            txbNameTW.Enabled = enabled;
            txbNameVN.Enabled = enabled;
            txbNameEN.Enabled = enabled;
            cbbUserUpload.Enabled = enabled;
            txbKeyword.Enabled = enabled;
            btnAddFile.Enabled = enabled;
            btnAddPermission.Enabled = enabled;
            gvFiles.OptionsBehavior.ReadOnly = !enabled;
            bgvSecurity.OptionsBehavior.ReadOnly = !enabled;
            gColDelFile.Visible = enabled;
            gColDelPermission.Visible = enabled;
        }

        private void ShowControls(bool visible)
        {
            btnEdit.Visibility = visible ? DevExpress.XtraBars.BarItemVisibility.Never : DevExpress.XtraBars.BarItemVisibility.Always;
            btnDel.Visibility = visible ? DevExpress.XtraBars.BarItemVisibility.Never : DevExpress.XtraBars.BarItemVisibility.Always;
            btnConfirm.Visibility = visible ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
            btnChangeProgress.Visibility = visible ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
            lcProgress.Visibility = visible ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
        }

        private void HideOptions()
        {
            btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnDel.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnChangeProgress.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            lcProgress.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
        }

        private string GenerateHashFileName()
        {
            var userLogin = lsUsers.FirstOrDefault(u => u.Id == _userId);
            if (userLogin == null)
            {
                throw new Exception($"User with Id {_userId} not found.");
            }

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var randomString = new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)])
                .ToArray());

            return $"{userLogin.Id}-{userLogin.IdDepartment.Substring(0, 3)}-{DateTime.Now:MMddhhmmss}-{randomString}";
        }

        public string RemoveDiacriticalMarks(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        private Securityinfo GetPermission()
        {
            string userIdLogin = TPConfigs.LoginUser.Id;

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

        private void CheckViewLayout()
        {
            var tabFocus = controlgroupDocument.SelectedTabPage;

            if (lsLayoutViewed == null || _event207 != Event207DocInfo.Approval)
            {
                return;
            }
            bool IsExistView = lsLayoutViewed.Any(r => r.Name == tabFocus.Name);
            //btnViewed.Visibility = IsExistView ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
            if (!IsExistView)
            {
                btnViewed.ImageOptions.SvgImage = TPSvgimages.CheckedRadio;
                btnViewed.Caption = "通過";
                btnViewed.ItemAppearance.Normal.ForeColor = Color.Black;
                return;
            }

            if (!lsLayoutViewed.Any(r => r.Confirm == false))
            {
                btnApproved.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                btnDisapprove.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                btnViewed.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }
            else
            {
                bool IsViewed = lsLayoutViewed.FirstOrDefault(r => r.Name == tabFocus.Name)?.Confirm ?? false;
                btnViewed.ImageOptions.SvgImage = IsViewed ? TPSvgimages.CheckedRadio : TPSvgimages.UncheckedRadio;
                btnViewed.Caption = IsViewed ? "通過" : "確認";
                btnViewed.ItemAppearance.Normal.ForeColor = IsViewed ? Color.Black : Color.Red;
            }
        }

        #endregion

        private void f207_DocumentInfo_Load(object sender, EventArgs e)
        {
            Text = "新增群組";

            // Set datasource cho gridcontrol
            gcFiles.DataSource = _BSAttachments;
            gcSecurity.DataSource = _BSSecuritys;

            // Cài các giá trị mặc định
            _userId = TPConfigs.LoginUser.Id;
            cbbUserUpload.EditValue = _userId;
            cbbUserProcess.EditValue = _userId;

            // Tuỳ theo từng event để hiển thị lên form
            switch (_event207)
            {
                case Event207DocInfo.Create:
                    txbId.Text = "XXX-XXXXXXXXXX-XX";
                    cbbType.EditValue = 1;
                    lbProgress.Text = "流程：" + progressSelect.DisplayName;
                    break;
                case Event207DocInfo.View:
                    lcgHistoryEdit.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                    // Thông tin cơ bản
                    var base207Info = _dt207_BaseBUS.GetItemById(_idBaseDocument);
                    txbId.Text = base207Info.Id;
                    cbbType.EditValue = base207Info.IdTypes;
                    cbbUserUpload.EditValue = base207Info.UserUpload;
                    cbbUserProcess.EditValue = base207Info.UserProcess;
                    var displayName = base207Info.DisplayName.Split(new[] { "\n" }, StringSplitOptions.None);
                    txbNameTW.Text = displayName[0];
                    txbNameVN.Text = displayName.Length > 1 ? displayName[1] : "";
                    txbKeyword.Text = base207Info.Keyword;

                    // Thông tin phụ kiện
                    lsAttachments.AddRange(_dt207_AttachmentBUS.GetListByIdBase(_idBaseDocument)
                        .Select(item => new Attachments
                        {
                            FileName = item.FileName,
                            EncryptionName = item.EncryptionName
                        }));

                    _BSAttachments.DataSource = lsAttachments;
                    lbCountFile.Text = $"共{lsAttachments.Count}個附件";

                    // Thông tin quyền hạn
                    helper.SaveViewInfo();
                    var lsBaseSecuritys = _dt207_SecurityBUS.GetListByIdBase(_idBaseDocument);
                    lsSecurityInfos = lsBaseSecuritys.Select(data => new Securityinfo
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

                    _BSSecuritys.DataSource = lsSecurityInfos;
                    bgvSecurity.BestFitColumns();
                    helper.LoadViewInfo();

                    // Thông tin lịch sửa cập nhật
                    var lsDocProcess = _dt207_DocProgressBUS.GetListByIdBase(_idBaseDocument).Where(r => r.Id != _idDocProcessing);
                    //db.dt207_DocProgress.Where(r => r.IdKnowledgeBase == idDocument && r.Descriptions == TPConfigs.EventEdit).ToList();
                    var lsDocProcessInfos =
                        (from data in _dt207_DocProgressInfoBUS.GetList()
                         group data by data.IdDocProgress into g
                         select new
                         {
                             IdDocProgress = g.Key,
                             IndexStep = g.OrderByDescending(dpi => dpi.TimeStep).Select(dpi => dpi.IndexStep).FirstOrDefault(),
                             TimeStep = g.OrderByDescending(dpi => dpi.TimeStep).Select(dpi => dpi.TimeStep).FirstOrDefault(),
                             IdUserProcess = g.OrderBy(dpi => dpi.TimeStep).Select(dpi => dpi.IdUserProcess).FirstOrDefault()
                         }).ToList();

                    var lsHisUpdates = (from data in lsDocProcess
                                        join infos in lsDocProcessInfos on data.Id equals infos.IdDocProgress
                                        join users in lsUsers on data.IdUserProcess equals users.Id
                                        select new
                                        {
                                            data.IdKnowledgeBase,
                                            data.Change,
                                            infos.TimeStep,
                                            UserProcess = $"{users.IdDepartment} | {infos.IdUserProcess}/{users.DisplayName}",
                                        }).OrderByDescending(r => r.TimeStep).ToList();

                    gcEditHistory.DataSource = lsHisUpdates;

                    break;
                case Event207DocInfo.Approval:
                    lcgHistoryEdit.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lcgProgress.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    controlgroupDocument.SelectedTabPage = lcgProgress;

                    var _docProcessing = _dt207_DocProgressBUS.GetItemByIdBaseNotComplete(_idBaseDocument);
                    if (_docProcessing == default)
                    {
                        XtraMessageBox.Show("文件已由其他主管處理完成", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Close();
                    }

                    int _idProgress = _docProcessing.IdProgress;
                    _idDocProcessing = _docProcessing.Id;
                    _eventApproved = EnumHelper.GetEnumByDescription<Event207DocInfo>(_docProcessing.Descriptions);

                    var lsStepProgress = _dm_StepProgressBUS.GetListByIdProgress(_idProgress);
                    _stepEnd = lsStepProgress.Max(r => r.IndexStep);
                    var lsDisplayNameSteps = (from data in lsStepProgress
                                              join groups in lsGroups on data.IdGroup equals groups.Id
                                              select new { groups.DisplayName }).ToList();
                    // Thêm danh sách các bước vào StepProgressBar
                    stepProgressDoc.Items.Add(new StepProgressBarItem("經辦人"));
                    foreach (var item in lsDisplayNameSteps)
                        stepProgressDoc.Items.Add(new StepProgressBarItem(item.DisplayName));
                    stepProgressDoc.ItemOptions.Indicator.Width = 40;

                    var lsDocProcessingInfos = _dt207_DocProgressInfoBUS.GetListByIdDocProcess(_idDocProcessing);
                    int _stepNow = lsDocProcessingInfos.OrderByDescending(r => r.TimeStep).First().IndexStep;
                    stepProgressDoc.SelectedItemIndex = _stepNow; // Focus đến bước hiện tại

                    // Thêm lịch sử trình ký vào gridProcess
                    var lsHistoryProcess = (from data in lsDocProcessingInfos
                                            join users in lsUsers on data.IdUserProcess equals users.Id
                                            select new
                                            {
                                                TimeStep = data.TimeStep,
                                                data.Descriptions,
                                                UserProcess = $"{users.IdDepartment} | {data.IdUserProcess}/{users.DisplayName}",
                                            }).ToList();

                    gcHistoryProcess.DataSource = lsHistoryProcess;

                    if (_stepNow != -1)
                    {
                        btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                        //btnApproved.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                        //btnDisapprove.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                        btnViewed.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    }
                    else
                    {
                        btnCancel.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                        btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                        btnApproved.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                        btnDisapprove.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    }

                    goto case Event207DocInfo.View;
                case Event207DocInfo.Check:
                    goto case Event207DocInfo.Approval;
            }

            // Thêm sự kiện xác nhận đã xem khi trình ký
            if (_event207 == Event207DocInfo.Approval)
            {
                lsLayoutViewed = controlgroupDocument.TabPages
                    .Where(r => r.Visibility == LayoutVisibility.Always && r.Name != "lcgHistoryEdit")
                    .Select(r => new LayoutView
                    {
                        Name = r.Name,
                        Text = r.Text,
                        Confirm = false
                    }).ToList();

                //  btnDisabtnbtnpprove.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }

            // Lấy quyền hạn của người dùng đối với văn kiện này
            permissionAttachments = GetPermission() ?? new Securityinfo
            {
                DeleteInfo = false,
                UpdateInfo = false,
                ReadFile = false,
                SaveFile = false
            };

            LockControl();
        }

        private void f207_DocumentInfo_Shown(object sender, EventArgs e)
        {
            txbNameTW.Focus();
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
                string encryptionName = GenerateHashFileName();
                Attachments attachment = new Attachments
                {
                    FullPath = fileName,
                    FileName = Path.GetFileName(fileName),
                    EncryptionName = encryptionName
                };
                lsAttachments.Add(attachment);
                Thread.Sleep(5);
            }

            _BSAttachments.DataSource = lsAttachments;
            lbCountFile.Text = $"共{lsAttachments.Count}個附件";
            gvFiles.RefreshData();
        }

        private void btnDelFile_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            Attachments attachment = gvFiles.GetRow(gvFiles.FocusedRowHandle) as Attachments;
            DialogResult dialogResult = XtraMessageBox.Show($"您想要刪除附件：{attachment.FileName}?", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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

        private void btnAddPermission_Click(object sender, EventArgs e)
        {
            lsSecurityInfos.Add(new Securityinfo()
            {
                IdKnowledgeBase = _idBaseDocument,
                ReadInfo = true,
                UpdateInfo = false,
                DeleteInfo = false,
                SearchInfo = true,
                ReadFile = false,
                SaveFile = false,
            });

            _BSSecuritys.DataSource = lsSecurityInfos;
            bgvSecurity.RefreshData();
        }

        private void btnDelPermission_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            Securityinfo permission = bgvSecurity.GetRow(bgvSecurity.FocusedRowHandle) as Securityinfo;
            permission.DisplayName = lsIdGroupOrUser.FirstOrDefault(r => r.IdGroupOrUser == permission.IdGroupOrUser)?.DisplayName;

            DialogResult dialogResult = XtraMessageBox.Show($"您想要刪除權限：{permission.DisplayName}?", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.No)
                return;

            lsSecurityInfos.Remove(permission);
            bgvSecurity.RefreshData();
        }

        private void btnDel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (permissionAttachments.DeleteInfo != true)
            {
                XtraMessageBox.Show(TPConfigs.NoPermission, TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            var dialogResult = XtraMessageBox.Show($"您确定要删除该文件：{_idBaseDocument}", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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
            _event207 = Event207DocInfo.Delete;

            using (var db = new DBDocumentManagementSystemEntities())
            {
                dt207_DocProgress docProgress = new dt207_DocProgress()
                {
                    IdKnowledgeBase = _idBaseDocument,
                    IsComplete = false,
                    IsSuccess = false,
                    IdProgress = progressSelect.Id,
                    Descriptions = EnumHelper.GetDescription(_event207),
                    IdUserProcess = TPConfigs.LoginUser.Id
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
            bool _IsProcessing = false;

            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                switch (_event207)
                {
                    case Event207DocInfo.Create:
                        _idBaseDocument = _dt207_BaseBUS.GetNewBaseId(TPConfigs.LoginUser.IdDepartment);
                        break;
                    case Event207DocInfo.Update:
                        _IsProcessing = _dt207_DocProgressBUS.CheckItemProcessing(_idBaseDocument);
                        if (_IsProcessing)
                        {
                            break;
                        }

                        // Lưu lại các thông tin để sau này rollback nếu huỷ
                        dt207_Base base_BAK = _dt207_BaseBUS.GetItemById(_idBaseDocument);
                        _dt207_Base_BAKBUS.Create(base_BAK);

                        List<dt207_Attachment> lsAttachments_BAK = _dt207_AttachmentBUS.GetListByIdBase(_idBaseDocument);
                        _dt207_Attachment_BAKBUS.CreateRange(lsAttachments_BAK);

                        List<dt207_Security> lsSecurities_BAK = _dt207_SecurityBUS.GetListByIdBase(_idBaseDocument);
                        _dt207_Security_BAKBUS.CreateRange(lsSecurities_BAK);

                        break;
                }

                // Tạo ra DocBase từ các thông tin trên form, thêm vào DB
                string _nameTW = RemoveDiacriticalMarks(txbNameTW.Text.Trim());
                string _nameVN = txbNameVN.Text.Trim();
                string _nameEN = RemoveDiacriticalMarks(txbNameEN.Text.Trim());
                string _keyword = Regex.Replace(txbKeyword.Text, @"(\s{2,}|\n{2,}|[\t])", " ").Trim();

                dt207_Base knowledge = new dt207_Base()
                {
                    Id = _idBaseDocument,
                    DisplayName = string.Join("\n", new[] { _nameTW, _nameVN, _nameEN }.Where(s => !string.IsNullOrWhiteSpace(s))),
                    UserProcess = cbbUserProcess.EditValue.ToString(),
                    IdTypes = (int)cbbType.EditValue,
                    Keyword = _keyword,
                    UserUpload = cbbUserUpload.EditValue.ToString(),
                    UploadDate = DateTime.Now
                };

                _dt207_BaseBUS.AddOrUpdate(knowledge);

                // Xoá tất cả các phụ kiện trước đó để thêm lại (Không xoá file cứng, định kỳ làm sạch file cứng nếu không dùng đến)
                _dt207_AttachmentBUS.RemoveRangeByIdBase(_idBaseDocument);
                var lsFilesAdd = lsAttachments.Select(r => new dt207_Attachment
                {
                    IdKnowledgeBase = _idBaseDocument,
                    EncryptionName = r.EncryptionName,
                    FileName = r.FileName
                }).ToList();
                _dt207_AttachmentBUS.AddRange(lsFilesAdd);

                foreach (Attachments item in lsAttachments)
                {
                    // Thêm các file cứng vào Server nếu là file mới
                    if (!string.IsNullOrEmpty(item.FullPath))
                    {
                        string sourceFileName = item.FullPath;
                        string destFileName = Path.Combine(TPConfigs.PathKnowledgeFile, item.EncryptionName);

                        File.Copy(sourceFileName, destFileName, true);
                    }
                }

                // Xoá tất cả các dữ liệu quyền hạn để thêm lại dựa vào dữ liệu trên form
                _dt207_SecurityBUS.RemoveRangeByIdBase(_idBaseDocument);
                var lsSecuritiesAdd = lsSecurityInfos.Select(r => new dt207_Security
                {
                    Id = r.Id,
                    IdKnowledgeBase = _idBaseDocument,
                    IdGroup = r.IdGroupOrUser.StartsWith("VNW") ? null : (short?)Convert.ToInt16(r.IdGroupOrUser),
                    IdUser = r.IdGroupOrUser.StartsWith("VNW") ? r.IdGroupOrUser : null,
                    ReadInfo = r.ReadInfo,
                    UpdateInfo = r.UpdateInfo,
                    DeleteInfo = r.DeleteInfo,
                    SearchInfo = r.SearchInfo,
                    ReadFile = r.ReadFile,
                    SaveFile = r.SaveFile
                }).ToList();
                _dt207_SecurityBUS.AddRange(lsSecuritiesAdd);

                //// Nếu chưa có lưu trình xử lý của văn kiện thì thêm mới (Trigger sẽ tự thêm vào ProcessInfo)
                if (!_IsProcessing)
                {
                    dt207_DocProgress docProgress = new dt207_DocProgress()
                    {
                        IdKnowledgeBase = _idBaseDocument,
                        IsComplete = false,
                        IsSuccess = false,
                        IdProgress = progressSelect.Id,
                        Descriptions = EnumHelper.GetDescription(_event207),
                        IdUserProcess = TPConfigs.LoginUser.Id,
                    };
                    _dt207_DocProgressBUS.Create(docProgress);
                }
                else
                {
                    dt207_DocProgressInfo progressInfo = new dt207_DocProgressInfo()
                    {
                        IdDocProgress = _idDocProcessing,
                        TimeStep = DateTime.Now,
                        IndexStep = 0,
                        IdUserProcess = TPConfigs.LoginUser.Id,
                        Descriptions = "呈核",
                    };
                    _dt207_DocProgressInfoBUS.Create(progressInfo);
                }
            }

            // Show a message box with the appropriate message and close the form
            XtraMessageBox.Show($"{EnumHelper.GetDescription(_event207)}!\r\n文件編號:{_idBaseDocument}", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
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

            // Lưu lại lịch sử xem file
            var IsProcessing = _dt207_DocProgressBUS.CheckItemProcessing(_idBaseDocument);
            if (!string.IsNullOrEmpty(_idBaseDocument) && !IsProcessing)
            {
                dt207_HistoryGetFile historyGetFile = new dt207_HistoryGetFile()
                {
                    IdKnowledgeBase = _idBaseDocument,
                    TypeGet = TPConfigs.strReadFile,
                    KnowledgeAttachmentName = dataRow.FileName,
                    IdUser = TPConfigs.LoginUser.Id,
                    TimeGet = DateTime.Now
                };
                _dt207_HistoryGetFileBUS.Create(historyGetFile);
            }

            string pathDocTemp = Path.Combine(TPConfigs.TempFolderData, $"{DateTime.Now:MMddhhmmss} {dataRow.FileName}");
            using (var handle = SplashScreenManager.ShowOverlayForm(gcFiles))
            {
                // Copy file về thư mục tạm để xem
                if (!Directory.Exists(TPConfigs.TempFolderData))
                    Directory.CreateDirectory(TPConfigs.TempFolderData);

                File.Copy(documentsFile, pathDocTemp, true);

                f207_ViewFile fDocumentInfo = new f207_ViewFile(pathDocTemp, _idBaseDocument)
                {
                    Text = dataRow.FileName,
                    CanSaveFile = permissionAttachments.SaveFile
                };
                fDocumentInfo.ShowDialog();
            }
        }

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (permissionAttachments.UpdateInfo != true)
            {
                XtraMessageBox.Show(TPConfigs.NoPermission, TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            // Chuyển người process sang userLogin
            cbbUserProcess.EditValue = _userId;
            controlgroupDocument.SelectedTabPage = lcgInfo;

            _event207 = Event207DocInfo.Update;
            LockControl();
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
            List<string> lsChangeDetails = new List<string>();
            var lsBaseProgressById = _dt207_DocProgressBUS.GetListByIdBase(_idBaseDocument);
            int _idBaseProgress = lsBaseProgressById.OrderByDescending(r => r.Id).FirstOrDefault().Id;
            string _eventProcess = lsBaseProgressById.OrderByDescending(r => r.Id).FirstOrDefault().Descriptions;
            _eventApproved = EnumHelper.GetEnumByDescription<Event207DocInfo>(_eventProcess);

            var lsBaseProcessInfos = _dt207_DocProgressInfoBUS.GetListByIdDocProcess(_idBaseProgress);
            int _indexStep = lsBaseProcessInfos.Count != 0 ? lsBaseProcessInfos.OrderByDescending(r => r.Id).FirstOrDefault().IndexStep + 1 : 0;

            switch (_eventApproved)
            {
                case Event207DocInfo.Update:
                    // Kiểm tra thông tin cơ bản có đổi không
                    var docBaseNew = _dt207_BaseBUS.GetItemById(_idBaseDocument);
                    var docBaseOld = _dt207_Base_BAKBUS.GetItemById(_idBaseDocument);

                    if (docBaseNew.DisplayName != docBaseOld.DisplayName) lsChangeDetails.Add("名稱");
                    if (docBaseNew.IdTypes != docBaseOld.IdTypes) lsChangeDetails.Add("類別");
                    if (docBaseNew.Keyword != docBaseOld.Keyword) lsChangeDetails.Add("關鍵字");

                    // Kiểm tra phụ kiện
                    var lsAttachmentNew = _dt207_AttachmentBUS.GetListByIdBase(_idBaseDocument)
                        .Select(r => new { r.FileName, r.EncryptionName })
                        .ToList();
                    var lsAttachmentOld = _dt207_Attachment_BAKBUS.GetListByIdBase(_idBaseDocument)
                        .Select(r => new { r.FileName, r.EncryptionName })
                        .ToList();

                    var jsonAttachmentNew = JsonConvert.SerializeObject(lsAttachmentNew);
                    var jsonAttachmentOld = JsonConvert.SerializeObject(lsAttachmentOld);
                    if (jsonAttachmentNew != jsonAttachmentOld)
                        lsChangeDetails.Add("附件");

                    // Kiểm tra quyền hạn
                    var lsSecurityNew = _dt207_SecurityBUS.GetListByIdBase(_idBaseDocument)
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
                    var lsSecurityOld = _dt207_Security_BAKBUS.GetListByIdBase(_idBaseDocument)
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
                    break;
                case Event207DocInfo.Delete:
                    _dt207_BaseBUS.Delete(_idBaseDocument);
                    break;
            }

            string descriptions = "核准";
            if (_indexStep == _stepEnd)
            {
                var docProcessUpdate = _dt207_DocProgressBUS.GetItemById(_idBaseProgress);// db.dt207_DocProgress.First(r => r.Id == idDocProgress);
                docProcessUpdate.IsSuccess = true;
                docProcessUpdate.IsComplete = true;
                docProcessUpdate.Change = string.Join("，", lsChangeDetails);
                _dt207_DocProgressBUS.AddOrUpdate(docProcessUpdate);
                //db.dt207_DocProgress.AddOrUpdate(docProcessUpdate);

                descriptions = "確認完畢";
                // Xoá dữ liệu cũ
                _dt207_Base_BAKBUS.RemoveRangeById(_idBaseDocument);
                _dt207_Attachment_BAKBUS.RemoveRangeByIdBase(_idBaseDocument);
                _dt207_Security_BAKBUS.RemoveRangeByIdBase(_idBaseDocument);
            }

            dt207_DocProgressInfo progressInfo = new dt207_DocProgressInfo()
            {
                IdDocProgress = _idBaseProgress,
                TimeStep = DateTime.Now,
                IndexStep = _indexStep,
                IdUserProcess = TPConfigs.LoginUser.Id,
                Descriptions = descriptions,
            };
            _dt207_DocProgressInfoBUS.Create(progressInfo);

            Close();
        }

        private void btnDisapprove_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XtraInputBoxArgs args = new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                Prompt = "退回文件原因",
                DefaultButtonIndex = 0,
                Editor = new MemoEdit()
            };

            var result = XtraInputBox.Show(args);
            string descriptions = result?.ToString() ?? "";

            if (_eventApproved == Event207DocInfo.Delete)
            {
                var docProcessUpdate = _dt207_DocProgressBUS.GetItemById(_idDocProcessing);//  db.dt207_DocProgress.First(r => r.Id == _idDocProcessing);
                docProcessUpdate.IsSuccess = false;
                docProcessUpdate.IsComplete = true;
                _dt207_DocProgressBUS.AddOrUpdate(docProcessUpdate);
            }

            dt207_DocProgressInfo progressInfo = new dt207_DocProgressInfo
            {
                IdDocProgress = _idDocProcessing,
                TimeStep = DateTime.Now,
                IndexStep = -1,
                IdUserProcess = TPConfigs.LoginUser.Id,
                Descriptions = string.IsNullOrEmpty(descriptions) ? "退回" : $"退回，說明：{descriptions}"
            };

            _dt207_DocProgressInfoBUS.Create(progressInfo);

            Close();
        }

        private void btnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Tạo textbox lấy nguyên nhân
            XtraInputBoxArgs args = new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                Prompt = "原因",
                DefaultButtonIndex = 0,
                Editor = new MemoEdit()
            };

            var result = XtraInputBox.Show(args);
            string descriptions = string.IsNullOrEmpty(result?.ToString()) ? "取消" : $"取消，說明：{result}";

            var docProcessUpdate = _dt207_DocProgressBUS.GetItemById(_idDocProcessing);
            _eventApproved = EnumHelper.GetEnumByDescription<Event207DocInfo>(docProcessUpdate.Descriptions);

            docProcessUpdate.IsSuccess = false;
            docProcessUpdate.IsComplete = true;
            docProcessUpdate.Change = descriptions;
            _dt207_DocProgressBUS.AddOrUpdate(docProcessUpdate);

            dt207_DocProgressInfo progressInfo = new dt207_DocProgressInfo()
            {
                IdDocProgress = _idDocProcessing,
                TimeStep = DateTime.Now,
                IndexStep = -1,
                IdUserProcess = TPConfigs.LoginUser.Id,
                Descriptions = descriptions,
            };
            _dt207_DocProgressInfoBUS.Create(progressInfo);

            switch (_eventApproved)
            {
                case Event207DocInfo.Create:
                    var docBaseDelete = _dt207_BaseBUS.GetItemById(_idBaseDocument);
                    docBaseDelete.IsDelete = true;
                    _dt207_BaseBUS.AddOrUpdate(docBaseDelete);
                    break;
                case Event207DocInfo.Update:
                    dt207_Base baseDoc = _dt207_Base_BAKBUS.GetItemById(_idBaseDocument);
                    _dt207_BaseBUS.AddOrUpdate(baseDoc);

                    List<dt207_Attachment> lsAttachments = _dt207_Attachment_BAKBUS.GetListByIdBase(_idBaseDocument);
                    _dt207_AttachmentBUS.AddRange(lsAttachments);

                    List<dt207_Security> lsSecurities = _dt207_Security_BAKBUS.GetListByIdBase(_idBaseDocument);
                    _dt207_SecurityBUS.AddRange(lsSecurities);

                    // Xoá dữ liệu cũ
                    _dt207_Base_BAKBUS.RemoveRangeById(_idBaseDocument);
                    _dt207_Attachment_BAKBUS.RemoveRangeByIdBase(_idBaseDocument);
                    _dt207_Security_BAKBUS.RemoveRangeByIdBase(_idBaseDocument);
                    break;
            }

            Close();
        }

        private void btnViewed_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var tabFocus = controlgroupDocument.SelectedTabPage;

            lsLayoutViewed.Where(lv => lv.Name == tabFocus.Name).ToList().ForEach(lv => lv.Confirm = true);

            CheckViewLayout();
        }

        private void controlgroupDocument_SelectedPageChanged(object sender, LayoutTabPageChangedEventArgs e)
        {
            CheckViewLayout();
        }
    }
}