using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraSplashScreen;
using DocumentFormat.OpenXml.Spreadsheet;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._03_DepartmentManage._07_Quiz;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using static DevExpress.Xpo.Helpers.CannotLoadObjectsHelper;

namespace KnowledgeSystem.Views._03_DepartmentManage._08_HealthCheck
{
    public partial class f308_CheckSession_Info : DevExpress.XtraEditors.XtraForm
    {
        public f308_CheckSession_Info()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public EventFormInfo eventInfo = EventFormInfo.Create;
        public string formName = "";
        public int idSession = -1;
        string idDept2word = TPConfigs.idDept2word;
        dt308_CheckSession dt308CheckSession;

        List<LayoutControlItem> lcControls;
        List<LayoutControlItem> lcImpControls;

        List<dm_User> usrs = new List<dm_User>();
        List<dm_User> oldUsrs = new List<dm_User>();
        BindingSource sourceUser = new BindingSource();

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void EnabledController(bool _enable = true)
        {
            txbNameVN.Enabled = _enable;
            txbNameTW.Enabled = _enable;
            cbbCheckType.Enabled = _enable;

            btnAddUser.Enabled = _enable;
            btnDelUser.Enabled = _enable;
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

        private void f308_CheckSession_Info_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            lcControls = new List<LayoutControlItem>() { lcNameVN, lcNameTW, lcCheckType };
            lcImpControls = new List<LayoutControlItem>() { lcNameVN, lcNameTW, lcCheckType };

            foreach (var item in lcControls)
            {
                item.AllowHtmlStringInCaption = true;
                item.Text = $"<color=#000000>{item.Text}</color>";
            }

            var checkTypes = new string[] { "Thông thường/一般健康檢查", "Đặc biệt/特殊健檢", "Nhân viên mới/新進人員檢查", "Trước bố trí việc làm/工作安排前健康檢查" };
            cbbCheckType.Properties.Items.AddRange(checkTypes);

            gcData.DataSource = sourceUser;

            switch (eventInfo)
            {
                case EventFormInfo.Create:

                    dt308CheckSession = new dt308_CheckSession();

                    break;
                case EventFormInfo.View:

                    dt308CheckSession = dt308_CheckSessionBUS.Instance.GetItemById(idSession);

                    txbNameTW.EditValue = dt308CheckSession.DisplayNameTW;
                    txbNameVN.EditValue = dt308CheckSession.DisplayNameVN;
                    cbbCheckType.EditValue = dt308CheckSession.CheckType;

                    usrs = dm_UserBUS.Instance.GetList();
                    var details = dt308_CheckDetailBUS.Instance.GetListByIdSession(idSession);

                    // Lấy danh sách nhân viên là dùng tạm IP để làm mốc là nhân viên cũ (từ cơ sở dữ liệu lên)
                    usrs = usrs.Where(u => details.Select(d => d.EmpId).Contains(u.Id)).Select(u => { u.IPAddress = "1"; return u; }).ToList();
                    oldUsrs = usrs.Where(u => details.Select(d => d.EmpId).Contains(u.Id)).Select(u => { u.IPAddress = "1"; return u; }).ToList();

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

            sourceUser.DataSource = usrs;
            gvData.BestFitColumns();
            gvData.RefreshData();

            LockControl();
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            f308_UserData fData = new f308_UserData();
            fData.UsersInput = usrs;
            fData.ShowDialog();

            if (fData.UsersOutput == null) return;

            usrs.AddRange(fData.UsersOutput);

            gvData.RefreshData();
        }

        private void btnDelUser_Click(object sender, EventArgs e)
        {
            gvData.GetSelectedRows()
                .Select(r => gvData.GetRow(r) as dm_User)
                .Where(u => u != null)
                .ToList()
                .ForEach(u => usrs.Remove(u));

            gvData.ClearSelection();
            gvData.RefreshData();
        }

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            eventInfo = EventFormInfo.Update;
            LockControl();
        }

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Kiểm tra xem đã điền đầy đủ thông tin yêu cầu hay chưa
            bool IsValidate = true;
            foreach (var item in lcImpControls)
            {
                if (string.IsNullOrEmpty(item.Control.Text)) IsValidate = false;
            }

            if (!IsValidate)
            {
                MsgTP.MsgError("請填寫所有信息<color=red>(*)</color>");
                return;
            }

            var dateUpload = DateTime.Today;
            var nameVN = txbNameVN.Text.Trim();
            var nameTW = txbNameTW.Text.Trim();
            var checkType = cbbCheckType.Text;

            if (StringHelper.CheckUpcase(nameVN, 33) && nameVN.Length > 20)
            {
                XtraMessageBox.Show("Tắt CAPSLOCK đi！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var result = false;
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                dt308CheckSession.DateSession = dateUpload;
                dt308CheckSession.DisplayNameVN = nameVN;
                dt308CheckSession.DisplayNameTW = nameTW;
                dt308CheckSession.CheckType = checkType;

                switch (eventInfo)
                {
                    case EventFormInfo.Create:

                        int idDt204Base = dt308_CheckSessionBUS.Instance.Add(dt308CheckSession);
                        result = idDt204Base != -1;

                        var dt308CheckDetail = usrs.Select(r => new dt308_CheckDetail()
                        {
                            SessionId = idDt204Base,
                            EmpId = r.Id,
                            HealthRating = -1
                        }).ToList();

                        dt308_CheckDetailBUS.Instance.AddRange(dt308CheckDetail);

                        break;
                    case EventFormInfo.Update:

                         result = dt308_CheckSessionBUS.Instance.AddOrUpdate(dt308CheckSession);

                        // So sánh sự khác biệt dựa trên Id và điều kiện IPAddress
                        var addedUsers = usrs.Where(u => !oldUsrs.Any(o => o.Id == u.Id) && u.IPAddress != "1").ToList();
                        var removedUsers = oldUsrs.Where(o => !usrs.Any(u => u.Id == o.Id)).ToList();

                        // Thêm mới
                        foreach (var item in addedUsers)
                        {
                            dt308_CheckDetailBUS.Instance.Add(new dt308_CheckDetail()
                            {
                                SessionId = idSession,
                                EmpId = item.Id,
                                HealthRating = -1
                            });
                        }

                        // Xóa
                        foreach (var item in removedUsers)
                        {
                            dt308_CheckDetailBUS.Instance.RemoveBySessionAndEmp(idSession, item.Id);
                        }

                        break;
                    case EventFormInfo.Delete:

                        //var dialogResult = XtraMessageBox.Show($"您確認要刪除{formName}: {dt204Base.Code} {dt204Base.DisplayName}", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        //if (dialogResult != DialogResult.Yes) return;
                        //result = dt204_InternalDocMgmtBUS.Instance.RemoveById(dt204Base.Id, TPConfigs.LoginUser.Id);

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
    }
}