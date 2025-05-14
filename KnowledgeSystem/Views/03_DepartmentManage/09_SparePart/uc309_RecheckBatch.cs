using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Util;
using System.Windows.Forms;
using BusinessLayer;
using DataAccessLayer;
using DevExpress.Security;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using OfficeOpenXml.Drawing.Chart.Style;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml;
using System.IO;
using OfficeOpenXml.Table;
using ExcelDataReader;

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
        List<dt309_InspectionBatchMaterial> inspectionBatchMaterials;
        List<dt309_Units> units;

        DXMenuItem itemDownCheckFile;
        DXMenuItem itemUpdateCheckFile;
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
            itemDownCheckFile = CreateMenuItem("下載盤點表", ItemDownCheckFile_Click, TPSvgimages.Excel);
            itemUpdateCheckFile = CreateMenuItem("上傳盤點表", ItemUpdateCheckFile_Click, TPSvgimages.UploadFile);
        }

        private void ItemUpdateCheckFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel Files|*.xls;*.xlsx";
                openFileDialog.Title = "Chọn tệp Excel";

                if (openFileDialog.ShowDialog() != DialogResult.OK) return;

                string filePath = openFileDialog.FileName;
                try
                {
                    using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
                            {
                                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                                {
                                    UseHeaderRow = true  // Lấy tiêu đề từ hàng đầu tiên
                                }
                            });

                            // Đọc và hiển thị dữ liệu từ Sheet đầu tiên
                            DataTable table = result.Tables[0];

                            var batch = (gvData.GetRow(gvData.FocusedRowHandle) as dynamic).Batch as dt309_InspectionBatch;
                            int batchId = batch.Id;

                            var excelDatas = inspectionBatchMaterials.Where(r => r.BatchId == batchId);

                            foreach (var data in excelDatas)
                            {
                                // Tìm dòng có cột "編碼" khớp với MaterialId
                                DataRow row = table.AsEnumerable().FirstOrDefault(r =>
                                {
                                    try
                                    {
                                        // Lấy giá trị từ cột "編碼" và chuyển thành chuỗi để so sánh
                                        var codeValue = r["編碼"];
                                        return codeValue != null && codeValue.ToString() == data.MaterialId.ToString();
                                    }
                                    catch
                                    {
                                        return false;
                                    }
                                });

                                if (row != null)
                                {
                                    // Gán ActualQuantity từ cột "盤點數量"
                                    double actualQty;
                                    if (double.TryParse(row["盤點數量"]?.ToString(), out actualQty))
                                    {
                                        data.ActualQuantity = actualQty;
                                    }
                                }
                            }

                           // Xử lý hiện form để người dùng xác nhận lại thông tin pandian
                           f309_ReCheckInfo reCheckInfo = new f309_ReCheckInfo
                           {
                               InspectionBatchMaterials = excelDatas.ToList()
                           };
                            reCheckInfo.ShowDialog();

                            //// Cập nhật lại dữ liệu vào database
                            //foreach (var data in excelDatas)
                            //{
                            //    dt309_InspectionBatchMaterialBUS.Instance.Update(data);
                            //}

                            //// Load lại dữ liệu
                            //LoadData();
                            //MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }

        private void ItemDownCheckFile_Click(object sender, EventArgs e)
        {
            var batch = (gvData.GetRow(gvData.FocusedRowHandle) as dynamic).Batch as dt309_InspectionBatch;
            int batchId = batch.Id;

            var excelDatas = inspectionBatchMaterials
                .Where(r => r.BatchId == batchId)
                .Join(
                    materials,
                    bm => bm.MaterialId,
                    m => m.Id,
                    (bm, m) => new
                    {
                        Material = m,
                        BatchMaterial = bm
                    }
                )
                .Select(r => new
                {
                    r.Material.Id,
                    r.Material.Code,
                    r.Material.DisplayName,
                    r.Material.Location,
                    Unit = units.FirstOrDefault(x => x.Id == r.Material.IdUnit)?.DisplayName ?? "",
                    r.BatchMaterial.ActualQuantity,
                })
                .ToList();

            string documentsPath = TPConfigs.DocumentPath();
            if (!Directory.Exists(documentsPath))
                Directory.CreateDirectory(documentsPath);

            string filePath = Path.Combine(documentsPath, $"盤點表 - {DateTime.Now:yyyyMMddHHmm}.xlsx");

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using (ExcelPackage pck = new ExcelPackage(filePath))
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet1");
                ws.Cells.Style.Font.Name = "Microsoft JhengHei";
                ws.Cells.Style.Font.Size = 14;

                // Thiết lập độ rộng các cột
                ws.Column(1).Hidden = true;
                ws.Column(2).Width = 30;
                ws.Column(3).Width = 50;
                ws.Column(4).Width = 15;
                ws.Column(5).Width = 15;
                ws.Column(6).Width = 15;

                // Xuất dữ liệu từ list excelDatas sang Table
                ws.Cells["A1"].LoadFromCollection(excelDatas, true, OfficeOpenXml.Table.TableStyles.Medium2);

                // Đặt tiêu đề cột
                ws.Cells["A1"].Value = "編碼";
                ws.Cells["B1"].Value = "料號";
                ws.Cells["C1"].Value = "材料名稱";
                ws.Cells["D1"].Value = "地位";
                ws.Cells["E1"].Value = "單位";
                ws.Cells["F1"].Value = "盤點數量";

                // Bật WrapText cho tất cả các ô
                ws.Cells.Style.WrapText = true;

                //// Tự động điều chỉnh chiều cao từng dòng theo nội dung
                //int dataCount = excelDatas.Count;
                //for (int rowIndex = 1; rowIndex <= dataCount + 1; rowIndex++) // +1 để tính cả tiêu đề
                //{
                //    ws.Row(rowIndex).CustomHeight = true;
                //    ws.Row(rowIndex).Height = ws.Row(rowIndex).Height; // Đặt lại chiều cao tự động
                //}

                //// Khóa toàn bộ sheet với mật khẩu
                //ws.Protection.IsProtected = true;
                //ws.Protection.AllowSelectLockedCells = false;
                //ws.Protection.SetPassword("123456");  // Mật khẩu bảo vệ sheet

                //// Mở khóa vùng từ A2 đến A cuối của bảng
                //string range = $"A2:A{dataCount + 1}";
                //ws.Cells[range].Style.Locked = false;

                // Lưu file
                pck.Save();
            }

            Process.Start(filePath);

        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                deptGetData = (barCbbDept.EditValue?.ToString().Split(' ')[0]) ?? string.Empty;

                var inspectionBatch = dt309_InspectionBatchBUS.Instance.GetList();
                inspectionBatchMaterials = dt309_InspectionBatchMaterialBUS.Instance.GetList();
                materials = dt309_MaterialsBUS.Instance.GetListByStartIdDept(deptGetData);

                var depts = dm_DeptBUS.Instance.GetList();
                var users = dm_UserBUS.Instance.GetList();
                units = dt309_UnitsBUS.Instance.GetList();

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
                                  UserReCheck = string.IsNullOrEmpty(bm.ConfirmedBy) ? "" : users.FirstOrDefault(r => r.Id == bm.ConfirmedBy)?.DisplayName ?? "N/A",
                                  IniQuantity = bm.ActualQuantity.HasValue ? bm.InitialQuantity : (double?)null
                              })
                        .ToList();

                    return batchMaterialList.Any() ? new
                    {
                        Batch = batch,
                        Spare = batchMaterialList
                    } : null;
                }).Where(x => x != null).ToList();

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

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void gvData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow)
            {
                e.Menu.Items.Add(itemDownCheckFile);
                e.Menu.Items.Add(itemUpdateCheckFile);
            }
        }
    }
}
