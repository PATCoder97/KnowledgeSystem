using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Items.ViewInfo;
using KnowledgeSystem.Helpers;
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

        const string MOTOR = "Xe máy";
        const string CAR = "Ô tô";

        DXMenuItem itemViewInfo;

        private void InitializeMenuItems()
        {
            itemViewInfo = new DXMenuItem("顯示信息", ItemViewInfo_Click, TPSvgimages.Info, DXMenuItemPriority.Normal);
            //itemViewFile = new DXMenuItem("讀取檔案", ItemViewFile_Click, TPSvgimages.View, DXMenuItemPriority.Normal);
            //itemAddPlanFile = new DXMenuItem("上傳計劃表", ItemAddPlanFile_Click, TPSvgimages.UpLevel, DXMenuItemPriority.Normal);
            //itemAddAttach = new DXMenuItem("上傳報告", ItemAddAttach_Click, TPSvgimages.UploadFile, DXMenuItemPriority.Normal);
            //itemCloseReport = new DXMenuItem("結案", ItemCloseReport_Click, TPSvgimages.Confirm, DXMenuItemPriority.Normal);
            //itemDelAttach = new DXMenuItem("刪除附件", ItemDelAttach_Click, TPSvgimages.Remove, DXMenuItemPriority.Normal);

            itemViewInfo.ImageOptions.SvgImageSize = new Size(24, 24);
            itemViewInfo.AppearanceHovered.ForeColor = Color.Blue;

            //itemViewFile.ImageOptions.SvgImageSize = new Size(24, 24);
            //itemViewFile.AppearanceHovered.ForeColor = Color.Blue;

            //itemAddPlanFile.ImageOptions.SvgImageSize = new Size(24, 24);
            //itemAddPlanFile.AppearanceHovered.ForeColor = Color.Blue;

            //itemCloseReport.ImageOptions.SvgImageSize = new Size(24, 24);
            //itemCloseReport.AppearanceHovered.ForeColor = Color.Blue;

            //itemAddAttach.ImageOptions.SvgImageSize = new Size(24, 24);
            //itemAddAttach.AppearanceHovered.ForeColor = Color.Blue;

            //itemDelAttach.ImageOptions.SvgImageSize = new Size(24, 24);
            //itemDelAttach.AppearanceHovered.ForeColor = Color.Blue;
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
            gvVehicleStatus.ReadOnlyGridView();
            gvVehicleStatus.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            gvInfo.ReadOnlyGridView();
            gvInfo.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            cbbTypeVehicle.Properties.Items.AddRange(new object[] { MOTOR, CAR });
        }

        private async void cbbTypeVehicle_SelectedIndexChanged(object sender, EventArgs e)
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

            gcVehicleStatus.DataSource = vehicleStatuses;
            gvVehicleStatus.BestFitColumns();
        }

        private void gvVehicleStatus_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell)
            {
                GridView view = sender as GridView;
                view.FocusedRowHandle = e.HitInfo.RowHandle;

                //     int idBase = Convert.ToInt16(view.GetRowCellValue(e.HitInfo.RowHandle, gColId));
                //bool HaveReport = reportsInfo.Any(r => r.IdBase == idBase);

                e.Menu.Items.Add(itemViewInfo);

                //if (HaveReport)
                //{
                //    e.Menu.Items.Add(itemViewFile);
                //}
                //else
                //{
                //    e.Menu.Items.Add(itemAddPlanFile);
                //}
            }
        }
    }
}
