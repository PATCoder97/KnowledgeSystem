using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Html.Internal;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraSplashScreen;
using DocumentFormat.OpenXml.Spreadsheet;
using KnowledgeSystem.Helpers;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Crmf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using static DevExpress.Xpo.Helpers.CannotLoadObjectsHelper;
using static DevExpress.XtraEditors.Mask.MaskSettings;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace KnowledgeSystem.Views._03_DepartmentManage._07_Quiz
{
    public partial class f307_Interview_Info : DevExpress.XtraEditors.XtraForm
    {
        public f307_Interview_Info()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public EventFormInfo eventInfo = EventFormInfo.Create;
        public string formName = "";
        public string idBase = "";
        public string idDeptGetData = TPConfigs.LoginUser.IdDepartment;

        dt307_InterviewReport interviewReport;

        List<dm_User> interviewers = new List<dm_User>();
        List<dm_User> interviewees = new List<dm_User>();
        BindingSource sourceInterviewers = new BindingSource();
        BindingSource sourceInterviewees = new BindingSource();

        List<LayoutControlItem> lcControls;
        List<LayoutControlItem> lcImpControls;
        List<dm_User> users = new List<dm_User>();

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;

            btnAddInterviewer.ImageOptions.SvgImage = TPSvgimages.Add;
            btnAddInterviewee.ImageOptions.SvgImage = TPSvgimages.Add;
            btnDelInterviewer.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnDelInterviewee.ImageOptions.SvgImage = TPSvgimages.Remove;
        }

        private void EnabledController(bool _enable = true)
        {
            txbDisplayName.Enabled = _enable;
        }

        private void LockControl()
        {
            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    Text = $"新增{formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    EnabledController();
                    break;
                case EventFormInfo.Update:
                    Text = $"更新{formName}";

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
                string colorHex = item.Control.Enabled ? "000000" : "000000";
                item.Text = item.Text.Replace("000000", colorHex);
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
        }

        private void LoadData()
        {
            var jobs = dm_JobTitleBUS.Instance.GetList();
            var depts = dm_DeptBUS.Instance.GetList();

            var interviewerDatas = (from r in interviewers
                                    join job in jobs on r.ActualJobCode equals job.Id
                                    join dept in depts on r.IdDepartment equals dept.Id
                                    select new
                                    {
                                        usr = r,
                                        Id = r.Id,
                                        DisplayName = $"{r.DisplayName} {r.DisplayNameVN}",
                                        IdDepartment = $"{dept.Id}\r\n{dept.DisplayName}",
                                        JobCode = job.DisplayName,
                                    }).ToList();

            var intervieweeDatas = (from r in interviewees
                                    join job in jobs on r.ActualJobCode equals job.Id
                                    join dept in depts on r.IdDepartment equals dept.Id
                                    select new
                                    {
                                        usr = r,
                                        Id = r.Id,
                                        DisplayName = $"{r.DisplayName} {r.DisplayNameVN}",
                                        IdDepartment = $"{dept.Id}\r\n{dept.DisplayName}",
                                        JobCode = job.DisplayName,
                                    }).ToList();

            sourceInterviewers.DataSource = interviewerDatas;
            gvInterviewer.BestFitColumns();

            sourceInterviewees.DataSource = intervieweeDatas;
            gvInterviewee.BestFitColumns();

            gvInterviewer.RefreshData();
            gvInterviewee.RefreshData();
        }

        private void f307_Interview_Info_Load(object sender, EventArgs e)
        {
            lcControls = new List<LayoutControlItem>() { lcDisplayName };
            lcImpControls = new List<LayoutControlItem>() { lcDisplayName };
            foreach (var item in lcControls)
            {
                item.AllowHtmlStringInCaption = true;
                item.Text = $"<color=#000000>{item.Text}</color>";
            }

            gvInterviewer.ReadOnlyGridView();
            gvInterviewer.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvInterviewee.ReadOnlyGridView();
            gvInterviewee.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            gcInterviewer.DataSource = sourceInterviewers;
            gcInterviewee.DataSource = sourceInterviewees;

            switch (eventInfo)
            {
                case EventFormInfo.Create:

                    interviewReport = new dt307_InterviewReport();

                    break;
                case EventFormInfo.View:

                    users = dm_UserBUS.Instance.GetList();
                    interviewReport = dt307_InterviewReportBUS.Instance.GetItemById(idBase);

                    txbDisplayName.EditValue = interviewReport.DisplayName;

                    List<string> interviewers_id = JsonConvert.DeserializeObject<List<string>>(interviewReport.Interviewer);
                    interviewers = users.Where(r => interviewers_id.Any(x => x == r.Id)).ToList();

                    List<string> interviees_id = JsonConvert.DeserializeObject<List<string>>(interviewReport.Interviewees);
                    interviewees = users.Where(r => interviees_id.Any(x => x == r.Id)).ToList();

                    LoadData();

                    break;
                case EventFormInfo.Update:
                    break;
                case EventFormInfo.Delete:
                    break;
                case EventFormInfo.ViewOnly:
                    break;
                default:
                    break;
            }

            LockControl();
        }

        private void AddUsers(List<dm_User> target, bool isFullUser = false)
        {
            using (var fData = new f307_UsersData())
            {
                fData.UsersInput = target;
                fData.IsFullUser = isFullUser;
                fData.ShowDialog();

                if (fData.UsersOutput != null)
                {
                    target.AddRange(fData.UsersOutput);
                    LoadData();
                }
            }
        }

        private void RemoveUsers(List<dm_User> target, GridView grid)
        {
            var rows = grid.GetSelectedRows();
            var selectedUsers = new List<dm_User>();

            foreach (var rowHandle in rows)
            {
                var data = grid.GetRow(rowHandle);
                var usr = ((dynamic)data).usr as dm_User;

                if (usr != null)
                    selectedUsers.Add(usr);
            }

            target.RemoveAll(u => selectedUsers.Exists(r => r.Id == u.Id));
            LoadData();
        }
        private void btnAddInterviewer_Click(object sender, EventArgs e)
        {
            AddUsers(interviewers, true);
        }

        private void btnAddInterviewee_Click(object sender, EventArgs e)
        {
            AddUsers(interviewees);
        }

        private void btnDelInterviewer_Click(object sender, EventArgs e)
        {
            RemoveUsers(interviewers, gvInterviewer);
        }

        private void btnDelInterviewee_Click(object sender, EventArgs e)
        {
            RemoveUsers(interviewees, gvInterviewee);
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Kiểm tra xem đã điền đầy đủ thông tin yêu cầu hay chưa
            bool IsValidate = true;

            foreach (var item in lcImpControls)
            {
                if (item.Control is BaseEdit baseEdit)
                {
                    if (string.IsNullOrEmpty(baseEdit.EditValue?.ToString()))
                    {
                        IsValidate = false;
                        break; // Dừng vòng lặp ngay khi phát hiện lỗi
                    }
                }
            }

            if (!IsValidate)
            {
                MsgTP.MsgError("請填寫所有信息<color=red>(*)</color>");
                return;
            }

            var displayName = txbDisplayName.EditValue?.ToString();
            string interviewer_str = JsonConvert.SerializeObject(interviewers.Select(r => r.Id).ToList());
            string interviees_str = JsonConvert.SerializeObject(interviewees.Select(r => r.Id).ToList());

            var result = false;
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                interviewReport.DisplayName = displayName;
                interviewReport.Interviewer = interviewer_str;
                interviewReport.Interviewees = interviees_str;

                switch (eventInfo)
                {
                    case EventFormInfo.Create:

                        interviewReport.CreatedAt = DateTime.Now;
                        result = dt307_InterviewReportBUS.Instance.Add(interviewReport);

                        break;
                    case EventFormInfo.Update:

                        result = dt307_InterviewReportBUS.Instance.AddOrUpdate(interviewReport);

                        break;
                    case EventFormInfo.Delete:

                        var dialogResult = XtraMessageBox.Show($"您確認要刪除{formName}:\r\n{interviewReport.DisplayName}", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult != DialogResult.Yes) return;
                        result = dt307_InterviewReportBUS.Instance.RemoveById(interviewReport.Id);

                        break;
                    default:
                        break;
                }
            }

            if (result)
            {
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

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MsgTP.MsgConfirmDel();

            eventInfo = EventFormInfo.Delete;
            LockControl();
        }
    }
}