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
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Items.ViewInfo;
using DevExpress.XtraSplashScreen;
using DocumentFormat.OpenXml.Spreadsheet;
using KnowledgeSystem.Helpers;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;

namespace KnowledgeSystem.Views._04_SystemAdministrator._03_Extension._04_HardnessStandardLog
{
    public partial class uc403_04_HardnessStandardLog : DevExpress.XtraEditors.XtraUserControl
    {
        public uc403_04_HardnessStandardLog()
        {
            InitializeComponent();
            InitializeIcon();
            InitializeMenuItems();

            helper = new RefreshHelper(gvData, "Id");

            Font fontUI12 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI12;

            barCbbDept.EditValueChanged += CbbDept_EditValueChanged;
        }

        private void CreateRuleGV()
        {
            // Quy tắc định dạng khi TransactionType = 'C'
            var ruleCHECK = new GridFormatRule
            {
                Column = gColDesc,
                Name = "RuleCheck",
                Rule = new FormatConditionRuleExpression
                {
                    Expression = "[Desc] = '不合格'",
                    Appearance = { ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical, }
                }
            };
            gvData.FormatRules.Add(ruleCHECK);
        }

        private void CbbDept_EditValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        RefreshHelper helper;
        BindingSource sourceBases = new BindingSource();
        string idDept2word = TPConfigs.idDept2word;
        string deptGetData = "";
        bool isLevel2Boss = false;

        DXMenuItem itemDelInfo;

        private void InitializeIcon()
        {
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            barCbbDept.ImageOptions.SvgImage = TPSvgimages.Dept;
            btnStandardInfo.ImageOptions.SvgImage = TPSvgimages.Gears;
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
            itemDelInfo = CreateMenuItem("刪除資料", ItemDelInfo_Click, TPSvgimages.Remove);
        }

        private void ItemDelInfo_Click(object sender, EventArgs e)
        {
            var dialogResult = XtraMessageBox.Show($"您確認要刪除這資料？", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult != DialogResult.Yes) return;

            GridView view = gvData;
            int idData = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId));

            var result = dt403_04_HardnessStandardLogBUS.Instance.RemoveById(idData);

            if (result)
            {
                LoadData();
            }
        }


        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                var usrs = dm_UserBUS.Instance.GetList();
                deptGetData = string.IsNullOrWhiteSpace(barCbbDept.EditValue?.ToString()) ? "NoDept" : barCbbDept.EditValue.ToString().Split(' ')[0];

                var dataLogs = dt403_04_HardnessStandardLogBUS.Instance.GetListByDept(deptGetData);

                var displayData = (from data in dataLogs
                                   select new
                                   {
                                       data,
                                       TesterName = usrs.FirstOrDefault(r => r.Id == data.Tester)?.DisplayName ?? "無效"
                                   }).OrderByDescending(r => r.data.TimeCreate).ToList();

                sourceBases.DataSource = displayData;

                gvData.BestFitColumns();
                gvData.CollapseAllDetails();

                helper.LoadViewInfo();
            }
        }

        private void uc403_04_HardnessStandardLog_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;

            // Kiểm tra quyền từng ke để có quyền truy cập theo nhóm
            var userGroups = dm_GroupUserBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id);
            var departments = dm_DeptBUS.Instance.GetList();
            var groups = dm_GroupBUS.Instance.GetListByName("硬度數據管理");

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

            var groupLever2 = dm_GroupBUS.Instance.GetListByName($"二級{deptGetData}");
            isLevel2Boss = userGroups.Any(r => r.IdGroup == groupLever2.FirstOrDefault()?.Id);
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void gvData_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;

            if (e.IsGetData)
            {
                switch (e.Column.FieldName)
                {
                    case "Error":
                        decimal value1 = Convert.ToDecimal(view.GetListSourceRowCellValue(e.ListSourceRowIndex, "data.TestValue"));
                        decimal value2 = Convert.ToDecimal(view.GetListSourceRowCellValue(e.ListSourceRowIndex, "data.StandardValue"));
                        decimal error = value1 - value2;

                        e.Value = error;
                        break;

                    case "Desc":
                        value1 = Convert.ToDecimal(view.GetListSourceRowCellValue(e.ListSourceRowIndex, "data.AllowableError"));
                        value2 = Convert.ToDecimal(view.GetListSourceRowCellValue(e.ListSourceRowIndex, "Error"));
                        bool pass = Math.Abs(value2) <= value1;

                        e.Value = pass ? "合格" : "不合格";
                        break;
                }
            }
        }

        private void btnStandardInfo_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (deptGetData.Length != 4)
            {
                XtraMessageBox.Show("請您選擇「課」來查看標準試片", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            f403_04_StandardInfo form = new f403_04_StandardInfo()
            {
                Text = btnStandardInfo.Caption,
                deptGetData = deptGetData,
            };
            form.ShowDialog();
        }

        private void gvData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow && isLevel2Boss)
            {
                e.Menu.Items.Add(itemDelInfo);
            }
        }
    }
}
