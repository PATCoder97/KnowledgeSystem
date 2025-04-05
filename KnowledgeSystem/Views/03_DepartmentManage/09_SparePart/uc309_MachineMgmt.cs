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

        List<dm_User> users = new List<dm_User>();

        List<dt309_Machines> machines;
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

                machines = dt309_MachinesBUS.Instance.GetListByIdDept(TPConfigs.LoginUser.IdDepartment);

                storages = dt309_StoragesBUS.Instance.GetList();
                users = dm_UserBUS.Instance.GetList();
                units = dt309_UnitsBUS.Instance.GetList();

                sourceBases.DataSource = machines;

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

            LoadData();
            gcData.DataSource = sourceBases;

            gvData.BestFitColumns();

            gvData.OptionsDetail.EnableMasterViewMode = true;
            gvData.OptionsView.ShowGroupPanel = false;
        }

        private void gvData_MasterRowEmpty(object sender, MasterRowEmptyEventArgs e)
        {
            GridView view = sender as GridView;
            dt309_Machines machine = view.GetRow(e.RowHandle) as dt309_Machines;
            int idMachine = machine.Id;

            e.IsEmpty = dt309_MachineMaterialsBUS.Instance.GetListByIdMachine(idMachine).Count() == 0;
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
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                GridView view = sender as GridView;
                dt309_Machines machine = view.GetRow(e.RowHandle) as dt309_Machines;
                int idMachine = machine.Id;

                if (machine != null)
                {
                    var materialsId = dt309_MachineMaterialsBUS.Instance.GetListByIdMachine(idMachine).Select(r => r.MaterialId).ToList();
                    var materials = dt309_MaterialsBUS.Instance.GetListByIds(materialsId);

                    var displayData = materials.Select(x => new
                    {
                        data = x,
                        Unit = units.FirstOrDefault(u => u.Id == x.IdUnit).DisplayName,
                        UserMngr = users.FirstOrDefault(u => u.Id == x.IdManager).DisplayName,
                    }).ToList();

                    e.ChildList = displayData;
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
    }
}
