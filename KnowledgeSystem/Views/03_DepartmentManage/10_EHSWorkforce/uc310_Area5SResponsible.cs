using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Items.ViewInfo;
using DevExpress.XtraTreeList.Nodes;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._03_DepartmentManage._09_SparePart;
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
    public partial class uc310_Area5SResponsible : DevExpress.XtraEditors.XtraUserControl
    {
        public uc310_Area5SResponsible()
        {
            InitializeComponent();
            InitializeIcon();
            InitializeMenuItems();
        }

        BindingSource source5sArea = new BindingSource();
        BindingSource sourceOrg = new BindingSource();

        DXMenuItem itemViewInfo;

        List<dt310_Area5S> area5S;

        List<dm_Departments> depts;
        List<dt310_Role> roles;
        List<dm_User> users;
        //List<UnitEHSOrgCustom> unitEHSOrgCustoms;

        //private class UnitEHSOrgCustom
        //{
        //    public int Id { get; set; }
        //    public int? IdData { get; set; }
        //    public int IdParent { get; set; }
        //    public string DeptName { get; set; }
        //    public string Role { get; set; }
        //    public string Emp { get; set; }
        //}

        DXMenuItem CreateMenuItem(string caption, EventHandler clickEvent, SvgImage svgImage)
        {
            var menuItem = new DXMenuItem(caption, clickEvent, svgImage, DXMenuItemPriority.Normal);
            SetMenuItemProperties(menuItem);
            return menuItem;
        }

        void SetMenuItemProperties(DXMenuItem menuItem)
        {
            menuItem.ImageOptions.SvgImageSize = new Size(24, 24);
            menuItem.AppearanceHovered.ForeColor = System.Drawing.Color.Blue;
        }

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnSummaryTable.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void InitializeMenuItems()
        {
            itemViewInfo = CreateMenuItem("查看信息", ItemViewInfo_Click, TPSvgimages.View);
        }

        private void ItemViewInfo_Click(object sender, EventArgs e)
        {
            GridView view = gvData;
            int idArea = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId));
            f310_Area5SResponsible_Info fInfo = new f310_Area5SResponsible_Info()
            {
                eventInfo = EventFormInfo.View,
                formName = "區域",
                idBase = idArea,
                isAddInfo = false
            };
            fInfo.ShowDialog();

            LoadData();
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f310_Area5SResponsible_Info fInfo = new f310_Area5SResponsible_Info() { isAddInfo = false, isEditorInfo = false };
            fInfo.ShowDialog(this);
        }

        private void LoadData()
        {
            area5S = dt310_Area5SBUS.Instance.GetList();
            source5sArea.DataSource = area5S;



            //roles = dt310_RoleBUS.Instance.GetList();
            //unitEHSOrgs = dt310_UnitEHSOrgBUS.Instance.GetList();
            ////depts = dm_DeptBUS.Instance.GetList();
            //depts = dm_DeptBUS.Instance.GetAllChildren(0).Where(r => r.IsGroup != true && !TPConfigs.ExclusionDept310.Split(';').Contains(r.Id)).ToList();
            //users = dm_UserBUS.Instance.GetList();

            //unitEHSOrgCustoms = new List<UnitEHSOrgCustom>();

            //int index = 1;
            //List<dm_Departments> startParents = depts.Where(r => r.IdParent == -1).ToList();

            //foreach (var parent in startParents)
            //{
            //    AddDepartmentRecursive(parent, 0, ref index, unitEHSOrgCustoms, depts, unitEHSOrgs, roles, users);
            //}

            //source5sArea.DataSource = unitEHSOrgCustoms;

            //var now = DateTime.Now;
            //var result = unitEHSOrgs.Select(u =>
            //{
            //    var roleObj = roles.FirstOrDefault(r => r.Id == u.RoleId);
            //    var userObj = users.FirstOrDefault(r => r.Id == u.EmployeeId);

            //    return new
            //    {
            //        u.Id,
            //        Emp = $"{userObj?.IdDepartment} {userObj?.DisplayName} {userObj?.DisplayNameVN}",
            //        Role = roleObj?.DisplayName,
            //        u.StartDate,
            //        ThamNien = (now.Year - u.StartDate.Year) -
            //                   (now.Month < u.StartDate.Month ||
            //                   (now.Month == u.StartDate.Month && now.Day < u.StartDate.Day) ? 1 : 0)
            //    };
            //}).ToList();


            //sourceOrg.DataSource = result;
            //treeFunctions.BestFitColumns();
            gvData.BestFitColumns();
        }

        private void uc310_Area5SResponsible_Load(object sender, EventArgs e)
        {
            LoadData();

            //treeFunctions.DataSource = sourceFunc;
            //treeFunctions.KeyFieldName = "Id";
            //treeFunctions.ParentFieldName = "IdParent";
            //treeFunctions.CheckBoxFieldName = "Status";
            //treeFunctions.BestFitColumns();
            //treeFunctions.ReadOnlyTreelist();

            //TreeListNode node = treeFunctions.GetNodeByVisibleIndex(0);
            //if (node != null)
            //    node.Expanded = !node.Expanded;

            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            gcData.DataSource = source5sArea;
            gvData.BestFitColumns();
            gvData.OptionsDetail.EnableMasterViewMode = true;
            gvData.OptionsView.ShowGroupPanel = false;
        }

        private void gvData_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow)
            {
                e.Menu.Items.Add(itemViewInfo);
            }
        }
    }
}
