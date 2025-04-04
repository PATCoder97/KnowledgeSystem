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
        List<dt308_Disease> dt308Diseases;

        Dictionary<string, string> events = new Dictionary<string, string>()
        {
            {"IN","入庫" },
            {"OUT","出庫" },
            {"CHECK","盤點" },
        };

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

        private void CreateRuleGV()
        {
            //// Quy tắc cảnh báo khi số lượng trong kho + máy < min
            //var ruleCHECK = new GridFormatRule
            //{
            //    Column = gColEvent,
            //    Name = "RuleCheck",
            //    Rule = new FormatConditionRuleExpression
            //    {
            //        Expression = "StartsWith([data.TransactionType], \'C\')",
            //        Appearance =
            //        {
            //            ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical,
            //        }
            //    }
            //};
            //gvTransactions.FormatRules.Add(ruleCHECK);

            //var ruleIN = new GridFormatRule
            //{
            //    Column = gColEvent,
            //    Name = "RuleIn",
            //    Rule = new FormatConditionRuleExpression
            //    {
            //        Expression = "StartsWith([data.TransactionType], \'I\')",
            //        Appearance =
            //        {
            //            ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Question,
            //        }
            //    }
            //};
            //gvTransactions.FormatRules.Add(ruleIN);

            //// Quy tắc cảnh báo khi số lượng trong kho + máy < min
            //var ruleNotify = new GridFormatRule
            //{
            //    ApplyToRow = true,
            //    Name = "RuleNotify",
            //    Rule = new FormatConditionRuleExpression
            //    {
            //        Expression = "[data.QuantityInStorage] + [data.QuantityInMachine] < [data.MinQuantity]",
            //        Appearance =
            //        {
            //            BackColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical,
            //            BackColor2 = Color.White,
            //            Options = { UseBackColor = true }
            //        }
            //    }
            //};
            //gvData.FormatRules.Add(ruleNotify);

            //// Quy tắc hiển thị biểu tượng tăng/giảm trong lịch sử giao dịch
            //var ruleIconSet = new GridFormatRule
            //{
            //    Name = "RuleTransactionTrend",
            //    Column = gColQuantity,
            //    Rule = new FormatConditionRuleIconSet
            //    {
            //        IconSet = new FormatConditionIconSet
            //        {
            //            ValueType = FormatConditionValueType.Automatic,
            //            Icons =
            //            {
            //                new FormatConditionIconSetIcon { PredefinedName = "Arrows3_1.png", Value = 0, ValueComparison = FormatConditionComparisonType.Greater },
            //                new FormatConditionIconSetIcon { PredefinedName = "Triangles3_2.png", Value = 0, ValueComparison = FormatConditionComparisonType.GreaterOrEqual },
            //                new FormatConditionIconSetIcon { PredefinedName = "Arrows3_3.png", Value = decimal.MinValue, ValueComparison = FormatConditionComparisonType.GreaterOrEqual }
            //            }
            //        }
            //    }
            //};
            //gvTransactions.FormatRules.Add(ruleIconSet);
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
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                machines = dt309_MachinesBUS.Instance.GetListByIdDept(TPConfigs.LoginUser.IdDepartment);

                //storages = dt309_StoragesBUS.Instance.GetList();
                //users = dm_UserBUS.Instance.GetList();
                //var units = dt309_UnitsBUS.Instance.GetList();

                //materials = dt309_MaterialsBUS.Instance.GetListByIdDept(TPConfigs.LoginUser.IdDepartment);

                //var displayData = materials.Select(x => new
                //{
                //    data = x,
                //    Unit = units.FirstOrDefault(u => u.Id == x.IdUnit).DisplayName,
                //    UserMngr = users.FirstOrDefault(u => u.Id == x.IdManager).DisplayName,
                //}).ToList();

                sourceBases.DataSource = machines;

                gvData.BestFitColumns();
                gvData.CollapseAllDetails();

                helper.LoadViewInfo();

                users = dm_UserBUS.Instance.GetList();
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
            CreateRuleGV();
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
            GridView view = sender as GridView;
            dt309_Machines machine = view.GetRow(e.RowHandle) as dt309_Machines;
            int idMachine = machine.Id;

            if (machine != null)
            {
                var materialsId = dt309_MachineMaterialsBUS.Instance.GetListByIdMachine(idMachine).Select(r => r.MaterialId).ToList();
                var materials = dt309_MaterialsBUS.Instance.GetListByIds(materialsId);

                e.ChildList = materials;
            }
        }

        private void gvData_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            GridView masterView = sender as GridView;
            int visibleDetailRelationIndex = masterView.GetVisibleDetailRelationIndex(e.RowHandle);
            GridView detailView = masterView.GetDetailView(e.RowHandle, visibleDetailRelationIndex) as GridView;

            detailView.BestFitColumns();
        }
    }
}
