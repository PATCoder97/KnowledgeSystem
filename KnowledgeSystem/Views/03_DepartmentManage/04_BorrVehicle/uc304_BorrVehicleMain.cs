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
using System.Xml.Linq;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;

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
        DXMenuItem itemAdBackVehicle;
        DXMenuItem itemEditInfo;

        List<dm_DrivingLic> drivingLics;
        List<dm_DrivingLic> carDrivingLics;

        public dm_User borrUsr = TPConfigs.LoginUser;

        bool roleAdminBorrVehicle = false;

        private void InitializeMenuItems()
        {
            itemViewInfo = new DXMenuItem("Lịch sử mượn", ItemViewInfo_Click, TPSvgimages.Info, DXMenuItemPriority.Normal);
            itemBorrVehicle = new DXMenuItem("Mượn xe", ItemBorrVehicle_Click, TPSvgimages.Add, DXMenuItemPriority.Normal);
            itemBackVehicle = new DXMenuItem("Trả xe", ItemBackVehicle_Click, TPSvgimages.Confirm, DXMenuItemPriority.Normal);
            itemAdBackVehicle = new DXMenuItem("Trả xe", ItemAdackVehicle_Click, TPSvgimages.Confirm, DXMenuItemPriority.Normal);
            itemEditInfo = new DXMenuItem("Cập nhật thông tin", ItemEditInfo_Click, TPSvgimages.Edit, DXMenuItemPriority.High);

            itemViewInfo.ImageOptions.SvgImageSize = new Size(24, 24);
            itemViewInfo.AppearanceHovered.ForeColor = Color.Blue;

            itemBorrVehicle.ImageOptions.SvgImageSize = new Size(24, 24);
            itemBorrVehicle.AppearanceHovered.ForeColor = Color.Blue;

            itemBackVehicle.ImageOptions.SvgImageSize = new Size(24, 24);
            itemBackVehicle.AppearanceHovered.ForeColor = Color.Blue;

            itemAdBackVehicle.ImageOptions.SvgImageSize = new Size(24, 24);
            itemAdBackVehicle.AppearanceHovered.ForeColor = Color.Blue;

            itemEditInfo.ImageOptions.SvgImageSize = new Size(24, 24);
            itemEditInfo.AppearanceHovered.ForeColor = Color.Blue;
        }

        private async void ItemEditInfo_Click(object sender, EventArgs e)
        {
            VehicleBorrInfo info = gvInfo.GetRow(gvInfo.FocusedRowHandle) as VehicleBorrInfo;

            uc304_EditInfo ucEditInfo = new uc304_EditInfo() { StartKm = info.StartKm.ToString(), EndKm = info.EndKm.ToString() };
            if (XtraDialog.Show(ucEditInfo, "Cập nhật thông tin", MessageBoxButtons.OKCancel) != DialogResult.OK)
                return;

            int startKm = Convert.ToInt32(ucEditInfo.StartKm);
            int endKm = Convert.ToInt32(ucEditInfo.EndKm);
            int totalKm = endKm - startKm;

            info.StartKm = startKm;
            info.EndKm = endKm;
            info.TotalKm = totalKm;

            switch (cbbTypeVehicle.SelectedIndex)
            {
                case 0:
                    if (totalKm < 0 || totalKm > 15)
                    {
                        XtraMessageBox.Show($"Xe máy mỗi chuyến chỉ được đi 0-15km", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    await BorrVehicleHelper.Instance.EditInfoMoto(info);
                    break;
                case 1:
                    var result = await BorrVehicleHelper.Instance.EditInfoCar(info);
                    break;
            }

            LoadDataVehicle();
        }

        private async void ItemAdackVehicle_Click(object sender, EventArgs e)
        {
            VehicleBorrInfo info = gvInfo.GetRow(gvInfo.FocusedRowHandle) as VehicleBorrInfo;

            uc304_BackVehicle ucAdBackVehicle = new uc304_BackVehicle();
            if (XtraDialog.Show(ucAdBackVehicle, "Thông tin trả xe", MessageBoxButtons.OKCancel) != DialogResult.OK)
                return;

            DateTimeOffset borrTime = info.BorrTime;
            DateTimeOffset backTime = ucAdBackVehicle.BackTime;

            string nameVehicle = info.VehicleName;
            string backTimeStr = backTime.ToString("yyyyMMddHHmm");
            int startKm = info.StartKm;
            int endKm = Convert.ToInt32(ucAdBackVehicle.EndKm);
            int totalKm = endKm - startKm;

            bool result = false;

            if (totalKm < 0) return;

            if (borrTime > backTime)
            {
                XtraMessageBox.Show("Sao? Con lợn nhựa này, mày đi lùi thời gian à!", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            // Kiểm tra nếu chênh lệch lớn hơn 2 giờ
            TimeSpan difference = backTime - borrTime;
            if (difference.TotalHours > 2)
            {
                XtraMessageBox.Show("Trả xe trong vòng 2 tiếng nhé con lợn nhựa!", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            if (totalKm > 15)
            {
                XtraMessageBox.Show($"Xe máy mỗi chuyến chỉ được đi dưới 15km", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (XtraMessageBox.Show($"Bạn chắc chắn muốn trả xe: {nameVehicle}, với {totalKm} Km ?", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }

            string formattedBorrTime = info.BorrTime.ToString("yyyyMMddHHmm");

            var usrAdBack = dm_UserBUS.Instance.GetItemById(info.IdUserBorr);

            result = await BorrVehicleHelper.Instance.BackMotor(usrAdBack, nameVehicle, endKm, formattedBorrTime, backTimeStr, totalKm);

            LoadDataVehicle();
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

        private async void ItemBorrVehicle_Click(object sender, EventArgs e)
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

            f304_BorrVehicleInfo frm = new f304_BorrVehicleInfo();
            frm.indexTypeVehicle = cbbTypeVehicle.SelectedIndex;
            frm.vehicleStatus = status;
            frm.licExpDate = carDrivingLics.FirstOrDefault()?.Exprires.ToString("yyyyMMdd") ?? "";
            frm.borrUsr = borrUsr;
            frm.lastBorrTime = dataInfos.FirstOrDefault()?.BorrTime ?? (DateTime)default;

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

                            var dataBorrNow = data.FirstOrDefault(r => r.BackTime == default);

                            if (dataBorrNow != null)
                            {
                                vehicleStatuses[i].IdUserBorr = dataBorrNow.IdUserBorr;
                                vehicleStatuses[i].NameUserBorr = dataBorrNow.NameUserBorr;
                                vehicleStatuses[i].BorrTime = dataBorrNow.BorrTime.ToString("yyyy/MM/dd HH:mm");
                            }
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

                            var dataBorrNow = data.FirstOrDefault(r => r.BackTime == default);

                            if (dataBorrNow != null)
                            {
                                vehicleStatuses[i].IdUserBorr = dataBorrNow.IdUserBorr;
                                vehicleStatuses[i].NameUserBorr = dataBorrNow.NameUserBorr;
                                vehicleStatuses[i].BorrTime = dataBorrNow.BorrTime.ToString("yyyy/MM/dd HH:mm");
                            }

                            //vehicleStatuses[i].IdUserBorr = data.FirstOrDefault().IdUserBorr;
                            //vehicleStatuses[i].NameUserBorr = data.FirstOrDefault().NameUserBorr;
                            //vehicleStatuses[i].BorrTime = data.FirstOrDefault().BorrTime.ToString("yyyy/MM/dd HH:mm");
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

            // Kiểm tra xem có quyền đổi người dùng không
            var userGroups = dm_GroupUserBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id);
            var groupEditDeptAndJob = dm_GroupBUS.Instance.GetListByName("借車【換人員】");

            roleAdminBorrVehicle = groupEditDeptAndJob.Any(group => userGroups.Any(userGroup => userGroup.IdGroup == group.Id));

            if (roleAdminBorrVehicle)
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

        private void gvInfo_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (roleAdminBorrVehicle && e.HitInfo.InRowCell && e.HitInfo.RowHandle >= 0)
            {
                VehicleBorrInfo info = gvInfo.GetRow(gvInfo.FocusedRowHandle) as VehicleBorrInfo;

                if (info.BackTime == default && cbbTypeVehicle.SelectedIndex == 0)
                {
                    e.Menu.Items.Add(itemAdBackVehicle);
                }
                else if (cbbTypeVehicle.SelectedIndex == 0)
                {
                    e.Menu.Items.Add(itemEditInfo);
                }
            }
        }
    }
}
