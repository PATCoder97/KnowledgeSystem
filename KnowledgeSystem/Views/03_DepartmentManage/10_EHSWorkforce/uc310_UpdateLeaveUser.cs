using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
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
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
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
            itemUpdateLeaveUser = CreateMenuItem("更新負責人", ItemUpdateLeaveUser_Click, TPSvgimages.View);
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
            roles = dt310_RoleBUS.Instance.GetList();
            updateLeaveUsers = dt310_UpdateLeaveUserBUS.Instance.GetList();
            depts = dm_DeptBUS.Instance.GetAllChildren(0).Where(r => r.IsGroup != true && !TPConfigs.ExclusionDept310.Split(';').Contains(r.Id)).ToList();
            users = dm_UserBUS.Instance.GetList();

            sourceDataUpdate.DataSource = updateLeaveUsers;
            gvData.BestFitColumns();
        }

        private void uc310_UpdateLeaveUser_Load(object sender, EventArgs e)
        {
            LoadData();

            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            gcData.DataSource = sourceDataUpdate;
        }

        private void gvData_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow)
            {
                e.Menu.Items.Add(itemUpdateLeaveUser);
                //e.Menu.Items.Add(itemUpdatePrice);
            }
        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            int idUpdateData = (int)view.GetRowCellValue(view.FocusedRowHandle, gColIdData);
            var updateLeaveUser = dt310_UpdateLeaveUserBUS.Instance.GetItemById(idUpdateData);

            if (string.IsNullOrWhiteSpace(updateLeaveUser?.DataJson))
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
    }
}
