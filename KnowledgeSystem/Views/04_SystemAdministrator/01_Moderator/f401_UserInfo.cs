using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._04_SystemAdministrator._01_Moderator
{
    public partial class f401_UserInfo : DevExpress.XtraEditors.XtraForm
    {
        public f401_UserInfo()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public string formName = string.Empty;
        public EventFormInfo eventInfo = EventFormInfo.Create;
        public dm_User userInfo = null;
        private string oldUserInfoJson = "";
        string idDept2word;
        bool IsSysAdmin = false;

        private UpdateEvent _eventUpdate = UpdateEvent.Normal;

        List<dm_Role> lsAllRoles;
        List<dm_Role> lsChooseRoles = new List<dm_Role>();
        List<dt301_Course> lsCourses;

        BindingSource _sourceAllRole = new BindingSource();
        BindingSource _sourceChooseRole = new BindingSource();

        Font fontUI14 = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        Font fontUI12 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);

        private enum UpdateEvent
        {
            Normal,
            [Description("留職停薪")]
            Suspension,
            [Description("調至")]
            DeptChange,
            [Description("離職")]
            Resign,
            [Description("在職")]
            Conferred,
            [Description("晉升")]
            JobChange
        }

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;

            btnSuspension.ImageOptions.SvgImage = TPSvgimages.Suspension;
            btnDeptChange.ImageOptions.SvgImage = TPSvgimages.Transfer;
            btnResign.ImageOptions.SvgImage = TPSvgimages.Resign;
            btnConferred.ImageOptions.SvgImage = TPSvgimages.Conferred;
            btnJobChange.ImageOptions.SvgImage = TPSvgimages.UpLevel;
            btnPersonnelChanges.ImageOptions.SvgImage = TPSvgimages.PersonnelChanges;
        }

        private void EnabledController(bool _enable = true)
        {
            txbUserNameVN.Enabled = _enable;
            txbUserNameTW.Enabled = _enable;
            txbDOB.Enabled = _enable;
            txbCCCD.Enabled = _enable;
            cbbNationality.Enabled = _enable;
            cbbSex.Enabled = _enable;
            txbAddr.Enabled = _enable;
            txbPhone1.Enabled = _enable;
            txbPhone2.Enabled = _enable;
            txbDateStart.Enabled = _enable;
        }

        private void LockControl()
        {
            txbUserId.Enabled = false;
            txbUserId.ReadOnly = false;
            cbbDept.Enabled = false;
            cbbStatus.Enabled = false;
            cbbJobTitle.Enabled = false;

            btnDeptChange.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnResign.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnSuspension.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnConferred.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnJobChange.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnPersonnelChanges.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

            lcRole.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            Size = new Size(600, 358);

            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    Text = $"新增{formName}";

                    txbUserId.Enabled = true;
                    cbbDept.Enabled = true;
                    cbbStatus.Enabled = true;
                    cbbJobTitle.Enabled = true;

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                    EnabledController();
                    break;
                case EventFormInfo.Update:
                    Text = $"更新{formName}";

                    txbUserId.Enabled = true;
                    txbUserId.ReadOnly = true;

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                    if (IsSysAdmin)
                    {
                        lcRole.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        Size = new Size(600, 658);
                    }

                    EnabledController();
                    break;
                case EventFormInfo.Delete:
                    Text = $"刪除{formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                    EnabledController(false);
                    break;
                case EventFormInfo.View:
                    Text = $"{formName}信息";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

                    btnPersonnelChanges.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    if (userInfo.Status == 0)
                    {
                        btnSuspension.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                        btnDeptChange.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                        btnResign.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                        btnJobChange.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    }
                    else if (userInfo.Status == 2)
                    {
                        btnConferred.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    }
                    else
                    {
                        btnPersonnelChanges.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    }

                    EnabledController(false);
                    break;
                default:
                    break;
            }

            bool role301Main = AppPermission.Instance.CheckAppPermission(AppPermission.SafetyCertMain);
            bool roleEditUserJobAndDept = AppPermission.Instance.CheckAppPermission(AppPermission.EditUserJobAndDept);

            //if (!(role301Main && roleEditUserJobAndDept && TPConfigs.IdParentControl == AppPermission.SafetyCertMain))
            //{
            //    btnPersonnelChanges.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

            //    btnSuspension.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            //    btnDeptChange.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            //    btnResign.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            //    btnConferred.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            //    btnJobChange.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            //}

            Rectangle screenBounds = Screen.PrimaryScreen.Bounds;
            int x = screenBounds.Width / 2 - Width / 2;
            int y = screenBounds.Height / 2 - Height / 2;
            Location = new Point(x, y);
        }

        private void f401_UserInfo_Load(object sender, EventArgs e)
        {
            IsSysAdmin = AppPermission.Instance.CheckAppPermission(AppPermission.SysAdmin);

            LockControl();

            var lsDepts = dm_DeptBUS.Instance.GetList().Select(r => new dm_Departments { Id = r.Id, DisplayName = $"{r.Id,-5}{r.DisplayName}" }).ToList();
            cbbDept.Properties.DataSource = lsDepts;
            cbbDept.Properties.DisplayMember = "DisplayName";
            cbbDept.Properties.ValueMember = "Id";

            var lsJobTitles = dm_JobTitleBUS.Instance.GetList();
            cbbJobTitle.Properties.DataSource = lsJobTitles;
            cbbJobTitle.Properties.DisplayMember = "DisplayName";
            cbbJobTitle.Properties.ValueMember = "Id";

            cbbNationality.Properties.Items.AddRange(new string[] { "VN", "TW", "CN" });

            lsAllRoles = dm_RoleBUS.Instance.GetList();
            _sourceAllRole.DataSource = lsAllRoles;
            gcAllRole.DataSource = _sourceAllRole;

            cbbStatus.Properties.Items.AddRange(TPConfigs.lsUserStatus.Select(r => r.Value).ToList());
            cbbSex.Properties.Items.AddRange(new List<string>() { "男", "女" });

            _sourceChooseRole.DataSource = lsChooseRoles;
            gcChooseRole.DataSource = _sourceChooseRole;

            gvAllRole.ReadOnlyGridView();
            gvChooseRole.ReadOnlyGridView();

            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    userInfo = new dm_User();
                    break;
                case EventFormInfo.View:
                    userInfo.DisplayName = userInfo.DisplayName.Split('\n')[0];
                    userInfo.DateCreate = DateTime.Parse(userInfo.DateCreate.ToShortDateString());

                    txbUserId.EditValue = userInfo.Id;
                    txbUserNameVN.EditValue = userInfo.DisplayNameVN?.Trim();
                    txbUserNameTW.EditValue = userInfo.DisplayName.Split('\n')[0]?.Trim();
                    cbbDept.EditValue = userInfo.IdDepartment;
                    cbbJobTitle.EditValue = userInfo.JobCode;
                    txbDOB.EditValue = userInfo.DOB;
                    txbCCCD.EditValue = userInfo.CitizenID;
                    cbbNationality.EditValue = userInfo.Nationality;

                    txbPhone1.EditValue = userInfo.PhoneNum1;
                    txbPhone2.EditValue = userInfo.PhoneNum2;
                    txbAddr.EditValue = userInfo.Addr;
                    cbbSex.EditValue = userInfo.Sex == null ? "" : userInfo.Sex.Value ? "男" : "女";
                    cbbStatus.EditValue = userInfo.Status == null ? "" : TPConfigs.lsUserStatus[userInfo.Status.Value];
                    txbDateStart.EditValue = userInfo.DateCreate;

                    oldUserInfoJson = JsonConvert.SerializeObject(userInfo);

                    // Load Role
                    var lsUserRoles = dm_UserRoleBUS.Instance.GetListByUID(userInfo.Id).Select(r => r.IdRole).ToList();
                    lsChooseRoles.AddRange(lsAllRoles.Where(a => lsUserRoles.Exists(b => b == a.Id)));
                    lsAllRoles.RemoveAll(a => lsUserRoles.Exists(b => b == a.Id));

                    gcAllRole.RefreshDataSource();
                    gcChooseRole.RefreshDataSource();
                    break;
            }
        }

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            eventInfo = EventFormInfo.Update;
            LockControl();
        }

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MsgTP.MsgConfirmDel();

            eventInfo = EventFormInfo.Delete;
            LockControl();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var result = false;
            string msg = "";
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                userInfo.DisplayName = txbUserNameTW.EditValue?.ToString().Trim();
                userInfo.DisplayNameVN = txbUserNameVN.EditValue?.ToString().Trim();
                userInfo.IdDepartment = cbbDept.EditValue?.ToString();
                userInfo.JobCode = cbbJobTitle.EditValue?.ToString();
                userInfo.DOB = txbDOB.DateTime;
                userInfo.CitizenID = txbCCCD.EditValue?.ToString();
                userInfo.Nationality = cbbNationality.EditValue?.ToString();

                userInfo.DateCreate = txbDateStart.DateTime;
                userInfo.PhoneNum1 = txbPhone1.EditValue?.ToString();
                userInfo.PhoneNum2 = txbPhone2.EditValue?.ToString();
                userInfo.Addr = txbAddr.EditValue?.ToString();
                userInfo.Sex = cbbSex.EditValue?.ToString() == "男";
                userInfo.Status = TPConfigs.lsUserStatus.FirstOrDefault(r => r.Value == cbbStatus.EditValue?.ToString()).Key;

                string newUserInfoJson = JsonConvert.SerializeObject(userInfo);

                msg = $"{userInfo.Id} {userInfo.DisplayName} {userInfo.DisplayNameVN}";
                switch (eventInfo)
                {
                    case EventFormInfo.Create:
                        userInfo.Id = txbUserId.EditValue?.ToString();
                        result = dm_UserBUS.Instance.Add(userInfo);
                        break;
                    case EventFormInfo.View:
                        break;
                    case EventFormInfo.Update:
                        result = true;
                        var resultUpdate = false;

                        if (oldUserInfoJson != newUserInfoJson)
                        {
                            resultUpdate = dm_UserBUS.Instance.AddOrUpdate(userInfo);
                            result = !resultUpdate ? false : result;
                        }

                        // Xử lý cập nhật các thông tin đặc biệt có ảnh hưởng đến các thông tin khác như: Chứng chỉ an toàn 301
                        var lsCertToInValids = dt301_BaseBUS.Instance.GetListByUIDAndValidCert(userInfo.Id);
                        switch (_eventUpdate)
                        {
                            case UpdateEvent.Suspension:
                                foreach (var item in lsCertToInValids)
                                {
                                    item.ValidLicense = false;
                                    item.InvalidLicense = true;
                                    item.CertSuspended = true;
                                    item.Describe = EnumHelper.GetDescription(_eventUpdate);

                                    dt301_BaseBUS.Instance.AddOrUpdate(item);
                                }
                                break;
                            case UpdateEvent.Conferred:
                                var lsCertSuspendeds = dt301_BaseBUS.Instance.GetListByUIDAndCertSuspended(userInfo.Id);
                                foreach (var item in lsCertSuspendeds)
                                {
                                    item.ValidLicense = true;
                                    item.InvalidLicense = false;
                                    item.CertSuspended = false;
                                    item.Describe = "";

                                    dt301_BaseBUS.Instance.AddOrUpdate(item);
                                }
                                break;
                            case UpdateEvent.DeptChange:
                            case UpdateEvent.Resign:
                                foreach (var item in lsCertToInValids)
                                {
                                    item.ValidLicense = false;
                                    item.InvalidLicense = true;

                                    dt301_BaseBUS.Instance.AddOrUpdate(item);
                                }
                                break;
                            case UpdateEvent.JobChange:

                                var lsValidCerts = dt301_BaseBUS.Instance.GetListByUIDAndValidCert(userInfo.Id);
                                var lsCertReqSets = dt301_CertReqSetBUS.Instance.GetListByJobAndDept(userInfo.JobCode, idDept2word);

                                // Chuyển các chứng chỉ còn hạn về chức vụ mới
                                var lsNewValidCerts = (from data in lsValidCerts
                                                       join req in lsCertReqSets on data.IdCourse equals req.IdCourse
                                                       select data).ToList();

                                foreach (var item in lsNewValidCerts)
                                {
                                    item.IdJobTitle = userInfo.JobCode;
                                    dt301_BaseBUS.Instance.AddOrUpdate(item);
                                }

                                // Chuyển các chứ chỉ còn hạn của chức vụ cũ không cần ở chức vụ mới về 無效
                                var lsNewInvalidCerts = (from data in lsValidCerts
                                                         where !lsNewValidCerts.Contains(data)
                                                         select data).ToList();

                                foreach (dt301_Base item in lsNewInvalidCerts)
                                {
                                    item.ValidLicense = false;
                                    item.InvalidLicense = true;
                                    dt301_BaseBUS.Instance.AddOrUpdate(item);
                                }

                                break;
                        }

                        if (IsSysAdmin)
                        {
                            var resultDel = dm_UserRoleBUS.Instance.RemoveRangeByUID(userInfo.Id);

                            var lsUserRolesAdd = lsChooseRoles.Select(r => new dm_UserRole { IdUser = userInfo.Id, IdRole = r.Id }).ToList();
                            var resultAdd = dm_UserRoleBUS.Instance.AddRange(lsUserRolesAdd);
                        }
                        break;
                    case EventFormInfo.Delete:
                        var dialogResult = XtraMessageBox.Show($"您確認刪除人員: {userInfo.DisplayName}", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult != DialogResult.Yes) return;

                        result = dm_UserBUS.Instance.Remove(userInfo.Id);
                        dm_UserRoleBUS.Instance.RemoveRangeByUID(userInfo.Id);
                        break;
                    default:
                        break;
                }
            }

            if (result)
            {
                //switch (_eventInfo)
                //{
                //    case EventFormInfo.Update:
                //        logger.Info(_eventInfo.ToString(), msg);
                //        break;
                //    case EventFormInfo.Delete:
                //        logger.Warning(_eventInfo.ToString(), msg);
                //        break;
                //}
                Close();
            }
            else
            {
                MsgTP.MsgErrorDB();
            }
        }

        private void gvAllRole_DoubleClick(object sender, EventArgs e)
        {
            if (eventInfo != EventFormInfo.Update) return;

            GridView view = gvAllRole;
            dm_Role _role = view.GetRow(view.FocusedRowHandle) as dm_Role;

            lsAllRoles.Remove(_role);
            view.RefreshData();
            lsChooseRoles.Add(_role);
            gvChooseRole.RefreshData();
        }

        private void gvChooseRole_DoubleClick(object sender, EventArgs e)
        {
            if (eventInfo != EventFormInfo.Update) return;

            GridView view = gvChooseRole;
            dm_Role _role = view.GetRow(view.FocusedRowHandle) as dm_Role;

            lsChooseRoles.Remove(_role);
            view.RefreshData();
            lsAllRoles.Add(_role);
            gvAllRole.RefreshData();
        }

        private void txbUserId_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (TPConfigs.DomainComputer != DomainVNFPG.domainVNFPG || txbUserId.Enabled == false) return;

            string userNameByDomain = DomainVNFPG.Instance.GetAccountName(txbUserId.Text.ToUpper());
            if (string.IsNullOrEmpty(userNameByDomain)) return;

            string[] displayNameFHS = userNameByDomain.Split('/');
            cbbDept.EditValue = displayNameFHS[0].Replace("LG", string.Empty);
            txbUserNameTW.EditValue = displayNameFHS[1];
        }

        private void cbbDept_AutoSearch(object sender, DevExpress.XtraEditors.Controls.LookUpEditAutoSearchEventArgs e)
        {
            e.ClearHighlight();
        }

        private void btnSuspension_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            eventInfo = EventFormInfo.Update;
            _eventUpdate = UpdateEvent.Suspension;
            LockControl();

            cbbStatus.EditValue = TPConfigs.lsUserStatus[2];
        }

        private void btnConferred_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            eventInfo = EventFormInfo.Update;
            _eventUpdate = UpdateEvent.Conferred;
            LockControl();

            cbbStatus.EditValue = TPConfigs.lsUserStatus[0];
        }

        private void btnResign_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            eventInfo = EventFormInfo.Update;
            _eventUpdate = UpdateEvent.Resign;
            LockControl();

            cbbStatus.EditValue = TPConfigs.lsUserStatus[1];
        }

        private void btnJobChange_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XtraInputBoxArgs args = new XtraInputBoxArgs();

            args.AllowHtmlText = DevExpress.Utils.DefaultBoolean.True;
            args.Caption = "晉升";
            args.Prompt = $"<font='Microsoft JhengHei UI' size=14>請選職務</font>";
            args.DefaultButtonIndex = 0;
            SearchLookUpEdit editor = new SearchLookUpEdit();
            GridView editView = new GridView();
            GridColumn gcol1 = new GridColumn() { Caption = "職務代號", FieldName = "Id", Visible = true, VisibleIndex = 0 };
            GridColumn gcol2 = new GridColumn() { Caption = "職務名稱", FieldName = "DisplayName", Visible = true, VisibleIndex = 1 };

            var lsJobTitles = dm_JobTitleBUS.Instance.GetList().Where(r => r.Id != userInfo.JobCode).ToList();
            editor.Properties.DataSource = lsJobTitles;
            editor.Properties.DisplayMember = "DisplayName";
            editor.Properties.ValueMember = "Id";
            args.Editor = editor;

            editView.Appearance.HeaderPanel.Font = fontUI14;
            editView.Appearance.HeaderPanel.ForeColor = Color.Black;
            editView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            editView.Appearance.Row.Font = fontUI12;
            editView.Appearance.Row.ForeColor = Color.Black;
            editView.Columns.AddRange(new GridColumn[] { gcol1, gcol2 });

            editor.Properties.Appearance.Font = fontUI14;
            editor.Properties.Appearance.ForeColor = Color.Black;
            editor.Properties.NullText = "";
            editor.Properties.PopupView = editView;

            var result = XtraInputBox.Show(args);
            if (result == null) return;

            string idJobChange = result?.ToString() ?? "";

            cbbJobTitle.EditValue = idJobChange;

            eventInfo = EventFormInfo.Update;
            _eventUpdate = UpdateEvent.JobChange;
            LockControl();

            // Thông báo ra các chứng chỉ được giữ lại và chứng chỉ sẽ chuyển vào 無效 khi thay đổi chức vụ
            var lsValidCerts = dt301_BaseBUS.Instance.GetListByUIDAndValidCert(userInfo.Id);
            lsCourses = dt301_CourseBUS.Instance.GetList();

            idDept2word = userInfo.IdDepartment.Substring(0, 2);
            var lsCertReqSets = dt301_CertReqSetBUS.Instance.GetListByJobAndDept(idJobChange, idDept2word);

            // Lấy ds chứng chỉ còn được dùng đến ở chức vụ mới
            var lsNewValidCerts = (from data in lsValidCerts
                                   join req in lsCertReqSets on data.IdCourse equals req.IdCourse
                                   join course in lsCourses on data.IdCourse equals course.Id
                                   select $"{data.IdCourse} {course.DisplayName}").ToList();

            // Lấy ds các chứng chỉ ở chức vụ cũ còn hạn sẽ bị huỷ ở chức vụ mới
            var lsNewInvalidCerts = (from data in lsValidCerts
                                     join course in lsCourses on data.IdCourse equals course.Id
                                     where !lsNewValidCerts.Contains($"{data.IdCourse} {course.DisplayName}")
                                     select $"{data.IdCourse} {course.DisplayName}").ToList();

            string msgValidCert = $"<font='Microsoft JhengHei UI' size=14><color=blue>應取證照：</color></br>{string.Join("\r\n", lsNewValidCerts)}</font>";
            string msgInvalidCert = $"<font='Microsoft JhengHei UI' size=14><color=red>無效證照：</color></br>{string.Join("\r\n", lsNewInvalidCerts)}</font>";

            MsgTP.MsgShowInfomation(msgValidCert + "\r\n" + msgInvalidCert);
        }

        private void btnDeptChange_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XtraInputBoxArgs args = new XtraInputBoxArgs();

            args.AllowHtmlText = DevExpress.Utils.DefaultBoolean.True;
            args.Caption = "晉升";
            args.Prompt = $"<font='Microsoft JhengHei UI' size=14>請選單位</font>";
            args.DefaultButtonIndex = 0;
            SearchLookUpEdit editor = new SearchLookUpEdit();
            GridView editView = new GridView();
            GridColumn gcol1 = new GridColumn() { Caption = "部門代號", FieldName = "Id", Visible = true, VisibleIndex = 0 };
            GridColumn gcol2 = new GridColumn() { Caption = "部門名稱", FieldName = "DisplayName", Visible = true, VisibleIndex = 1 };

            var lsDepts = dm_DeptBUS.Instance.GetList().Select(r => new dm_Departments
            {
                Id = r.Id,
                DisplayName = $"{r.Id,-5}{r.DisplayName}"
            }).Where(r => r.Id != userInfo.IdDepartment && r.Id.Length == 4).ToList();

            editor.Properties.DataSource = lsDepts;
            editor.Properties.DisplayMember = "DisplayName";
            editor.Properties.ValueMember = "Id";
            args.Editor = editor;

            editor.Properties.Appearance.Font = fontUI14;
            editor.Properties.Appearance.ForeColor = Color.Black;
            editor.Properties.NullText = "";
            editor.Properties.PopupView = editView;

            editView.Appearance.HeaderPanel.Font = fontUI14;
            editView.Appearance.HeaderPanel.ForeColor = Color.Black;
            editView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            editView.Appearance.Row.Font = fontUI12;
            editView.Appearance.Row.ForeColor = Color.Black;
            editView.Columns.AddRange(new GridColumn[] { gcol1, gcol2 });

            var result = XtraInputBox.Show(args);
            if (result == null) return;

            string idDeptChange = result?.ToString() ?? "";

            cbbDept.EditValue = idDeptChange;

            eventInfo = EventFormInfo.Update;
            _eventUpdate = UpdateEvent.DeptChange;
            LockControl();
        }
    }
}