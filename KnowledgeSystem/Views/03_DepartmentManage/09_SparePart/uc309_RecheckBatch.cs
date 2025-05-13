using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Util;
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
    public partial class uc309_RecheckBatch : DevExpress.XtraEditors.XtraUserControl
    {
        public uc309_RecheckBatch()
        {
            InitializeComponent();
            InitializeIcon();
            InitializeMenuItems();

            helper = new RefreshHelper(gvData, "Id");

            Font fontUI12 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI12;

            barCbbDept.EditValueChanged += CbbDept_EditValueChanged;
        }

        RefreshHelper helper;
        BindingSource sourceBases = new BindingSource();
        string idDept2word = TPConfigs.idDept2word;
        string deptGetData = "";

        List<dm_User> users = new List<dm_User>();
        List<dm_Group> groups;
        List<dm_Departments> depts;

        List<dt309_Materials> materials;
        List<dt309_Storages> storages;

        DXMenuItem itemViewInfo;
        DXMenuItem itemUpdatePrice;
        DXMenuItem itemMaterialIn;
        DXMenuItem itemMaterialOut;
        DXMenuItem itemMaterialTransfer;
        DXMenuItem itemMaterialCheck;
        DXMenuItem itemMaterialGetFromOther;

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
            barCbbDept.ImageOptions.SvgImage = TPSvgimages.Dept;
        }

        private void CreateRuleGV()
        {
            var ruleNotify = new GridFormatRule
            {
                ApplyToRow = true,
                Name = "RuleNotify",
                Rule = new FormatConditionRuleExpression
                {
                    Expression = "[BatchMaterial.ActualQuantity] != [BatchMaterial.InitialQuantity]",
                    Appearance =
                    {
                        BackColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical,
                        BackColor2 = Color.White,
                        Options = { UseBackColor = true }
                    }
                }
            };
            gvSparePart.FormatRules.Add(ruleNotify);
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
            //itemViewInfo = CreateMenuItem("查看資訊", ItemViewInfo_Click, TPSvgimages.View);
            //itemUpdatePrice = CreateMenuItem("更新單價", ItemUpdatePrice_Click, TPSvgimages.Money);

            //itemMaterialIn = CreateMenuItem("收料", ItemMaterialIn_Click, TPSvgimages.Num1);
            //itemMaterialOut = CreateMenuItem("領用", ItemMaterialOut_Click, TPSvgimages.Num2);
            //itemMaterialTransfer = CreateMenuItem("轉庫", ItemMaterialTransfer_Click, TPSvgimages.Num3);
            //itemMaterialCheck = CreateMenuItem("盤點", ItemMaterialCheck_Click, TPSvgimages.Num4);
            //itemMaterialGetFromOther = CreateMenuItem("調撥", ItemMaterialGetFromOther_Click, TPSvgimages.Num5);
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                deptGetData = (barCbbDept.EditValue?.ToString().Split(' ')[0]) ?? string.Empty;

                var inspectionBatch = dt309_InspectionBatchBUS.Instance.GetList();
                var inspectionBatchMaterials = dt309_InspectionBatchMaterialBUS.Instance.GetList();
                var materials = dt309_MaterialsBUS.Instance.GetListByStartIdDept(deptGetData);

                var depts = dm_DeptBUS.Instance.GetList();
                var users = dm_UserBUS.Instance.GetList();
                var units = dt309_UnitsBUS.Instance.GetList();

                var displayData = inspectionBatch.Select(batch =>
                {
                    var batchMaterialList = inspectionBatchMaterials
                        .Where(bm => bm.BatchId == batch.Id)
                        .Join(materials,
                              bm => bm.MaterialId,
                              m => m.Id,
                              (bm, m) => new
                              {
                                  Material = m,
                                  BatchMaterial = bm,
                                  Unit = units.FirstOrDefault(r => r.Id == m.IdUnit)?.DisplayName ?? "N/A",
                                  UserMngr = users.FirstOrDefault(r => r.Id == m.IdManager)?.DisplayName ?? "N/A",
                                  Dept = (depts.Where(r => r.Id == m.IdDept).Select(r => $"{r.Id} {r.DisplayName}").FirstOrDefault() ?? "N/A") + (string.IsNullOrEmpty(bm.ActualQuantity.ToString()) ? " - 處理中" : " - 已完成"),
                                  UserReCheck = string.IsNullOrEmpty(bm.ConfirmedBy) ? "" : users.FirstOrDefault(r => r.Id == bm.ConfirmedBy)?.DisplayName ?? "N/A"
                              })
                        .ToList();

                    return new
                    {
                        Batch = batch,
                        Spare = batchMaterialList
                    };
                }).ToList();

                sourceBases.DataSource = displayData;

                gvData.BestFitColumns();
                gvData.CollapseAllDetails();

                helper.LoadViewInfo();
            }
        }

        private void uc309_RecheckBatch_Load(object sender, EventArgs e)
        {
            gvSparePart.OptionsCustomization.AllowGroup = false;

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
            CreateRuleGV();
            gcData.DataSource = sourceBases;

            gvData.BestFitColumns();

            gvData.OptionsDetail.EnableMasterViewMode = true;
            gvData.OptionsView.ShowGroupPanel = false;
        }

        private void CbbDept_EditValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void gvData_MasterRowGetRelationCount(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gvData_MasterRowGetRelationName(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "備品";
        }

        private void gvData_MasterRowExpanded(object sender, DevExpress.XtraGrid.Views.Grid.CustomMasterRowEventArgs e)
        {
            GridView masterView = sender as GridView;
            int visibleDetailRelationIndex = masterView.GetVisibleDetailRelationIndex(e.RowHandle);
            GridView detailView = masterView.GetDetailView(e.RowHandle, visibleDetailRelationIndex) as GridView;

            detailView.BestFitColumns();
        }
    }
}
