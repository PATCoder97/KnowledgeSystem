using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraReports;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            barExport.ImageOptions.SvgImage = TPSvgimages.Excel;
            barCbbDept.ImageOptions.SvgImage = TPSvgimages.Dept;

            btnMachineList.ImageOptions.SvgImage = TPSvgimages.Num1;
            btnSummary.ImageOptions.SvgImage = TPSvgimages.Num2;
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
                idBase = idMachine,
                idDeptGetData = deptGetData
            };
            fInfo.ShowDialog();

            LoadData();
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                deptGetData = string.IsNullOrWhiteSpace(barCbbDept.EditValue?.ToString()) ? "NoDept" : barCbbDept.EditValue.ToString().Split(' ')[0];
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
                        Material = machineMaterialList,
                        TotalPrice = totalPrice,
                        consumableCount,
                        equipmentCount,
                        IdDept = machine.IdDept
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
                    var materialsChild = parentRow.Material as IEnumerable<dynamic>;

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
            if (deptGetData.Length != 4)
            {
                XtraMessageBox.Show("請您選擇「課」來查看物料", "錯誤",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow)
            {
                e.Menu.Items.Add(itemViewInfo);
            }
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f309_Machine_Info finfo = new f309_Machine_Info()
            {
                eventInfo = EventFormInfo.Create,
                formName = "用料設備",
                idDeptGetData = deptGetData
            };
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

        public static List<int> GetVisibleDataIds(GridView view)
        {
            var ids = new List<int>();

            for (int i = 0; i < view.DataRowCount; i++)
            {
                int rowHandle = view.GetVisibleRowHandle(i);

                // Cách nhanh: nếu có cột/field "data.Id" trong Grid (cột ẩn cũng được)
                var cell = view.GetRowCellValue(rowHandle, "Machine.Id");
                if (cell != null && int.TryParse(cell.ToString(), out int idFromCell))
                {
                    ids.Add(idFromCell);
                    continue;
                }

                // Fallback: lấy từ object ẩn danh bằng reflection
                var row = view.GetRow(rowHandle);
                if (row == null) continue;

                var dataProp = row.GetType().GetProperty("data");
                var dataVal = dataProp?.GetValue(row, null);
                var idProp = dataVal?.GetType().GetProperty("Id");
                var idVal = idProp?.GetValue(dataVal, null)?.ToString();

                if (int.TryParse(idVal, out int idFromProp))
                    ids.Add(idFromProp);
            }

            return ids;
        }

        private void btnMachineList_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var ids = GetVisibleDataIds(gvData);

            var displayData = machines
                .Where(machine => ids.Contains(machine.Id)) // 🔸 chỉ lấy những máy có trong ids
                .Select(machine =>
                {
                    var machineMaterialList = machineMaterials
                        .Where(mm => mm.MachineId == machine.Id)
                        .Join(materials,
                              mm => mm.MaterialId,
                              m => m.Id,
                              (mm, m) => m)
                        .ToList();

                    // 🔸 Ép kiểu để đảm bảo kết quả đúng
                    double totalPrice = machineMaterialList.Sum(m =>
                        Convert.ToDouble(m.Price) *
                        (Convert.ToDouble(m.QuantityInStorage) + Convert.ToDouble(m.QuantityInMachine)));

                    // 🔸 Đếm loại vật tư
                    int consumableCount = machineMaterialList.Count(r => r.TypeUse == "消耗品");
                    int equipmentCount = machineMaterialList.Count(r => r.TypeUse == "備品");

                    return new
                    {
                        單位 = machine.IdDept,
                        設備名稱 = machine.DisplayName,
                        地點 = machine.Location,
                        數量 = machine.Quantity,
                        總金額 = totalPrice,
                        消耗品種類 = consumableCount,
                        備品種類 = equipmentCount,
                        重要等級 = machine.ImpLevel
                    };
                }).ToList();

            string documentsPath = TPConfigs.DocumentPath();
            if (!Directory.Exists(documentsPath))
                Directory.CreateDirectory(documentsPath);

            string filePath = Path.Combine(documentsPath, $"設備清單 - {DateTime.Now:yyyyMMddHHmmss}.xlsx");

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (ExcelPackage pck = new ExcelPackage(filePath))
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet1");
                ws.Cells.Style.Font.Name = "Microsoft JhengHei";
                ws.Cells.Style.Font.Size = 14;

                // Xuất dữ liệu từ list excelDatas sang Table
                ws.Cells["A1"].LoadFromCollection(displayData, true, OfficeOpenXml.Table.TableStyles.Medium2);
                // Bật WrapText cho tất cả các ô
                ws.Column(3).Style.WrapText = true;
                ws.Cells[ws.Dimension.Address].AutoFitColumns();
                ws.Column(3).Width = 35;

                // Lưu file
                pck.Save();
            }

            Process.Start(filePath);
        }

        private void btnSummary_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var machines = dt309_MachinesBUS.Instance.GetListByStartIdDept(deptGetData);
            var materials = dt309_MaterialsBUS.Instance.GetList();
            var machineMaterials = dt309_MachineMaterialsBUS.Instance.GetList();
            var depts = dm_DeptBUS.Instance.GetList();

            // Nhóm máy theo IdDept
            var summaryByDept = machines
                .GroupBy(m => m.IdDept)
                .Select(group =>
                {
                    var deptId = group.Key;

                    // Các máy trong phòng ban này
                    var deptMachineIds = group.Select(m => m.Id).ToList();

                    // Tính tổng số lượng máy trong phòng ban
                    double totalMachineQty = group.Sum(m => Convert.ToDouble(m.Quantity));

                    // Lấy danh sách vật tư gắn với các máy này
                    var flatMaterials = machineMaterials
                        .Where(mm => deptMachineIds.Contains(mm.MachineId))
                        .Join(materials,
                              mm => mm.MaterialId,
                              m => m.Id,
                              (mm, m) => m)
                        .ToList();

                    // Loại trùng vật tư theo Id
                    var distinctMaterials = flatMaterials
                        .Where(r => r.TypeUse == "備品")
                        .GroupBy(m => m.Id)
                        .Select(g => g.First())
                        .ToList();

                    // Tính tổng tồn kho, trong máy
                    double sumStorage = distinctMaterials.Sum(m => Convert.ToDouble(m.QuantityInStorage));
                    double sumMachine = distinctMaterials.Sum(m => Convert.ToDouble(m.QuantityInMachine));
                    double totalMaterial = sumStorage + sumMachine;
                    var dept = depts.FirstOrDefault(r => r.Id == deptId);

                    return new
                    {
                        Dept = $"{dept?.Id} {dept?.DisplayName}",
                        totalMachineQty,
                        sumStorage,
                        sumMachine,
                        totalMaterial
                    };
                })
                .OrderBy(r => r.Dept)
                .ToList();


            string documentsPath = TPConfigs.DocumentPath();
            if (!Directory.Exists(documentsPath))
                Directory.CreateDirectory(documentsPath);

            string filePath = Path.Combine(documentsPath, $"設備匯總表 - {DateTime.Now:yyyyMMddHHmmss}.xlsx");

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (ExcelPackage pck = new ExcelPackage(filePath))
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet1");
                ws.Cells.Style.Font.Name = "Microsoft JhengHei";
                ws.Cells.Style.Font.Size = 14;

                ws.Cells["A1"].Value = "部門";
                ws.Cells["B1"].Value = "設備數量";
                ws.Cells["C1"].Value = "課庫數量";
                ws.Cells["D1"].Value = "機邊庫數量";
                ws.Cells["E1"].Value = "備品數量";

                // Xuất dữ liệu từ list excelDatas sang Table
                ws.Cells["A2"].LoadFromCollection(summaryByDept, false);

                //ws.Column(1).Style.WrapText = true;
                ws.Cells[ws.Dimension.Address].AutoFitColumns();

                // Căn giữa và kẻ ô toàn bảng
                var fullRange = ws.Cells[ws.Dimension.Address];
                fullRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                fullRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                fullRange.Style.Border.Top.Style = fullRange.Style.Border.Bottom.Style =
                    fullRange.Style.Border.Left.Style = fullRange.Style.Border.Right.Style =
                    ExcelBorderStyle.Thin;

                // Lưu file
                pck.Save();
            }

            Process.Start(filePath);
        }
    }
}
