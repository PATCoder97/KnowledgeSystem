using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Layout;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
            InitializeIcon();
        }

        public f207_Document_Info(string idDocument_)
        {
            InitializeComponent();
            InitializeControl();
            InitializeIcon();
            _idBaseDocument = idDocument_;
        }

        #region parameters

        // Khai báo các BUS
        dt207_BaseBUS _dt207_BaseBUS = new dt207_BaseBUS();
        dt207_Base_BAKBUS _dt207_Base_BAKBUS = new dt207_Base_BAKBUS();
        dt207_AttachmentBUS _dt207_AttachmentBUS = new dt207_AttachmentBUS();
        dt207_Attachment_BAKBUS _dt207_Attachment_BAKBUS = new dt207_Attachment_BAKBUS();
        dt207_SecurityBUS _dt207_SecurityBUS = new dt207_SecurityBUS();
        dt207_Security_BAKBUS _dt207_Security_BAKBUS = new dt207_Security_BAKBUS();
        dt207_HistoryGetFileBUS _dt207_HistoryGetFileBUS = new dt207_HistoryGetFileBUS();

        // Khai báo các source
        BindingSource _BSAttachments = new BindingSource();
        BindingSource _BSSecuritys = new BindingSource();

        string _idBaseDocument = string.Empty;
        string _userId = string.Empty;
        DateTime? _dateUpload = DateTime.Now;

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

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.EmailSend;
            btnDel.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnChangeProgress.ImageOptions.SvgImage = TPSvgimages.Progress;
            btnApproved.ImageOptions.SvgImage = TPSvgimages.Confirm;
            btnDisapprove.ImageOptions.SvgImage = TPSvgimages.Cancel;
            btnDisapprove.ImageOptions.SvgImage = TPSvgimages.Cancel;
            btnCancel.ImageOptions.SvgImage = TPSvgimages.Close;
            btnAddFile.ImageOptions.SvgImage = TPSvgimages.Add2;
            btnAddPermission.ImageOptions.SvgImage = TPSvgimages.Add2;
        }

        private void InitializeControl()
        {
            // Cài mặc định các thông số
            helper = new RefreshHelper(bgvSecurity, "Id");
            lcgProgress.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            lcgHistoryEdit.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

            lcType.Text = " 類別<color=red>*</color>";
            lcNameTW.Text = "中文名稱<color=red>*</color>";
            lcKeyword.Text = "關鍵字<color=red>*</color>";

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
            lsKnowledgeTypes = dt207_TypeBUS.Instance.GetList();
            lsUsers = dm_UserBUS.Instance.GetList().Where(r => r.Status == 0).ToList(); 
            lsGroups = dm_GroupBUS.Instance.GetList();
            lsGroupUser = dm_GroupUserBUS.Instance.GetList();
            progressSelect = dm_ProgressBUS.Instance.GetListByDept(TPConfigs.LoginUser.IdDepartment).FirstOrDefault();

            // Create lists of Securityinfo objects from lsUsers and lsGroups
            var lsIdUsers = lsUsers.Select(r => new Securityinfo { IdDept = r.IdDepartment, IdGroupOrUser = r.Id, DisplayName = r.DisplayName }).ToList();
            var lsIdGroup = lsGroups.Select(r => new Securityinfo { IdDept = r.IdDept, IdGroupOrUser = r.Id.ToString(), DisplayName = r.DisplayName }).ToList();
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
            var randomString = new string(Enumerable.Repeat(chars, 7)
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
                cbbUserProcess.EditValue.ToString() == userIdLogin ||
                _event207 == Event207DocInfo.Approval)
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

        private bool ValidateData()
        {
            bool IsOK = true;
            string msg = "請提供以下補充資訊：";
            if (string.IsNullOrEmpty(cbbType.EditValue?.ToString()))
            {
                msg += "</br> •類別";
                IsOK = false;
            }

            if (string.IsNullOrEmpty(txbNameTW.EditValue?.ToString()))
            {
                msg += "</br> •中文名稱";
                IsOK = false;
            }

            if (string.IsNullOrEmpty(txbKeyword.EditValue?.ToString()))
            {
                msg += "</br> •關鍵字";
                IsOK = false;
            }

            if (lsAttachments.Count == 0)
            {
                msg += "</br> •附件";
                IsOK = false;
            }

            if (lsSecurityInfos.Count == 0)
            {
                msg += "</br> •密等";
                IsOK = false;
            }

            if (lsSecurityInfos.Any(r => string.IsNullOrEmpty(r.IdGroupOrUser)))
            {
                msg += "</br> •選擇密等";
                IsOK = false;
            }

            if (!IsOK)
            {
                XtraMessageBoxArgs args = new XtraMessageBoxArgs();
                args.AllowHtmlText = DefaultBoolean.True;

                args.Caption = TPConfigs.SoftNameTW;
                args.Text = $"<font='Microsoft JhengHei UI' size=14>{msg}</font>";
                args.Buttons = new DialogResult[] { DialogResult.OK };

                XtraMessageBox.Show(args);
            }

            return IsOK;
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
                    //cbbType.EditValue = 1;
                    lbProgress.Text = "流程：" + progressSelect.DisplayName;
                    break;
                case Event207DocInfo.View:
                    lcgHistoryEdit.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lbProgress.Text = "流程：" + progressSelect.DisplayName;

                    // Thông tin cơ bản
                    var base207Info = _dt207_BaseBUS.GetItemById(_idBaseDocument);
                    txbId.Text = base207Info.Id;
                    cbbType.EditValue = base207Info.IdTypes;
                    cbbUserUpload.EditValue = base207Info.UserUpload;
                    cbbUserProcess.EditValue = base207Info.UserProcess;
                    var displayName = base207Info.DisplayName.Split(new[] { "\n" }, StringSplitOptions.None);
                    txbNameTW.Text = displayName[0];
                    txbNameVN.Text = displayName.Length > 1 ? displayName[1] : "";
                    txbNameEN.Text = displayName.Length > 2 ? displayName[2] : "";
                    txbKeyword.Text = base207Info.Keyword;
                    _dateUpload = base207Info.UploadDate;

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
                    var lsDocProcess = dt207_DocProcessingBUS.Instance.GetListByIdBase(_idBaseDocument).Where(r => r.Id != _idDocProcessing);
                    var lsDocProcessInfos =
                        (from data in dt207_DocProcessingInfoBUS.Instance.GetList()
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

                    var _docProcessing = dt207_DocProcessingBUS.Instance.GetItemByIdBaseNotComplete(_idBaseDocument);
                    if (_docProcessing == default)
                    {
                        XtraMessageBox.Show("文件已由其他主管處理完成", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Close();
                    }

                    int _idProgress = _docProcessing.IdProgress;
                    _idDocProcessing = _docProcessing.Id;
                    _eventApproved = EnumHelper.GetEnumByDescription<Event207DocInfo>(_docProcessing.Descriptions);

                    var lsStepProgress = dm_StepProgressBUS.Instance.GetListByIdProgress(_idProgress);
                    _stepEnd = lsStepProgress.Max(r => r.IndexStep);
                    var lsDisplayNameSteps = (from data in lsStepProgress
                                              join groups in lsGroups on data.IdGroup equals groups.Id
                                              select new { groups.DisplayName }).ToList();
                    // Thêm danh sách các bước vào StepProgressBar
                    stepProgressDoc.Items.Add(new StepProgressBarItem("經辦人"));
                    foreach (var item in lsDisplayNameSteps)
                        stepProgressDoc.Items.Add(new StepProgressBarItem(item.DisplayName));
                    stepProgressDoc.ItemOptions.Indicator.Width = 40;

                    var lsDocProcessingInfos = dt207_DocProcessingInfoBUS.Instance.GetListByIdDocProcess(_idDocProcessing);
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
                        btnViewed.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    }
                    else
                    {
                        btnCancel.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                        btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
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
            if (_event207 == Event207DocInfo.Approval)
            {
                XtraMessageBoxArgs args = new XtraMessageBoxArgs();
                args.AllowHtmlText = DefaultBoolean.True;

                args.Caption = TPConfigs.SoftNameTW;
                args.Text = "<font='Microsoft JhengHei UI' size=18>請您確認文件各份資料(附件、核簽流程、密等等)\r\n後按<color=red>「確認」</color>以完成審查作業！</font>";
                args.Buttons = new DialogResult[] { DialogResult.OK };

                XtraMessageBox.Show(args);
            }
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

            int index = 0;
            foreach (string fileName in openFileDialog.FileNames)
            {
                string encryptionName = EncryptionHelper.EncryptionFileName(fileName);
                Attachments attachment = new Attachments
                {
                    FullPath = fileName,
                    FileName = Path.GetFileName(fileName),
                    EncryptionName = $"{encryptionName}{index++}"
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

            string msg = $"您想要刪除附件：\r\n{attachment.FileName}?";
            if (MsgTP.MsgYesNoQuestion(msg) == DialogResult.No)
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
                ReadFile = true,
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
                MsgTP.MsgNoPermission();
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

            dt207_DocProcessing docProgress = new dt207_DocProcessing()
            {
                IdKnowledgeBase = _idBaseDocument,
                IsComplete = false,
                IsSuccess = false,
                IdProgress = progressSelect.Id,
                Descriptions = EnumHelper.GetDescription(_event207),
                IdUserProcess = TPConfigs.LoginUser.Id
            };
            dt207_DocProcessingBUS.Instance.Add(docProgress);

            Close();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Đưa focused ra khỏi bảng để cập nhật lên source
            bgvSecurity.FocusedRowHandle = GridControl.AutoFilterRowHandle;
            if (ValidateData() == false)
            {
                return;
            }

            bool _IsProcessing = false;

            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                switch (_event207)
                {
                    case Event207DocInfo.Create:
                        _idBaseDocument = _dt207_BaseBUS.GetNewBaseId(TPConfigs.LoginUser.IdDepartment);
                        break;
                    case Event207DocInfo.Update:
                        _IsProcessing = dt207_DocProcessingBUS.Instance.CheckItemProcessing(_idBaseDocument);
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
                string _keyword = Regex.Replace(txbKeyword.Text, @"[\t\n\r\s]+", match =>
                {
                    if (match.Value.Contains("\n"))
                    {
                        return "\r\n";
                    }
                    else
                    {
                        return " ";
                    }
                }).Trim();

                dt207_Base knowledge = new dt207_Base()
                {
                    Id = _idBaseDocument,
                    DisplayName = string.Join("\n", new[] { _nameTW, _nameVN, _nameEN }),
                    UserProcess = cbbUserProcess.EditValue.ToString(),
                    IdTypes = (int)cbbType.EditValue,
                    Keyword = _keyword,
                    UserUpload = cbbUserUpload.EditValue.ToString(),
                    UploadDate = _dateUpload
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
                        string destFileName = Path.Combine(TPConfigs.Folder207, item.EncryptionName);
                        if (!Directory.Exists(TPConfigs.Folder207))
                            Directory.CreateDirectory(TPConfigs.Folder207);

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

                // Nếu chưa có lưu trình xử lý của văn kiện thì thêm mới (Trigger sẽ tự thêm vào ProcessInfo)
                if (!_IsProcessing)
                {
                    dt207_DocProcessing docProgress = new dt207_DocProcessing()
                    {
                        IdKnowledgeBase = _idBaseDocument,
                        IsComplete = false,
                        IsSuccess = false,
                        IdProgress = progressSelect.Id,
                        Descriptions = EnumHelper.GetDescription(_event207),
                        IdUserProcess = TPConfigs.LoginUser.Id,
                    };
                    dt207_DocProcessingBUS.Instance.Add(docProgress);
                }
                else
                {
                    dt207_DocProcessingInfo progressInfo = new dt207_DocProcessingInfo()
                    {
                        IdDocProgress = _idDocProcessing,
                        TimeStep = DateTime.Now,
                        IndexStep = 0,
                        IdUserProcess = TPConfigs.LoginUser.Id,
                        Descriptions = "呈核",
                    };
                    dt207_DocProcessingInfoBUS.Instance.Add(progressInfo);
                }
            }

            // Show a message box with the appropriate message and close the form
            XtraMessageBoxArgs args = new XtraMessageBoxArgs();
            args.AllowHtmlText = DefaultBoolean.True;

            args.Caption = TPConfigs.SoftNameTW;
            args.Text = $"<font='DFKai-SB' size=18>{EnumHelper.GetDescription(_event207)}！\r\n文件編號<color=red>「{_idBaseDocument}」</color></font>";
            args.Buttons = new DialogResult[] { DialogResult.OK };

            XtraMessageBox.Show(args);
            Close();
        }

        private void gcFiles_DoubleClick(object sender, EventArgs e)
        {
            if (permissionAttachments.ReadFile != true)
            {
                MsgTP.MsgNoPermission();
                return;
            }

            int focusRow = gvFiles.FocusedRowHandle;
            if (focusRow < 0)
                return;

            Attachments dataRow = gvFiles.GetRow(focusRow) as Attachments;
            string documentsFile = string.IsNullOrEmpty(dataRow.FullPath) ? Path.Combine(TPConfigs.Folder207, dataRow.EncryptionName) : dataRow.FullPath;

            // Lưu lại lịch sử xem file, Không lưu khi đang ký
            var IsProcessing = dt207_DocProcessingBUS.Instance.CheckItemProcessing(_idBaseDocument);
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
                MsgTP.MsgNoPermission();
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
            var lsBaseProgressById = dt207_DocProcessingBUS.Instance.GetListByIdBase(_idBaseDocument);
            int _idBaseProgress = lsBaseProgressById.OrderByDescending(r => r.Id).FirstOrDefault().Id;
            string _eventProcess = lsBaseProgressById.OrderByDescending(r => r.Id).FirstOrDefault().Descriptions;
            _eventApproved = EnumHelper.GetEnumByDescription<Event207DocInfo>(_eventProcess);

            var lsBaseProcessInfos = dt207_DocProcessingInfoBUS.Instance.GetListByIdDocProcess(_idBaseProgress);
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
                var docProcessUpdate = dt207_DocProcessingBUS.Instance.GetItemById(_idBaseProgress);// db.dt207_DocProgress.First(r => r.Id == idDocProgress);
                docProcessUpdate.IsSuccess = true;
                docProcessUpdate.IsComplete = true;
                docProcessUpdate.Change = string.Join("，", lsChangeDetails);
                dt207_DocProcessingBUS.Instance.AddOrUpdate(docProcessUpdate);
                //db.dt207_DocProgress.AddOrUpdate(docProcessUpdate);

                descriptions = "確認完畢";
                // Xoá dữ liệu cũ
                _dt207_Base_BAKBUS.RemoveRangeById(_idBaseDocument);
                _dt207_Attachment_BAKBUS.RemoveRangeByIdBase(_idBaseDocument);
                _dt207_Security_BAKBUS.RemoveRangeByIdBase(_idBaseDocument);
            }

            dt207_DocProcessingInfo progressInfo = new dt207_DocProcessingInfo()
            {
                IdDocProgress = _idBaseProgress,
                TimeStep = DateTime.Now,
                IndexStep = _indexStep,
                IdUserProcess = TPConfigs.LoginUser.Id,
                Descriptions = descriptions,
            };
            dt207_DocProcessingInfoBUS.Instance.Add(progressInfo);

            Close();
        }

        private void btnDisapprove_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XtraInputBoxArgs args = new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                Prompt = "退回文件原因",
                DefaultButtonIndex = 0,
                Editor = new MemoEdit(),
                DefaultResponse = ""
            };

            var result = XtraInputBox.Show(args);
            if (result == null) return;

            string descriptions = result?.ToString() ?? "";

            if (_eventApproved == Event207DocInfo.Delete)
            {
                var docProcessUpdate = dt207_DocProcessingBUS.Instance.GetItemById(_idDocProcessing);//  db.dt207_DocProgress.First(r => r.Id == _idDocProcessing);
                docProcessUpdate.IsSuccess = false;
                docProcessUpdate.IsComplete = true;
                dt207_DocProcessingBUS.Instance.AddOrUpdate(docProcessUpdate);
            }

            dt207_DocProcessingInfo progressInfo = new dt207_DocProcessingInfo
            {
                IdDocProgress = _idDocProcessing,
                TimeStep = DateTime.Now,
                IndexStep = -1,
                IdUserProcess = TPConfigs.LoginUser.Id,
                Descriptions = string.IsNullOrEmpty(descriptions) ? "退回" : $"退回，說明：{descriptions}"
            };

            dt207_DocProcessingInfoBUS.Instance.Add(progressInfo);

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

            var docProcessUpdate = dt207_DocProcessingBUS.Instance.GetItemById(_idDocProcessing);
            _eventApproved = EnumHelper.GetEnumByDescription<Event207DocInfo>(docProcessUpdate.Descriptions);

            docProcessUpdate.IsSuccess = false;
            docProcessUpdate.IsComplete = true;
            docProcessUpdate.Change = descriptions;
            dt207_DocProcessingBUS.Instance.AddOrUpdate(docProcessUpdate);

            dt207_DocProcessingInfo progressInfo = new dt207_DocProcessingInfo()
            {
                IdDocProgress = _idDocProcessing,
                TimeStep = DateTime.Now,
                IndexStep = -1,
                IdUserProcess = TPConfigs.LoginUser.Id,
                Descriptions = descriptions,
            };
            dt207_DocProcessingInfoBUS.Instance.Add(progressInfo);

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