using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraSplashScreen;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using KnowledgeSystem.Helpers;
using Newtonsoft.Json;
using Scriban;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;

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
        public int idBase302 = -1;
        string idDept2word;

        dt302_Base personBase;

        List<LayoutControlItem> lcControls;
        List<LayoutControlItem> lcImpControls;

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Disable;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void EnabledController(bool _enable = true)
        {
            cbbSupervisor.Enabled = _enable;
            txbSchool.Enabled = _enable;
            txbMajor.Enabled = _enable;
            // txbDateStart.Enabled = _enable;
        }

        private void LockControl()
        {
            txbUserId.Enabled = false;
            txbUserNameVN.Enabled = false;
            cbbDept.Enabled = false;
            cbbJobTitle.Enabled = false;
            txbDateStart.Enabled = false;

            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    Text = $"新增{formName}";

                    txbUserId.Enabled = true;

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                    EnabledController();
                    break;
                case EventFormInfo.Update:
                    Text = $"更新{formName}";

                    //txbUserId.Enabled = true;
                    //txbUserId.ReadOnly = true;

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

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

                    EnabledController(false);
                    break;
                default:
                    break;
            }

            foreach (var item in lcControls)
            {
                string colorHex = item.Control.Enabled ? "FFD700" : "FFFFFF";
                item.Text = item.Text.Replace("FFFFFF", colorHex);
            }

            // Các thông tin phải điền có thêm dấu * màu đỏ
            foreach (var item in lcImpControls)
            {
                if (item.Control.Enabled)
                {
                    item.Text += "<color=red>*</color>";
                }
                else
                {
                    item.Text = item.Text.Replace("<color=red>*</color>", "");
                }
            }


            //bool role301Main = AppPermission.Instance.CheckAppPermission(AppPermission.SafetyCertMain);
            //bool roleEditUserJobAndDept = AppPermission.Instance.CheckAppPermission(AppPermission.EditUserJobAndDept);

            //if (!(role301Main && roleEditUserJobAndDept && TPConfigs.IdParentControl == AppPermission.SafetyCertMain))
            //{
            //    btnPersonnelChanges.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

            //    btnSuspension.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            //    btnDeptChange.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            //    btnResign.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            //    btnResumeWork.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            //    btnJobChange.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            //}

            //Rectangle screenBounds = Screen.PrimaryScreen.Bounds;
            //int x = screenBounds.Width / 2 - Width / 2;
            //int y = screenBounds.Height / 2 - Height / 2;
            //Location = new Point(x, y);
        }

        private void f302_NewUserInfo_Load(object sender, EventArgs e)
        {
            idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);

            lcControls = new List<LayoutControlItem>() { lcUserID, lcUserName, lcDept, lcJob, lcSupervisor, lcSchool, lcMajor, lcEnterDate };
            lcImpControls = new List<LayoutControlItem>() { lcUserID, lcSupervisor, lcSchool, lcMajor };
            foreach (var item in lcControls)
            {
                item.AllowHtmlStringInCaption = true;
                item.Text = $"<color=#FFFFFF>{item.Text}</color>";
            }

            LockControl();

            var lsDepts = dm_DeptBUS.Instance.GetList().Where(r => r.Id.Length == 4 && r.Id.StartsWith(idDept2word))
                .Select(r => new dm_Departments { Id = r.Id, DisplayName = $"{r.Id,-5}{r.DisplayName}" }).ToList();
            cbbDept.Properties.DataSource = lsDepts;
            cbbDept.Properties.DisplayMember = "DisplayName";
            cbbDept.Properties.ValueMember = "Id";

            var lsJobTitles = dm_JobTitleBUS.Instance.GetList();
            cbbJobTitle.Properties.DataSource = lsJobTitles;
            cbbJobTitle.Properties.DisplayMember = "DisplayName";
            cbbJobTitle.Properties.ValueMember = "Id";

            var users = dm_UserBUS.Instance.GetListByDept(idDept2word).Where(r => r.Status == 0).ToList();
            cbbSupervisor.Properties.DataSource = users;
            cbbSupervisor.Properties.DisplayMember = "DisplayName";
            cbbSupervisor.Properties.ValueMember = "Id";

            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    personBase = new dt302_Base();
                    break;
                case EventFormInfo.View:
                    personBase = dt302_BaseBUS.Instance.GetItemById(idBase302);

                    cbbSupervisor.EditValue = personBase.Supervisor;
                    txbSchool.EditValue = personBase.School;
                    txbMajor.EditValue = personBase.Major;

                    var dmUsers = dm_UserBUS.Instance.GetItemById(personBase.IdUser);

                    txbUserId.EditValue = personBase.IdUser;
                    txbUserNameVN.EditValue = $"{dmUsers.DisplayName?.Trim()} {dmUsers.DisplayNameVN?.Trim()}";
                    cbbDept.EditValue = dmUsers.IdDepartment;
                    cbbJobTitle.EditValue = dmUsers.JobCode;
                    txbDateStart.EditValue = dmUsers.DateCreate;

                    break;
            }
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var result = false;
            string msg = "";
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                // Lấy các thông tin người dùng từ giao diện
                dm_User userInfo = new dm_User();

                userInfo.DisplayNameVN = txbUserNameVN.EditValue?.ToString().Trim();
                userInfo.IdDepartment = cbbDept.EditValue?.ToString();
                userInfo.JobCode = cbbJobTitle.EditValue?.ToString();
                userInfo.DateCreate = txbDateStart.DateTime;
                userInfo.Status = TPConfigs.lsUserStatus.FirstOrDefault(r => r.Value == cbbSupervisor.EditValue?.ToString()).Key;

                personBase.Supervisor = cbbSupervisor.EditValue?.ToString();
                personBase.School = txbSchool.EditValue?.ToString();
                personBase.Major = txbMajor.EditValue?.ToString();

                msg = $"{userInfo.Id} {userInfo.DisplayName} {userInfo.DisplayNameVN}";
                switch (eventInfo)
                {
                    case EventFormInfo.Create:

                        userInfo.Id = txbUserId.EditValue?.ToString();
                        personBase.IdUser = userInfo.Id;

                        //result = dm_UserBUS.Instance.Add(userInfo);
                        result = dt302_BaseBUS.Instance.Add(personBase);

                        break;
                    case EventFormInfo.View:
                        break;
                    case EventFormInfo.Update:

                        result = dt302_BaseBUS.Instance.AddOrUpdate(personBase);

                        break;
                    case EventFormInfo.Delete:

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

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            eventInfo = EventFormInfo.Update;
            LockControl();
        }

        private void txbUserId_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            var dmUsers = dm_UserBUS.Instance.GetItemById(txbUserId.EditValue?.ToString());
            if (dmUsers == null) return;

            txbUserNameVN.EditValue = $"{dmUsers.DisplayName?.Trim()} {dmUsers.DisplayNameVN?.Trim()}";
            cbbDept.EditValue = dmUsers.IdDepartment;
            cbbJobTitle.EditValue = dmUsers.JobCode;
            txbDateStart.EditValue = dmUsers.DateCreate;
        }

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var describe = XtraInputBox.Show(new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                Prompt = "請輸入停止培訓課程的原因",
                DefaultButtonIndex = 0,
                Editor = new MemoEdit(),
                DefaultResponse = ""
            })?.ToString() ?? "";

            if (string.IsNullOrEmpty(describe)) return;

            personBase.Describe = describe;
            var result = dt302_BaseBUS.Instance.AddOrUpdate(personBase);

            if (result)
            {
                Close();
            }
            else
            {
                MsgTP.MsgErrorDB();
            }
        }
    }
}