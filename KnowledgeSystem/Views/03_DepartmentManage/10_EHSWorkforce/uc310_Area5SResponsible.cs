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

            DevExpress.Utils.AppearanceObject.DefaultMenuFont = new System.Drawing.Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
        }

        BindingSource source5sArea = new BindingSource();
        BindingSource sourceResp = new BindingSource();

        DXMenuItem itemViewInfo;

        List<dt310_Area5S> area5S;
        List<dt310_Area5SResponsible> areaResps;

        List<dm_Departments> depts;
        List<dt310_Role> roles;
        List<dm_User> users;

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
            f310_Area5SResponsible_Info fInfo = new f310_Area5SResponsible_Info() { isAddInfo = true, isEditorInfo = false };
            fInfo.ShowDialog(this);
            LoadData();
        }

        private void LoadData()
        {
            area5S = dt310_Area5SBUS.Instance.GetList();
            areaResps = dt310_Area5SResponsibleBUS.Instance.GetList();
            users = dm_UserBUS.Instance.GetList();

            source5sArea.DataSource = area5S;

            var joinData = from resp in areaResps
                           join area in area5S on resp.AreaId equals area.Id
                           where resp.DeletedAt == null && area.DeletedAt == null
                           let emp = users.FirstOrDefault(r => r.Id == resp.EmployeeId)
                           let agent = users.FirstOrDefault(r => r.Id == resp.AgentId)
                           let boss = users.FirstOrDefault(r => r.Id == resp.BossId)
                           select new
                           {
                               EmployeeId = resp.EmployeeId,
                               EmpName = emp != null ? $"LG{emp.IdDepartment}/{emp.DisplayName}" : $"Unknown-{resp.EmployeeId}",
                               AgentName = agent != null ? $"LG{agent.IdDepartment}/{agent.DisplayName}" : $"Unknown-{resp.AgentId}",
                               BossName = boss != null ? $"LG{boss.IdDepartment}/{boss.DisplayName}" : $"Unknown-{resp.BossId}",
                               AreaId = area.Id,
                               AreaName = area.DisplayName,
                               AreaDesc = area.DESC,
                               RespAreaCode = resp.AreaCode,
                               RespAreaName = resp.AreaName,
                           };

            // Root node: EmployeeId
            var rootNodes = joinData
                .GroupBy(x => x.EmpName)
                .Select(x => new
                {
                    Id = x.Key,
                    IdParent = (string)null,
                    Name = x.Key,
                    EmpName = (string)null,
                    AgentName = (string)null,
                    BossName = (string)null,
                    RespAreaCode = (string)null,
                    RespAreaName = (string)null
                });

            // Child node: Areas
            var childNodes = joinData
                .Select(x => new
                {
                    Id = $"{x.EmployeeId}_{x.AreaId}_{Guid.NewGuid().ToString("N").Substring(5)}",
                    IdParent = x.EmpName,
                    Name = x.AreaName,      // tên hiển thị node con
                    EmpName = x.EmpName,
                    AgentName = x.AgentName,
                    BossName = x.BossName,
                    RespAreaCode = x.RespAreaCode,
                    RespAreaName = x.RespAreaName
                });

            // Merge
            var treeListSource = rootNodes
                .Union(childNodes)
                .ToList();

            // Gán TreeList
            sourceResp.DataSource = treeListSource;

            treeResps.BestFitColumns();
            gvData.BestFitColumns();
        }

        private void uc310_Area5SResponsible_Load(object sender, EventArgs e)
        {
            LoadData();

            treeResps.DataSource = sourceResp;
            treeResps.KeyFieldName = "Id";
            treeResps.ParentFieldName = "IdParent";
            treeResps.BestFitColumns();
            treeResps.ReadOnlyTreelist();
            treeResps.KeyDown += GridControlHelper.TreeViewCopyCellData_KeyDown;

            TreeListNode node = treeResps.GetNodeByVisibleIndex(0);
            if (node != null)
                node.Expanded = !node.Expanded;

            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            gcData.DataSource = source5sArea;
            gvData.BestFitColumns();
            gvData.OptionsDetail.EnableMasterViewMode = true;
            gvData.OptionsView.ShowGroupPanel = false;

            gvResponsibleEmp.ReadOnlyGridView();
            gvResponsibleEmp.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvResponsibleEmp.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
        }

        private void gvData_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow)
            {
                e.Menu.Items.Add(itemViewInfo);
            }
        }

        private void gvData_MasterRowEmpty(object sender, MasterRowEmptyEventArgs e)
        {
            GridView view = sender as GridView;
            int idArea = Convert.ToInt32(view.GetRowCellValue(e.RowHandle, gColId));
            e.IsEmpty = !areaResps.Any(r => r.AreaId == idArea);
        }

        private void gvData_MasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
        {
            GridView view = sender as GridView;
            int idArea = Convert.ToInt32(view.GetRowCellValue(e.RowHandle, gColId));
            e.ChildList = areaResps
                .Where(r => r.AreaId == idArea)
                .Select(r =>
                {
                    var emp = users.FirstOrDefault(u => u.Id == r.EmployeeId);
                    var agent = users.FirstOrDefault(u => u.Id == r.AgentId);
                    var boss = users.FirstOrDefault(u => u.Id == r.BossId);
                    return new
                    {
                        r.AreaCode,
                        r.AreaName,
                        r.DeptId,
                        EmpName = emp != null ? $"LG{emp.IdDepartment}/{emp.DisplayName}" : $"Unknown-{r.EmployeeId}",
                        AgentName = agent != null ? $"LG{agent.IdDepartment}/{agent.DisplayName}" : $"Unknown-{r.AgentId}",
                        BossName = boss != null ? $"LG{boss.IdDepartment}/{boss.DisplayName}" : $"Unknown-{r.BossId}"
                    };
                }).ToList();
        }

        private void gvData_MasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gvData_MasterRowGetRelationName(object sender, MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "Responsible";
        }

        private void gvData_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            GridView masterView = sender as GridView;
            int visibleDetailRelationIndex = masterView.GetVisibleDetailRelationIndex(e.RowHandle);
            GridView detailView = masterView.GetDetailView(e.RowHandle, visibleDetailRelationIndex) as GridView;

            detailView.BestFitColumns();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }
    }
}
