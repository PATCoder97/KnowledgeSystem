using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._02_NewPersonnel
{
    public partial class f302_NewUserInfo : DevExpress.XtraEditors.XtraForm
    {
        public f302_NewUserInfo()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public string formName = string.Empty;
        public EventFormInfo eventInfo = EventFormInfo.Create;
        //  public dm_User userInfo = null;
        private string oldUserInfoJson = "";
        string idDept2word;
        bool IsSysAdmin = false;

        List<dm_Role> lsAllRoles;
        List<dm_Role> lsChooseRoles = new List<dm_Role>();
        List<dt301_Course> lsCourses;

        BindingSource _sourceAllRole = new BindingSource();
        BindingSource _sourceChooseRole = new BindingSource();

        Font fontUI14 = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        Font fontUI12 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;

            btnSuspension.ImageOptions.SvgImage = TPSvgimages.Suspension;
            btnDeptChange.ImageOptions.SvgImage = TPSvgimages.Transfer;
            btnResign.ImageOptions.SvgImage = TPSvgimages.Resign;
            btnResumeWork.ImageOptions.SvgImage = TPSvgimages.Conferred;
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
            btnResumeWork.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
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
                    //if (userInfo.Status == 0)
                    //{
                    //    btnSuspension.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    //    btnDeptChange.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    //    btnResign.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    //    btnJobChange.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    //}
                    //else if (userInfo.Status == 2)
                    //{
                    //    btnResumeWork.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    //}
                    //else
                    //{
                    //    btnPersonnelChanges.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    //}

                    EnabledController(false);
                    break;
                default:
                    break;
            }

            bool role301Main = AppPermission.Instance.CheckAppPermission(AppPermission.SafetyCertMain);
            bool roleEditUserJobAndDept = AppPermission.Instance.CheckAppPermission(AppPermission.EditUserJobAndDept);

            if (!(role301Main && roleEditUserJobAndDept && TPConfigs.IdParentControl == AppPermission.SafetyCertMain))
            {
                btnPersonnelChanges.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                btnSuspension.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                btnDeptChange.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                btnResign.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                btnResumeWork.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                btnJobChange.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }

            Rectangle screenBounds = Screen.PrimaryScreen.Bounds;
            int x = screenBounds.Width / 2 - Width / 2;
            int y = screenBounds.Height / 2 - Height / 2;
            Location = new Point(x, y);
        }

        private void f302_NewUserInfo_Load(object sender, EventArgs e)
        {
            //IsSysAdmin = AppPermission.Instance.CheckAppPermission(AppPermission.SysAdmin);
            //lsCourses = dt301_CourseBUS.Instance.GetList();

            LockControl();

            var lsDepts = dm_DeptBUS.Instance.GetList().Where(r => r.Id.Length == 4)
                .Select(r => new dm_Departments { Id = r.Id, DisplayName = $"{r.Id,-5}{r.DisplayName}" }).ToList();
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
            gvAllRole.ReadOnlyGridView();

            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    //userInfo = new dm_User();
                    break;
                case EventFormInfo.View:
                    //userInfo.DisplayName = userInfo.DisplayName.Split('\n')[0];
                    //userInfo.DateCreate = DateTime.Parse(userInfo.DateCreate.ToShortDateString());

                    //txbUserId.EditValue = userInfo.Id;
                    //txbUserNameVN.EditValue = userInfo.DisplayNameVN?.Trim();
                    //txbUserNameTW.EditValue = userInfo.DisplayName.Split('\n')[0]?.Trim();
                    //cbbDept.EditValue = userInfo.IdDepartment;
                    //cbbJobTitle.EditValue = userInfo.JobCode;
                    //txbDOB.EditValue = userInfo.DOB;
                    //txbCCCD.EditValue = userInfo.CitizenID;
                    //cbbNationality.EditValue = userInfo.Nationality;

                    //txbPhone1.EditValue = userInfo.PhoneNum1;
                    //txbPhone2.EditValue = userInfo.PhoneNum2;
                    //txbAddr.EditValue = userInfo.Addr;
                    //cbbSex.EditValue = userInfo.Sex == null ? "" : userInfo.Sex.Value ? "男" : "女";
                    //cbbStatus.EditValue = userInfo.Status == null ? "" : TPConfigs.lsUserStatus[userInfo.Status.Value];
                    //txbDateStart.EditValue = userInfo.DateCreate;

                    //oldUserInfoJson = JsonConvert.SerializeObject(userInfo);
                    //idDept2word = userInfo.IdDepartment.Substring(0, 2);

                    //// Lấy quyền hạn và chuyển các quyền mà user có sang gcChooseRoles
                    //var lsUserRoles = dm_UserRoleBUS.Instance.GetListByUID(userInfo.Id).Select(r => r.IdRole).ToList();
                    //lsChooseRoles.AddRange(lsAllRoles.Where(a => lsUserRoles.Exists(b => b == a.Id)));
                    //lsAllRoles.RemoveAll(a => lsUserRoles.Exists(b => b == a.Id));

                    //gcAllRole.RefreshDataSource();
                    //gcChooseRole.RefreshDataSource();
                    break;
            }
        }
    }
}