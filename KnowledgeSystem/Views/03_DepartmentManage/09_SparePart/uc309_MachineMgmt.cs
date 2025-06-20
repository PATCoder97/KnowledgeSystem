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
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;

namespace KnowledgeSystem.Views._03_DepartmentManage._09_SparePart
{
    public partial class uc309_MachineMgmt : DevExpress.XtraEditors.XtraUserControl
    {
        public uc309_MachineMgmt()
        {
            InitializeComponent();
            InitializeIcon();
            InitializeMenuItems();

            helper = new RefreshHelper(gvData, "Id");

            Font fontUI12 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI12;
        }

        RefreshHelper helper;
        BindingSource sourceBases = new BindingSource();
        string idDept2word = TPConfigs.idDept2word;
        string deptGetData = "";

        List<dm_User> users = new List<dm_User>();

        List<dt309_Machines> machines;
        List<dt309_Materials> materials;
        List<dt309_MachineMaterials> machineMaterials;
        List<dt309_Storages> storages;
        List<dt309_Units> units;

        DXMenuItem itemViewInfo;
        DXMenuItem itemUpdatePrice;
        DXMenuItem itemMaterialIn;
        DXMenuItem itemMaterialOut;
        DXMenuItem itemMaterialTransfer;
        DXMenuItem itemMaterialCheck;

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
            barCbbDept.ImageOptions.SvgImage = TPSvgimages.Dept;
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
            itemViewInfo = CreateMenuItem("查看資訊", ItemViewInfo_Click, TPSvgimages.View);
        }

        private void ItemViewInfo_Click(object sender, EventArgs e)
        {
            GridView view = gvData;
            int idMachine = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColIdMachine));
            f309_Machine_Info fInfo = new f309_Machine_Info()
            {
                eventInfo = EventFormInfo.View,
                formName = "用料設備",
                idBase = idMachine
            };
            fInfo.ShowDialog();

            LoadData();
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                deptGetData = (barCbbDept.EditValue?.ToString().Split(' ')[0]) ?? string.Empty;
                machines = dt309_MachinesBUS.Instance.GetListByStartIdDept(deptGetData);
                materials = dt309_MaterialsBUS.Instance.GetList();
                machineMaterials = dt309_MachineMaterialsBUS.Instance.GetList();

                storages = dt309_StoragesBUS.Instance.GetList();
                users = dm_UserBUS.Instance.GetList();
                units = dt309_UnitsBUS.Instance.GetList();

                var displayData = machines.Select(machine =>
                {
                    var machineMaterialList = machineMaterials
                        .Where(mm => mm.MachineId == machine.Id)
                        .Join(materials,
                              mm => mm.MaterialId,
                              m => m.Id,
                              (mm, m) => m)
                        .ToList();

                    // Ép kiểu từng thành phần sang double để đảm bảo kết quả đúng
                    double totalPrice = machineMaterialList.Sum(m =>
                        Convert.ToDouble(m.Price) * (Convert.ToDouble(m.QuantityInStorage) + Convert.ToDouble(m.QuantityInMachine)));

                    // Đếm số lượng vật tư dùng làm tiêu hao phẩm (消耗品)
                    int consumableCount = machineMaterialList.Count(r => r.TypeUse == "消耗品");

                    // Đếm số lượng vật tư dùng làm thiết bị phụ tùng (備品)
                    int equipmentCount = machineMaterialList.Count(r => r.TypeUse == "備品");

                    return new
                    {
                        Machine = machine,
                        物料 = machineMaterialList,
                        TotalPrice = totalPrice,
                        consumableCount,
                        equipmentCount
                    };
                }).ToList();

                sourceBases.DataSource = displayData;

                gvData.BestFitColumns();
                gvData.CollapseAllDetails();

                helper.LoadViewInfo();
            }
        }

        private void uc309_MachineMgmt_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            gvSparePart.ReadOnlyGridView();
            gvSparePart.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            // Kiểm tra quyền từng ke để có quyền truy cập theo nhóm
            var userGroups = dm_GroupUserBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id);
            var departments = dm_DeptBUS.Instance.GetList();
            var groups = dm_GroupBUS.Instance.GetListByName("機邊庫");

            var accessibleGroups = groups
                .Where(group => userGroups.Any(userGroup => userGroup.IdGroup == group.Id))
                .ToList();

            var departmentItems = departments
                .Where(dept => accessibleGroups.Any(group => group.IdDept == dept.Id))
                .Select(dept => new ComboBoxItem { Value = $"{dept.Id} {dept.DisplayName}" })
                .ToArray();

            cbbDept.Items.AddRange(departmentItems);
            barCbbDept.EditValue = departmentItems.FirstOrDefault()?.Value ?? string.Empty;

            LoadData();
            gcData.DataSource = sourceBases;

            gvData.BestFitColumns();

            gvData.OptionsDetail.EnableMasterViewMode = true;
            gvData.OptionsView.ShowGroupPanel = false;
        }

        private void gvData_MasterRowEmpty(object sender, MasterRowEmptyEventArgs e)
        {
            //GridView view = sender as GridView;
            //dt309_Machines machine = view.GetRow(e.RowHandle) as dt309_Machines;
            //int idMachine = machine.Id;

            //e.IsEmpty = dt309_MachineMaterialsBUS.Instance.GetListByIdMachine(idMachine).Count() == 0;
        }

        private void gvData_MasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gvData_MasterRowGetRelationName(object sender, MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "備品";
        }

        private void gvData_MasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
        {
            GridView view = sender as GridView;
            if (view != null)
            {
                var parentRow = view.GetRow(e.RowHandle) as dynamic;
                if (parentRow != null)
                {
                    // Kiểm tra xem parentRow có chứa thuộc tính Materials và thuộc tính đó có phải là danh sách hay không
                    var materialsChild = parentRow.物料 as IEnumerable<dynamic>;

                    if (materialsChild != null)
                    {
                        var displayData = materialsChild.Select(x => new
                        {
                            data = x,
                            Unit = units.FirstOrDefault(u => u.Id == x.IdUnit)?.DisplayName,
                            UserMngr = users.FirstOrDefault(u => u.Id == x.IdManager)?.DisplayName,
                            TotalPrice = Convert.ToDouble(x.Price * (x.QuantityInStorage + x.QuantityInMachine)),
                        }).ToList();

                        e.ChildList = displayData;
                    }
                }
            }
        }

        private void gvData_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            GridView masterView = sender as GridView;
            int visibleDetailRelationIndex = masterView.GetVisibleDetailRelationIndex(e.RowHandle);
            GridView detailView = masterView.GetDetailView(e.RowHandle, visibleDetailRelationIndex) as GridView;

            detailView.BestFitColumns();
        }

        private void gvData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow)
            {
                e.Menu.Items.Add(itemViewInfo);
            }
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f309_Machine_Info finfo = new f309_Machine_Info();
            finfo.ShowDialog();
            LoadData();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void barCbbDept_EditValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
