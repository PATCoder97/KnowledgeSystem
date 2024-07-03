using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Items.ViewInfo;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._04_BorrVehicle
{
    public partial class uc304_BorrVehicleMain : DevExpress.XtraEditors.XtraUserControl
    {
        public uc304_BorrVehicleMain()
        {
            InitializeComponent();
            InitializeMenuItems();
        }

        DXMenuItem itemViewInfo;
        DXMenuItem itemBorrVehicle;
        DXMenuItem itemBackVehicle;

        List<dm_DrivingLic> drivingLics;
        List<dm_DrivingLic> carDrivingLics;

        public dm_User borrUsr = TPConfigs.LoginUser;

        private void InitializeMenuItems()
        {
            itemViewInfo = new DXMenuItem("Lịch sử mượn", ItemViewInfo_Click, TPSvgimages.Info, DXMenuItemPriority.Normal);
            itemBorrVehicle = new DXMenuItem("Mượn xe", ItemBorrVehicle_Click, TPSvgimages.Add, DXMenuItemPriority.Normal);
            itemBackVehicle = new DXMenuItem("Trả xe", ItemBackVehicle_Click, TPSvgimages.Confirm, DXMenuItemPriority.Normal);

            itemViewInfo.ImageOptions.SvgImageSize = new Size(24, 24);
            itemViewInfo.AppearanceHovered.ForeColor = Color.Blue;

            itemBorrVehicle.ImageOptions.SvgImageSize = new Size(24, 24);
            itemBorrVehicle.AppearanceHovered.ForeColor = Color.Blue;

            itemBackVehicle.ImageOptions.SvgImageSize = new Size(24, 24);
            itemBackVehicle.AppearanceHovered.ForeColor = Color.Blue;
        }

        private void ItemBackVehicle_Click(object sender, EventArgs e)
        {
            VehicleStatus status = gvVehicleStatus.GetRow(gvVehicleStatus.FocusedRowHandle) as VehicleStatus;

            f304_BorrVehicleInfo frm = new f304_BorrVehicleInfo();
            frm.eventInfo = EventFormInfo.Update;
            frm.indexTypeVehicle = cbbTypeVehicle.SelectedIndex;
            frm.vehicleStatus = status;
            frm.borrTime = status.BorrTime;
            frm.borrUsr = borrUsr;
            frm.ShowDialog();

            LoadDataVehicle();
        }

        private void ItemBorrVehicle_Click(object sender, EventArgs e)
        {
            VehicleStatus status = gvVehicleStatus.GetRow(gvVehicleStatus.FocusedRowHandle) as VehicleStatus;

            f304_BorrVehicleInfo frm = new f304_BorrVehicleInfo();
            frm.indexTypeVehicle = cbbTypeVehicle.SelectedIndex;
            frm.vehicleStatus = status;
            frm.licExpDate = carDrivingLics.FirstOrDefault()?.Exprires.ToString("yyyyMMdd") ?? "";
            frm.borrUsr = borrUsr;

            frm.ShowDialog();

            LoadDataVehicle();
        }

        private async void LoadDataVehicle()
        {
            List<VehicleStatus> vehicleStatuses = new List<VehicleStatus>();

            switch (cbbTypeVehicle.SelectedIndex)
            {
                case 0:
                    vehicleStatuses = await BorrVehicleHelper.Instance.GetListMotor();
                    for (int i = 0; i < vehicleStatuses.Count; i++)
                    {
                        if (!vehicleStatuses[i].CanBorr)
                        {
                            var data = await BorrVehicleHelper.Instance.GetBorrMotorUser(vehicleStatuses[i]);

                            vehicleStatuses[i].IdUserBorr = data.FirstOrDefault().IdUserBorr;
                            vehicleStatuses[i].NameUserBorr = data.FirstOrDefault().NameUserBorr;
                            vehicleStatuses[i].BorrTime = data.FirstOrDefault().BorrTime.ToString("yyyy/MM/dd HH:mm");
                        }
                    }
                    break;
                case 1:
                    vehicleStatuses = await BorrVehicleHelper.Instance.GetListCar();
                    for (int i = 0; i < vehicleStatuses.Count; i++)
                    {
                        if (!vehicleStatuses[i].CanBorr)
                        {
                            var data = await BorrVehicleHelper.Instance.GetBorrCarUser(vehicleStatuses[i]);

                            vehicleStatuses[i].IdUserBorr = data.FirstOrDefault().IdUserBorr;
                            vehicleStatuses[i].NameUserBorr = data.FirstOrDefault().NameUserBorr;
                            vehicleStatuses[i].BorrTime = data.FirstOrDefault().BorrTime.ToString("yyyy/MM/dd HH:mm");
                        }
                    }
                    break;
            }

            gcInfo.DataSource = null;
            gcVehicleStatus.DataSource = vehicleStatuses;
            gvVehicleStatus.BestFitColumns();
        }

        private async void ItemViewInfo_Click(object sender, EventArgs e)
        {
            VehicleStatus status = gvVehicleStatus.GetRow(gvVehicleStatus.FocusedRowHandle) as VehicleStatus;
            List<VehicleBorrInfo> dataInfos = new List<VehicleBorrInfo>();

            switch (cbbTypeVehicle.SelectedIndex)
            {
                case 0:
                    dataInfos = await BorrVehicleHelper.Instance.GetBorrMotorUser(status);
                    break;
                case 1:
                    dataInfos = await BorrVehicleHelper.Instance.GetBorrCarUser(status);
                    break;
            }

            gcInfo.DataSource = dataInfos;
            gvInfo.BestFitColumns();
        }

        private void uc304_BorrVehicleMain_Load(object sender, EventArgs e)
        {
            lcChangeUser.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            Font fontUI12 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI12;

            gvVehicleStatus.ReadOnlyGridView();
            gvVehicleStatus.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            gvInfo.ReadOnlyGridView();
            gvInfo.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            cbbTypeVehicle.Properties.Items.AddRange(TPConfigs.typeVehicles);

            drivingLics = dm_DrivingLicBUS.Instance.GetList().Where(r => r.UserID == borrUsr.Id).ToList();
            carDrivingLics = drivingLics.Where(r => !r.Class.StartsWith("A")).ToList();

            bool roleChangeUsr = AppPermission.Instance.CheckAppPermission(AppPermission.ChangeUser304);
            if (roleChangeUsr)
            {
                lcChangeUser.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                btnChangeUsr.Text = borrUsr.DisplayName;
            }
        }

        private void cbbTypeVehicle_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDataVehicle();
        }

        private void gvVehicleStatus_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.RowHandle >= 0)
            {
                GridView view = sender as GridView;
                view.FocusedRowHandle = e.HitInfo.RowHandle;

                var status = view.GetRow(view.FocusedRowHandle) as VehicleStatus;

                e.Menu.Items.Add(itemViewInfo);

                bool IsUserBorr = !string.IsNullOrWhiteSpace(status.IdUserBorr) && status.IdUserBorr == borrUsr.Id;
                bool CanBorr = string.IsNullOrWhiteSpace(status.IdUserBorr);
                bool CheckDrivingLic = cbbTypeVehicle.SelectedIndex == 1 && carDrivingLics.Count > 0 || cbbTypeVehicle.SelectedIndex == 0;

                if (IsUserBorr)
                {
                    e.Menu.Items.Add(itemBackVehicle);
                }
                else if (CanBorr && CheckDrivingLic)
                {
                    e.Menu.Items.Add(itemBorrVehicle);
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadDataVehicle();
        }

        private void btnChangeUsr_Click(object sender, EventArgs e)
        {
            uc304_ChangeUsr ucChangeUsr = new uc304_ChangeUsr();
            if (XtraDialog.Show(ucChangeUsr, "Chọn nhân viên", MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                return;
            }

            if (ucChangeUsr.IdSelectUser == null)
            {
                return;
            }

            borrUsr = dm_UserBUS.Instance.GetItemById(ucChangeUsr.IdSelectUser);
            btnChangeUsr.Text = borrUsr.DisplayName;

            drivingLics = dm_DrivingLicBUS.Instance.GetList().Where(r => r.UserID == borrUsr.Id).ToList();
            carDrivingLics = drivingLics.Where(r => !r.Class.StartsWith("A")).ToList();
        }
    }
}
