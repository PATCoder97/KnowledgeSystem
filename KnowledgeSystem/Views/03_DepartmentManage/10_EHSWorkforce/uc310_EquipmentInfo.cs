using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraReports.UI;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using KnowledgeSystem.Views._03_DepartmentManage._05_PrintLabel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._10_EHSWorkforce
{
    public partial class uc310_EquipmentInfo : XtraUserControl
    {
        private readonly BindingSource sourceData = new BindingSource();

        private List<vw310_EquipmentInfo> equipmentInfos = new List<vw310_EquipmentInfo>();
        private List<dm_GroupUser> userGroups = new List<dm_GroupUser>();
        private List<string> editableDeptIds = new List<string>();
        private bool isEHSAdmin = false;
        private bool canManage = false;

        private DXMenuItem itemViewInfo;
        private DXMenuItem itemMultiselect;
        private DXMenuItem itemPrintStamp;

        public uc310_EquipmentInfo()
        {
            InitializeComponent();
            InitializeIcon();
            InitializeMenuItems();

            Font fontUI12 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI12;
        }

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
        }

        private DXMenuItem CreateMenuItem(string caption, EventHandler clickEvent, SvgImage svgImage)
        {
            DXMenuItem menuItem = new DXMenuItem(caption, clickEvent, svgImage, DXMenuItemPriority.Normal);
            menuItem.ImageOptions.SvgImageSize = new Size(24, 24);
            menuItem.AppearanceHovered.ForeColor = Color.Blue;
            return menuItem;
        }

        private void InitializeMenuItems()
        {
            itemViewInfo = CreateMenuItem("查看信息", ItemViewInfo_Click, TPSvgimages.View);
            itemMultiselect = CreateMenuItem("啟用多選", ItemMultiselect_Click, TPSvgimages.CheckedRadio);
            itemPrintStamp = CreateMenuItem("執行列印", ItemPrintStamp_Click, TPSvgimages.Print);
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                equipmentInfos = vw310_EquipmentInfoBUS.Instance.GetList();

                var displayData = equipmentInfos
                    .Select(r => new
                    {
                        r.Id,
                        r.Code,
                        r.DisplayNameVN,
                        r.DisplayNameTW,
                        DeptDisplay = string.Join(" ", new[] { r.DeptId, r.DeptNameTW }.Where(x => !string.IsNullOrWhiteSpace(x))),
                        ManagerDisplay = string.Join(" ", new[] { r.ManagerId, r.ManagerNameTW, r.ManagerNameVN }.Where(x => !string.IsNullOrWhiteSpace(x))),
                        r.Note
                    })
                    .ToList();

                sourceData.DataSource = displayData;
                gcData.DataSource = sourceData;

                gvData.BestFitColumns();
            }
        }

        private void ResolvePermission()
        {
            userGroups = dm_GroupUserBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id);

            List<dm_Group> ehsGroups = dm_GroupBUS.Instance.GetListContainName("安衛環");
            dm_Group adminGroup = ehsGroups.FirstOrDefault(r => string.Equals(r.DisplayName?.Trim(), "安衛環7", StringComparison.Ordinal));
            isEHSAdmin = adminGroup != null && userGroups.Any(r => r.IdGroup == adminGroup.Id);

            editableDeptIds = ehsGroups
                .Where(r => userGroups.Any(gu => gu.IdGroup == r.Id))
                .Select(r => (r.DisplayName ?? string.Empty).Replace("安衛環", string.Empty).Trim())
                .Where(r => !string.IsNullOrWhiteSpace(r) && r != "7" && r.All(char.IsDigit))
                .Distinct()
                .OrderBy(r => r)
                .ToList();

            canManage = isEHSAdmin || editableDeptIds.Count > 0;
        }

        private List<vw310_EquipmentInfo> GetSelectedEquipmentInfos()
        {
            List<int> selectedIds = gvData.GetSelectedRows()
                .Where(r => r >= 0)
                .Select(r => Convert.ToInt32(gvData.GetRowCellValue(r, gColId) ?? -1))
                .Where(r => r > 0)
                .Distinct()
                .ToList();

            if (selectedIds.Count == 0)
            {
                int focusedId = Convert.ToInt32(gvData.GetRowCellValue(gvData.FocusedRowHandle, gColId) ?? -1);
                if (focusedId > 0)
                {
                    selectedIds.Add(focusedId);
                }
            }

            return equipmentInfos
                .Where(r => selectedIds.Contains(r.Id))
                .OrderBy(r => selectedIds.IndexOf(r.Id))
                .ToList();
        }

        private void ShowPrintPreview(IEnumerable<vw310_EquipmentInfo> items)
        {
            List<object> labels = items
                .Select(r => new
                {
                    NameVN = r.DisplayNameVN ?? string.Empty,
                    NameTW = r.DisplayNameTW ?? string.Empty,
                    Code = r.Code ?? string.Empty,
                    Dept = r.DeptId ?? string.Empty,
                    UserVN = r.ManagerNameVN ?? string.Empty,
                    UserTW = r.ManagerNameTW ?? string.Empty
                })
                .Cast<object>()
                .ToList();

            if (labels.Count == 0)
            {
                XtraMessageBox.Show("請先選擇要列印的設備資料。", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            XtraReport report = new rpDeviceManagement();
            report.DataSource = labels;
            report.CreateDocument();
            report.PrintingSystem.ShowMarginsWarning = false;

            using (f00_PrintReport printReport = new f00_PrintReport())
            {
                printReport.ViewerReport.DocumentSource = report;
                printReport.ShowDialog(this);
            }
        }

        private void ItemMultiselect_Click(object sender, EventArgs e)
        {
            bool multiSelect = gvData.OptionsSelection.MultiSelect;
            gvData.OptionsSelection.MultiSelect = !multiSelect;
            gvData.OptionsSelection.MultiSelectMode = !multiSelect
                ? GridMultiSelectMode.CheckBoxRowSelect
                : GridMultiSelectMode.RowSelect;

            if (multiSelect)
            {
                gvData.ClearSelection();
            }
        }

        private void ItemPrintStamp_Click(object sender, EventArgs e)
        {
            ShowPrintPreview(GetSelectedEquipmentInfos());
        }

        private void ItemViewInfo_Click(object sender, EventArgs e)
        {
            int idBase = Convert.ToInt32(gvData.GetRowCellValue(gvData.FocusedRowHandle, gColId) ?? -1);
            if (idBase <= 0)
            {
                return;
            }

            using (var finfo = new f310_EquipmentInfo_Info
            {
                eventInfo = EventFormInfo.View,
                formName = "設備",
                idBase = idBase
            })
            {
                finfo.ShowDialog(this);
            }

            LoadData();
        }

        private void uc310_EquipmentInfo_Load(object sender, EventArgs e)
        {
            ResolvePermission();

            if (!canManage)
            {
                btnAdd.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }

            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsView.ShowGroupPanel = false;

            LoadData();
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!canManage)
            {
                return;
            }

            using (var finfo = new f310_EquipmentInfo_Info
            {
                eventInfo = EventFormInfo.Create,
                formName = "設備"
            })
            {
                finfo.ShowDialog(this);
            }

            LoadData();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void gvData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow)
            {
                gvData.FocusedRowHandle = e.HitInfo.RowHandle;

                bool multiSelect = gvData.OptionsSelection.MultiSelect;
                itemMultiselect.Caption = multiSelect ? "啟用單選" : "啟用多選";
                itemMultiselect.BeginGroup = true;

                e.Menu.Items.Add(itemViewInfo);
                e.Menu.Items.Add(itemMultiselect);
                e.Menu.Items.Add(itemPrintStamp);
            }
        }
    }
}
