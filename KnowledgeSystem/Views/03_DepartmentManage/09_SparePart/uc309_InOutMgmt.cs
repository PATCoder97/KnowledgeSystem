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
using System.Windows.Media.Media3D;
using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;

namespace KnowledgeSystem.Views._03_DepartmentManage._09_SparePart
{
    public partial class uc309_InOutMgmt : DevExpress.XtraEditors.XtraUserControl
    {
        public uc309_InOutMgmt()
        {
            InitializeComponent();
            InitializeIcon();
            InitializeMenuItems();
            CreateRuleGV();

            helper = new RefreshHelper(gvData, "Id");

            Font fontUI12 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI12;
        }

        RefreshHelper helper;
        BindingSource sourceBases = new BindingSource();
        string idDept2word = TPConfigs.idDept2word;

        List<dm_User> users = new List<dm_User>();

        List<dt309_Transactions> transactions;
        List<dt309_Machines> machines;
        List<dt309_Storages> storages;
        List<dt309_Units> units;

        DXMenuItem itemViewInfo;
        DXMenuItem itemUpdatePrice;
        DXMenuItem itemMaterialIn;
        DXMenuItem itemMaterialOut;
        DXMenuItem itemMaterialTransfer;
        DXMenuItem itemMaterialCheck;

        Dictionary<string, string> events = new Dictionary<string, string>()
        {
            {"IN","入庫" },
            {"OUT","出庫" },
            {"CHECK","盤點" },
        };

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
            //itemViewInfo = CreateMenuItem("查看資訊", ItemViewInfo_Click, TPSvgimages.View);
            //itemUpdatePrice = CreateMenuItem("更新單價", ItemUpdatePrice_Click, TPSvgimages.Money);

            //itemMaterialIn = CreateMenuItem("收料", ItemMaterialIn_Click, TPSvgimages.Num1);
            //itemMaterialOut = CreateMenuItem("領用", ItemMaterialOut_Click, TPSvgimages.Num2);
            //itemMaterialTransfer = CreateMenuItem("轉庫", ItemMaterialTransfer_Click, TPSvgimages.Num3);
            //itemMaterialCheck = CreateMenuItem("盤點", ItemMaterialCheck_Click, TPSvgimages.Num4);
        }

        private void CreateRuleGV()
        {
            // Quy tắc cảnh báo khi số lượng trong kho + máy < min
            var ruleCHECK = new GridFormatRule
            {
                Column = gColEvent,
                Name = "RuleCheck",
                Rule = new FormatConditionRuleExpression
                {
                    Expression = "[Transaction.TransactionType] = 'CHECK'",
                    Appearance = { ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical, }
                }
            };
            gvData.FormatRules.Add(ruleCHECK);

            var ruleIN = new GridFormatRule
            {
                Column = gColEvent,
                Name = "RuleIn",
                Rule = new FormatConditionRuleExpression
                {
                    Expression = "[Transaction.TransactionType] = 'IN'",
                    Appearance = { ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Question, }
                }
            };
            gvData.FormatRules.Add(ruleIN);

        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                transactions = dt309_TransactionsBUS.Instance.GetList();

                var materials = dt309_MaterialsBUS.Instance.GetListByIdDept(TPConfigs.LoginUser.IdDepartment);
                storages = dt309_StoragesBUS.Instance.GetList();
                users = dm_UserBUS.Instance.GetList();
                units = dt309_UnitsBUS.Instance.GetList();

                var displayData = (from t in transactions
                                   join m in materials on t.MaterialId equals m.Id
                                   join s in storages on t.StorageId equals s.Id
                                   join e in events on t.TransactionType equals e.Key
                                   let user = users.FirstOrDefault(u => u.Id == t.UserDo)
                                   select new
                                   {
                                       Transaction = t,
                                       Material = m,
                                       Storage = s,
                                       Event = e,
                                       User = user == null ? null : $"LG{user.IdDepartment}/{user.DisplayName}"
                                   }).ToList();

                sourceBases.DataSource = displayData;

                gvData.BestFitColumns();
                gvData.CollapseAllDetails();

                helper.LoadViewInfo();

                users = dm_UserBUS.Instance.GetList();
            }
        }

        private void uc309_InOutMgmt_Load(object sender, EventArgs e)
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
    }
}
