using BusinessLayer;
using DataAccessLayer;
using DevExpress.Data;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Items.ViewInfo;
using DevExpress.XtraPrinting.Native;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._10_EHSWorkforce
{
    public partial class uc310_UpdateLeaveUser : DevExpress.XtraEditors.XtraUserControl
    {
        public uc310_UpdateLeaveUser()
        {
            InitializeComponent();
            InitializeIcon();
            InitializeMenuItems();

            helper = new RefreshHelper(gvData, "Id");

            Font fontUI12 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI12;
        }

        RefreshHelper helper;
        BindingSource sourceDataUpdate = new BindingSource();
        BindingSource sourceOrg = new BindingSource();

        List<dm_Departments> depts;
        List<dt310_UpdateLeaveUser> updateLeaveUsers;
        List<dt310_Role> roles;
        List<dm_User> users;

        DXMenuItem itemUpdateLeaveUser;
        DXMenuItem itemUpdatePrice;

        private void InitializeIcon()
        {
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
        }

        DXMenuItem CreateMenuItem(string caption, EventHandler clickEvent, SvgImage svgImage)
        {
            var menuItem = new DXMenuItem(caption, clickEvent, svgImage, DXMenuItemPriority.Normal);
            SetMenuItemProperties(menuItem);
            return menuItem;
        }

        void SetMenuItemProperties(DXMenuItem menuItem)
        {
            menuItem.ImageOptions.SvgImageSize = new System.Drawing.Size(24, 24);
            menuItem.AppearanceHovered.ForeColor = Color.Blue;
        }

        private void InitializeMenuItems()
        {
            itemUpdateLeaveUser = CreateMenuItem("重新呈核", ItemUpdateLeaveUser_Click, TPSvgimages.Edit);
            //itemUpdatePrice = CreateMenuItem("更新單價", ItemUpdatePrice_Click, TPSvgimages.Money);
        }

        private void ItemUpdateLeaveUser_Click(object sender, EventArgs e)
        {
            GridView view = gvData;
            int idUpdateData = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColIdData));

            f310_UpdateLeaveUser_Info updateLeaveUser_Info = new f310_UpdateLeaveUser_Info()
            {
                idDataUpdate = idUpdateData,
            };

            updateLeaveUser_Info.ShowDialog();

            LoadData();
        }

        private void LoadData()
        {
            helper.SaveViewInfo();

            roles = dt310_RoleBUS.Instance.GetList();
            updateLeaveUsers = dt310_UpdateLeaveUserBUS.Instance.GetList();
            depts = dm_DeptBUS.Instance.GetAllChildren(0).Where(r => r.IsGroup != true && !TPConfigs.ExclusionDept310.Split(';').Contains(r.Id)).ToList();
            users = dm_UserBUS.Instance.GetList();

            var currentUserGroups = dm_GroupUserBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id);
            var groups = dm_GroupBUS.Instance.GetList();

            var displayData = updateLeaveUsers.Select(r => new
            {
                r.Id,
                IdUserLeave = users.FirstOrDefault(u => u.Id == r.IdUserLeave) != null
                    ? $"{r.IdUserLeave} {users.FirstOrDefault(u => u.Id == r.IdUserLeave).DisplayName}"
                    : r.IdUserLeave,
                r.DisplayName,
                r.IsProcess,
                r.IsCancel,
                GroupProcess = r.IdGroupProcess == -1
                    ? ""
                    : groups.FirstOrDefault(g => g.Id == r.IdGroupProcess)?.DisplayName ?? "",
                CreateBy = users.FirstOrDefault(u => u.Id == r.CreateBy) != null
                    ? $"{r.CreateBy} {users.FirstOrDefault(u => u.Id == r.CreateBy).DisplayName}"
                    : r.CreateBy,
                r.CreateAt,
                HasMyPermission = r.IdGroupProcess == -1 ? false : currentUserGroups.Any(g => g.IdGroup == r.IdGroupProcess)
            }).ToList();

            sourceDataUpdate.DataSource = displayData;
            gvData.BestFitColumns();

            helper.LoadViewInfo();
        }

        private void uc310_UpdateLeaveUser_Load(object sender, EventArgs e)
        {
            LoadData();

            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            gcData.DataSource = sourceDataUpdate;

            gColHasMyPermission.SortOrder = DevExpress.Data.ColumnSortOrder.Descending;
            gvData.BestFitColumns();
        }

        private void gvData_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow)
            {
                GridView view = sender as GridView;
                bool isCancel = Convert.ToBoolean(view.GetRowCellValue(view.FocusedRowHandle, gridColumn4));

                // Chỉ hiện menu "重新呈核" khi IsCancel = true
                if (isCancel)
                {
                    e.Menu.Items.Add(itemUpdateLeaveUser);
                }
                //e.Menu.Items.Add(itemUpdatePrice);
            }
        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null || view.FocusedRowHandle < 0 || view.IsGroupRow(view.FocusedRowHandle))
                return;

            object cellValue = view.GetRowCellValue(view.FocusedRowHandle, gColIdData);
            if (cellValue == null)
                return;

            int idUpdateData = Convert.ToInt32(cellValue);
            var updateLeaveUser = dt310_UpdateLeaveUserBUS.Instance.GetItemById(idUpdateData);

            if (updateLeaveUser == null)
                return;

            if (string.IsNullOrWhiteSpace(updateLeaveUser.DataJson))
            {
                f310_UpdateLeaveUser_Info updateLeaveUser_Info = new f310_UpdateLeaveUser_Info()
                {
                    idDataUpdate = idUpdateData,
                };

                updateLeaveUser_Info.ShowDialog();
            }
            else
            {
                f310_UpdateApproval updateApproval = new f310_UpdateApproval()
                {
                    idDataUpdate = idUpdateData,
                };

                updateApproval.ShowDialog();
            }

            LoadData();
        }

        private void gvData_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            GridGroupRowInfo info = e.Info as GridGroupRowInfo;

            if (info != null && info.Column == gColHasMyPermission)
            {
                object groupValueObj = view.GetGroupRowValue(e.RowHandle, info.Column);
                bool hasMyPermission = groupValueObj != null && Convert.ToBoolean(groupValueObj);

                string colorName = hasMyPermission ? "Red" : "Green";
                string groupValue = hasMyPermission ? "待審查" : "已審查/非我審核";

                int rowCount = view.GetChildRowCount(e.RowHandle);

                info.GroupText = $" <color={colorName}>{groupValue}</color>《<color=Blue>{rowCount}筆</color>》";
            }
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }
    }
}
