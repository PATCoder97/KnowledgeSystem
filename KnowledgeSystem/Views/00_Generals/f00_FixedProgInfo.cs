using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using DocumentFormat.OpenXml.Spreadsheet;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._00_Generals
{
    public partial class f00_FixedProgInfo : DevExpress.XtraEditors.XtraForm
    {
        public f00_FixedProgInfo()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public EventFormInfo eventInfo = EventFormInfo.Create;
        public string formName;
        public dm_FixedProgress prog = null;
        string idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);

        BindingSource sourceProgresses = new BindingSource();
        List<ProgressDetail> progresses = new List<ProgressDetail>();

        List<dm_User> users;
        List<dm_JobTitle> jobTitles;

        private class ProgressDetail
        {
            public string IdUsr { get; set; }
            public string UserName { get; set; }
            public string JobName { get; set; }
        }

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void EnabledController(bool _enable = true)
        {
            // Kiểm tra xem phải sysAdmin không
            txbOwner.Enabled = false;
            bool IsSysAdmin = AppPermission.Instance.CheckAppPermission(AppPermission.SysAdmin);
            if (IsSysAdmin)
            {
                txbOwner.Enabled = _enable;
            }
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
                    gcProgress.Enabled = true;

                    EnabledController();
                    break;
                case EventFormInfo.Update:
                    Text = $"更新{formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    gcProgress.Enabled = true;

                    EnabledController();
                    break;
                case EventFormInfo.Delete:
                    Text = $"刪除{formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    gcProgress.Enabled = false;

                    EnabledController(false);
                    break;
                case EventFormInfo.View:
                    Text = $"{formName}信息";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    gcProgress.Enabled = false;

                    EnabledController(false);
                    break;
                default:
                    break;
            }
        }

        private bool ValidateData()
        {
            bool IsOK = true;
            if (string.IsNullOrEmpty(txbOwner.EditValue?.ToString()))
            {
                IsOK = false;
            }

            if (string.IsNullOrEmpty(txbDisplayName.EditValue?.ToString()))
            {
                IsOK = false;
            }

            if (progresses.Any(r => string.IsNullOrEmpty(r.IdUsr)) || progresses.Count() == 0)
            {
                IsOK = false;
            }

            return IsOK;
        }

        private void f00_FixedProgInfo_Load(object sender, EventArgs e)
        {
            LockControl();

            var usrs = dm_UserBUS.Instance.GetList().Where(r => r.Status == 0).ToList();
            txbOwner.Properties.DataSource = usrs;
            txbOwner.Properties.DisplayMember = "DisplayName";
            txbOwner.Properties.ValueMember = "Id";
            txbOwner.EditValue = TPConfigs.LoginUser.Id;

            users = dm_UserBUS.Instance.GetListByDept(idDept2word).Where(r => r.Status == 0).ToList();
            jobTitles = dm_JobTitleBUS.Instance.GetList();

            var dmType = dt306_TypeBUS.Instance.GetList();

            // Gắn các thông số cho các combobox
            lookupUser.ValueMember = "Id";
            lookupUser.DisplayMember = "Id";
            lookupUser.PopupView.Columns.AddRange(new[]
            {
                new GridColumn { FieldName = "IdDepartment", VisibleIndex = 0, Caption = "單位", MinWidth = 50, Width = 50 },
                new GridColumn { FieldName = "Id", VisibleIndex = 0, Caption = "工號", MinWidth = 120, Width = 120 },
                new GridColumn { FieldName = "DisplayName", VisibleIndex = 2, Caption = "名稱", MinWidth = 100, Width = 150 },
            });
            lookupUser.DataSource = users;

            sourceProgresses.DataSource = progresses;
            gcProgress.DataSource = sourceProgresses;

            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    prog = new dm_FixedProgress();
                    break;
                case EventFormInfo.View:
                    txbOwner.EditValue = prog.Owner;
                    txbDisplayName.EditValue = prog.DisplayName;

                    string[] usrProg = prog.Progress.Split(';');
                    foreach (var item in usrProg)
                    {
                        var usrInfo = users.FirstOrDefault(r => r.Id == item?.ToString());
                        if (usrInfo == null) continue;

                        string nameUser = usrInfo?.DisplayName ?? "";
                        string jobName = jobTitles.FirstOrDefault(r => r.Id == usrInfo.JobCode)?.DisplayName ?? "";
                        progresses.Add(new ProgressDetail() { IdUsr = item, JobName = jobName, UserName = nameUser });
                    }

                    break;
                case EventFormInfo.Update:
                    break;
                case EventFormInfo.Delete:
                    break;
                default:
                    break;
            }
        }

        private void gvProgress_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            if (e.Column.FieldName != "IdUsr") return;

            var usrInfo = users.FirstOrDefault(r => r.Id == e.Value?.ToString());
            string nameUser = usrInfo?.DisplayName ?? "";
            string jobName = jobTitles.FirstOrDefault(r => r.Id == usrInfo.JobCode)?.DisplayName ?? "";
            view.SetRowCellValue(e.RowHandle, view.Columns["UserName"], nameUser);
            view.SetRowCellValue(e.RowHandle, view.Columns["JobName"], jobName);
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Đưa focused ra khỏi bảng để cập nhật lên source
            gvProgress.FocusedRowHandle = GridControl.AutoFilterRowHandle;

            bool IsValidate = ValidateData();
            if (!IsValidate) return;


            var result = false;
            string msg = "";
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                prog.Owner = txbOwner.EditValue?.ToString();
                prog.DisplayName = txbDisplayName.Text.Trim();
                prog.Progress = string.Join(";", progresses.Select(r => r.IdUsr));

                msg = $"{prog.Owner} {prog.DisplayName} {prog.Progress}";
                switch (eventInfo)
                {
                    case EventFormInfo.Create:
                    case EventFormInfo.Update:
                        result = dm_FixedProgressBUS.Instance.AddOrUpdate(prog);
                        break;
                    case EventFormInfo.View:
                        break;
                    case EventFormInfo.Delete:
                        var dialogResult = XtraMessageBox.Show($"你確定刪除流程: {prog.DisplayName}？", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult != DialogResult.Yes) return;

                        var progRemove = dm_FixedProgressBUS.Instance.RemoveById(prog.Id);
                        result = progRemove != null;
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

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MsgTP.MsgConfirmDel();

            eventInfo = EventFormInfo.Delete;
            LockControl();
        }
    }
}