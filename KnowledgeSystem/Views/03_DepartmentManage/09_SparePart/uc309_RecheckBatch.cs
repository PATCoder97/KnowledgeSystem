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
using System.Transactions;
using Org.BouncyCastle.Asn1.Cmp;

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
        DXMenuItem itemDownUnusualFile;
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
            var ruleNotifyMain = new GridFormatRule
            {
                ApplyToRow = false,
                Column = gColStatus,
                Name = "RuleNotifyMain",
                Rule = new FormatConditionRuleExpression
                {
                    Expression = "[Status] == '待處理'",
                    Appearance =
                    {
                        ForeColor  = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical
                    }
                }
            };
            gvData.FormatRules.Add(ruleNotifyMain);

            //var ruleNotifyMainOK = new GridFormatRule
            //{
            //    ApplyToRow = false,
            //    Column = gColStatus,
            //    Name = "RuleNotifyMainOK",
            //    Rule = new FormatConditionRuleExpression
            //    {
            //        Expression = "[Status] == '已完成'",
            //        Appearance =
            //        {
            //            ForeColor  = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Information
            //        }
            //    }
            //};
            //gvData.FormatRules.Add(ruleNotifyMainOK);

            var ruleNotify = new GridFormatRule
            {
                ApplyToRow = true,
                Name = "RuleNotify",
                Rule = new FormatConditionRuleExpression
                {
                    Expression = "[BatchMaterial.IsComplete] != true",
                    Appearance = { ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical }
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
            bool uploadDesc = (sender as DXMenuItem).Caption.Contains("異常");

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

                            var excelDatas = inspectionBatchMaterials
                                .Where(r => r.BatchId == batchId && r.IsComplete != true)
                                .Join(
                                    materials.Where(x => x.IdDept.StartsWith(deptGetData)),
                                    bm => bm.MaterialId,
                                    m => m.Id,
                                    (bm, m) => bm
                                )
                                .ToList();

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

                                if (row == null) continue;

                                if (uploadDesc)
                                {
                                    data.Description = row["異常說明"]?.ToString().Trim();
                                }
                                else
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
                                InspectionBatchMaterials = excelDatas.ToList(),
                                Text = $"物料盤點明細表"
                            };
                            reCheckInfo.ShowDialog();

                            if (uploadDesc)
                            {
                                foreach (var data in excelDatas.Where(r => r.ActualQuantity != null))
                                {
                                    data.ConfirmationDate = DateTime.Today;
                                    data.ConfirmedBy = TPConfigs.LoginUser.Id;
                                    data.IsComplete = !string.IsNullOrEmpty(data.Description);
                                    dt309_InspectionBatchMaterialBUS.Instance.AddOrUpdate(data);
                                }
                            }
                            else
                            {
                                // Cập nhật lại dữ liệu vào database
                                foreach (var data in excelDatas.Where(r => r.ActualQuantity != null))
                                {
                                    data.ConfirmationDate = DateTime.Today;
                                    data.ConfirmedBy = TPConfigs.LoginUser.Id;
                                    data.IsComplete = data.InitialQuantity == data.ActualQuantity;
                                    dt309_InspectionBatchMaterialBUS.Instance.AddOrUpdate(data);

                                    dt309_TransactionsBUS.Instance.Add(new dt309_Transactions()
                                    {
                                        CreatedDate = DateTime.Now,
                                        MaterialId = data.MaterialId,
                                        TransactionType = "check",
                                        Quantity = (double)data.ActualQuantity,
                                        UserDo = TPConfigs.LoginUser.Id,
                                        Desc = "pandian",
                                        StorageId = 1
                                    });
                                }
                            }
                            // Load lại dữ liệu
                            LoadData();
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
            bool uploadDesc = (sender as DXMenuItem).Caption.Contains("異常");

            var batch = (gvData.GetRow(gvData.FocusedRowHandle) as dynamic).Batch as dt309_InspectionBatch;
            int batchId = batch.Id;

            var excelDatas = inspectionBatchMaterials
                .Where(r => r.BatchId == batchId && r.IsComplete != true)
                .Join(
                    materials.Where(x => deptGetData.Contains(x.IdDept)),
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
                    Quantity = uploadDesc ? r.BatchMaterial.InitialQuantity : -1,
                    r.BatchMaterial.ActualQuantity,
                    Desc = ""
                })
                .ToList();

            string documentsPath = TPConfigs.DocumentPath();
            if (!Directory.Exists(documentsPath))
                Directory.CreateDirectory(documentsPath);

            string filePath = Path.Combine(documentsPath, $"盤點表 - {DateTime.Now:yyyyMMddHHmmss}.xlsx");

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
                ws.Column(7).Width = 15;
                ws.Column(8).Width = 30;

                // Xuất dữ liệu từ list excelDatas sang Table
                ws.Cells["A1"].LoadFromCollection(excelDatas, true, OfficeOpenXml.Table.TableStyles.Medium2);

                // Đặt tiêu đề cột
                ws.Cells["A1"].Value = "編碼";
                ws.Cells["B1"].Value = "料號";
                ws.Cells["C1"].Value = "材料名稱";
                ws.Cells["D1"].Value = "地位";
                ws.Cells["E1"].Value = "單位";
                ws.Cells["F1"].Value = "系統上數量";
                ws.Cells["G1"].Value = "盤點數量";
                ws.Cells["H1"].Value = "異常說明";

                // Bật WrapText cho tất cả các ô
                ws.Cells.Style.WrapText = true;

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
                                  IniQuantity = bm.ActualQuantity.HasValue ? bm.InitialQuantity : (double?)null,
                                  Description = bm.Description,
                                  IsComplete = bm.IsComplete
                              })
                        .ToList();

                    return batchMaterialList.Any() ? new
                    {
                        Batch = batch,
                        Spare = batchMaterialList,
                        Status = batchMaterialList.Any(r => r.IsComplete != true) ? "待處理" : "已完成"
                    } : null;
                }).Where(x => x != null).ToList();

                sourceBases.DataSource = displayData;

                gvData.BestFitColumns();
                gvData.CollapseAllDetails();

                helper.LoadViewInfo();

                int rowHandle = gvData.FocusedRowHandle;
                if (gvData.IsMasterRow(rowHandle))
                {
                    gvData.ExpandMasterRow(rowHandle);
                }
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
            if (deptGetData.Length != 4)
            {
                XtraMessageBox.Show("請您選擇「課」來查看物料", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!e.HitInfo.InRowCell || !e.HitInfo.InDataRow || e.Menu == null)
                return;

            GridView masterView = sender as GridView;
            int masterRowHandle = e.HitInfo.RowHandle;

            // Lấy detail view từ dòng cha
            GridView detailView = masterView.GetDetailView(masterRowHandle, 0) as GridView;
            if (detailView == null)
                return;

            // Lấy dòng dữ liệu cha
            dynamic masterRow = masterView.GetRow(masterRowHandle);
            if (masterRow == null)
                return;

            // Lấy danh sách Spare từ dòng cha
            var spareList = (masterRow.Spare as IEnumerable<dynamic>)?.ToList();
            if (spareList == null || spareList.Count == 0)
                return;

            // Lấy danh sách BatchMaterial
            var batchMaterials = spareList
                .Select(item => item.BatchMaterial as dt309_InspectionBatchMaterial)
                .Where(b => b != null)
                .ToList();

            // Kiểm tra trạng thái hoàn thành
            bool hasCompletedItem = batchMaterials.Any(x => x.IsComplete == true);
            bool hasIncompleteItem = batchMaterials.Any(x => x.IsComplete != true);

            // Cập nhật caption theo trạng thái
            if (hasCompletedItem)
            {
                itemDownCheckFile.Caption = "下載異常表";
                itemUpdateCheckFile.Caption = "上傳異常表";
            }
            else
            {
                itemDownCheckFile.Caption = "下載盤點表";
                itemUpdateCheckFile.Caption = "上傳盤點表";
            }

            // Nếu đã hoàn thành hết thì trờ về
            if (!hasIncompleteItem)
                return;

            // Thêm item vào menu
            e.Menu.Items.Add(itemDownCheckFile);
            e.Menu.Items.Add(itemUpdateCheckFile);
        }
    }
}
